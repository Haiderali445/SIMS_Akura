using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.DAL
{
    public class CustomersDAL
    {
        // 1) Master listing: show ALL accounts of type 'Customer' with optional active filter
        public List<Customer> GetAllFromAccounts(bool onlyActiveAccounts = false)
        {
            var list = new List<Customer>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT 
                        a.id              AS AccountId,
                        a.name            AS AccountName,
                        a.account_code    AS AccountCode,
                        a.account_type    AS AccountType,
                        a.is_active       AS AccountIsActive,

                        c.id              AS CustomerId,
                        c.name            AS CustomerName,
                        c.phone           AS Phone,
                        c.email           AS Email,
                        c.address         AS Address,
                        c.is_active       AS CustomerIsActive,
                        c.created_at      AS CreatedAt
                    FROM Accounts a
                    LEFT JOIN Customers c ON c.account_id = a.id
                    WHERE a.account_type = 'Customer' " + (onlyActiveAccounts ? "AND a.is_active = 1 " : "") + @"
                    ORDER BY COALESCE(c.name, a.name)";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(MapFromAccountJoin(r));
                }
            }
            return list;
        }

        // 2) Search across accounts of type customer + linked customers
        public List<Customer> SearchFromAccounts(string keyword, bool onlyActiveAccounts = false)
        {
            var list = new List<Customer>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT 
                        a.id              AS AccountId,
                        a.name            AS AccountName,
                        a.account_code    AS AccountCode,
                        a.account_type    AS AccountType,
                        a.is_active       AS AccountIsActive,

                        c.id              AS CustomerId,
                        c.name            AS CustomerName,
                        c.phone           AS Phone,
                        c.email           AS Email,
                        c.address         AS Address,
                        c.is_active       AS CustomerIsActive,
                        c.created_at      AS CreatedAt
                    FROM Accounts a
                    LEFT JOIN Customers c ON c.account_id = a.id
                    WHERE a.account_type = 'Customer' " + (onlyActiveAccounts ? "AND a.is_active = 1 " : "") + @"
                      AND (
                           a.account_code LIKE @kw OR
                           a.name        LIKE @kw OR
                           c.name        LIKE @kw OR
                           c.phone       LIKE @kw OR
                           c.email       LIKE @kw OR
                           c.address     LIKE @kw
                          )
                    ORDER BY COALESCE(c.name, a.name)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(MapFromAccountJoin(r));
                }
            }
            return list;
        }

        // 3) Get a single Customer by its Customer.id (strict: requires customer row exists)
        public Customer GetByCustomerId(long id)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT 
                        a.id              AS AccountId,
                        a.name            AS AccountName,
                        a.account_code    AS AccountCode,
                        a.account_type    AS AccountType,
                        a.is_active       AS AccountIsActive,

                        c.id              AS CustomerId,
                        c.name            AS CustomerName,
                        c.phone           AS Phone,
                        c.email           AS Email,
                        c.address         AS Address,
                        c.is_active       AS CustomerIsActive,
                        c.created_at      AS CreatedAt
                    FROM Customers c
                    INNER JOIN Accounts a ON c.account_id = a.id
                    WHERE c.id = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                        return MapFromAccountJoin(r);
                }
            }
            return null;
        }

        // 4) Get by AccountId (works even when customer row is missing)
        public Customer GetByAccountId(long accountId)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT 
                        a.id              AS AccountId,
                        a.name            AS AccountName,
                        a.account_code    AS AccountCode,
                        a.account_type    AS AccountType,
                        a.is_active       AS AccountIsActive,

                        c.id              AS CustomerId,
                        c.name            AS CustomerName,
                        c.phone           AS Phone,
                        c.email           AS Email,
                        c.address         AS Address,
                        c.is_active       AS CustomerIsActive,
                        c.created_at      AS CreatedAt
                    FROM Accounts a
                    LEFT JOIN Customers c ON c.account_id = a.id
                    WHERE a.id = @accId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@accId", accountId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                        return MapFromAccountJoin(r);
                }
            }
            return null;
        }

        // 5) Create a Customer row (requires AccountId)
        public bool Add(Customer c)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    INSERT INTO Customers (name, account_id, phone, email, address, is_active, created_at)
                    VALUES (@name, @acc, @phone, @mail, @addr, @active, GETUTCDATE())";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", c.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@acc", c.AccountId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", c.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@mail", c.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@addr", c.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@active", c.IsActive);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 6) Update existing Customer row
        public bool Update(Customer c)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    UPDATE Customers SET
                        name = @name,
                        account_id = @acc,
                        phone = @phone,
                        email = @mail,
                        address = @addr,
                        is_active = @active
                    WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", c.Id);
                cmd.Parameters.AddWithValue("@name", c.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@acc", c.AccountId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", c.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@mail", c.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@addr", c.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@active", c.IsActive);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 7) Delete Customer row by id
        public bool Delete(long id)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Customers WHERE id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 8) Search within Customers table only (classic)
        public List<Customer> SearchClassic(string keyword)
        {
            var list = new List<Customer>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT c.*,
                           a.name AS AccountName,
                           a.account_code AS AccountCode,
                           a.account_type AS AccountType
                    FROM Customers c
                    LEFT JOIN Accounts a ON c.account_id = a.id
                    WHERE c.name LIKE @kw
                       OR c.phone LIKE @kw
                       OR c.email LIKE @kw
                       OR c.address LIKE @kw
                       OR a.account_code LIKE @kw
                       OR a.account_type LIKE @kw
                    ORDER BY c.name";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(MapClassic(r));
                }
            }
            return list;
        }

        // Safe mappers

        private Customer MapFromAccountJoin(SqlDataReader r)
        {
            bool hasCustomerRow = r["CustomerId"] != DBNull.Value;

            return new Customer
            {
                Id = hasCustomerRow ? Convert.ToInt64(r["CustomerId"]) : 0,
                Name = hasCustomerRow ? (r["CustomerName"]?.ToString() ?? r["AccountName"]?.ToString())
                                             : (r["AccountName"]?.ToString()),
                AccountId = r["AccountId"] == DBNull.Value ? (long?)null : Convert.ToInt64(r["AccountId"]),
                AccountName = r["AccountName"]?.ToString(),
                AccountCode = r["AccountCode"]?.ToString(),
                AccountType = r["AccountType"]?.ToString(),

                Phone = hasCustomerRow ? r["Phone"]?.ToString() : null,
                Email = hasCustomerRow ? r["Email"]?.ToString() : null,
                Address = hasCustomerRow ? r["Address"]?.ToString() : null,

                // Prefer Customer.IsActive when row exists, else fallback to Account.IsActive
                IsActive = hasCustomerRow
                              ? Convert.ToBoolean(r["CustomerIsActive"])
                              : Convert.ToBoolean(r["AccountIsActive"]),

                CreatedAt = hasCustomerRow && r["CreatedAt"] != DBNull.Value
                              ? Convert.ToDateTime(r["CreatedAt"])
                              : DateTime.MinValue
            };
        }

        private Customer MapClassic(SqlDataReader r)
        {
            return new Customer
            {
                Id = Convert.ToInt64(r["id"]),
                Name = r["name"]?.ToString(),
                AccountId = r["account_id"] == DBNull.Value ? (long?)null : Convert.ToInt64(r["account_id"]),
                AccountName = r["AccountName"]?.ToString(),
                AccountCode = r["AccountCode"]?.ToString(),
                AccountType = r["AccountType"]?.ToString(),
                Phone = r["phone"]?.ToString(),
                Email = r["email"]?.ToString(),
                Address = r["address"]?.ToString(),
                IsActive = Convert.ToBoolean(r["is_active"]),
                CreatedAt = Convert.ToDateTime(r["created_at"])
            };
        }
    }
}
