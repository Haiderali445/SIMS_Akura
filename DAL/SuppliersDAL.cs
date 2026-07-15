using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.DAL
{
    public class SuppliersDAL
    {
        // ✅ Get all suppliers
        public List<Supplier> GetAll()
        {
            var list = new List<Supplier>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT s.*, a.name AS AccountName, a.account_code AS AccountCode
                    FROM Suppliers s
                    LEFT JOIN Accounts a ON s.account_id = a.id
                    ORDER BY s.name";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    list.Add(new Supplier
                    {
                        Id = Convert.ToInt64(r["id"]),
                        Name = r["name"].ToString(),
                        AccountId = r["account_id"] == DBNull.Value ? null : (long?)Convert.ToInt64(r["account_id"]),
                        AccountName = r["AccountName"]?.ToString(),
                        AccountCode = r["AccountCode"]?.ToString(),
                        Phone = r["phone"]?.ToString(),
                        Email = r["email"]?.ToString(),
                        Address = r["address"]?.ToString(),
                        IsActive = Convert.ToBoolean(r["is_active"]),
                        CreatedAt = Convert.ToDateTime(r["created_at"])
                    });
                }
            }
            return list;
        }


        // ✅ Get supplier by ID
        public Supplier GetById(long id)
        {
            Supplier s = null;
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT s.*, a.name AS AccountName, a.account_code AS AccountCode
                    FROM Suppliers s
                    LEFT JOIN Accounts a ON s.account_id = a.id
                    WHERE s.id = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();

                if (r.Read())
                {
                    s = new Supplier
                    {
                        Id = Convert.ToInt64(r["id"]),
                        Name = r["name"].ToString(),
                        AccountId = r["account_id"] == DBNull.Value ? null : (long?)Convert.ToInt64(r["account_id"]),
                        AccountName = r["AccountName"]?.ToString(),
                        AccountCode = r["AccountCode"]?.ToString(),
                        Phone = r["phone"]?.ToString(),
                        Email = r["email"]?.ToString(),
                        Address = r["address"]?.ToString(),
                        IsActive = Convert.ToBoolean(r["is_active"]),
                        CreatedAt = Convert.ToDateTime(r["created_at"])
                    };
                }
            }
            return s;
        }

        // ✅ Add supplier
        public bool Add(Supplier s)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    INSERT INTO Suppliers (name, account_id, phone, email, address, is_active, created_at)
                    VALUES (@name, @acc, @phone, @mail, @addr, @active, GETUTCDATE())";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", s.Name);
                cmd.Parameters.AddWithValue("@acc", s.AccountId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", s.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@mail", s.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@addr", s.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@active", s.IsActive);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ✅ Update supplier
        public bool Update(Supplier s)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    UPDATE Suppliers SET
                        name = @name,
                        account_id = @acc,
                        phone = @phone,
                        email = @mail,
                        address = @addr,
                        is_active = @active
                    WHERE id = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", s.Id);
                cmd.Parameters.AddWithValue("@name", s.Name);
                cmd.Parameters.AddWithValue("@acc", s.AccountId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", s.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@mail", s.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@addr", s.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@active", s.IsActive);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ✅ Delete supplier
        public bool Delete(long id)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Suppliers WHERE id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ✅ Search suppliers by keyword
        public List<Supplier> Search(string keyword)
        {
            var list = new List<Supplier>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT s.*, a.name AS AccountName, a.account_code AS AccountCode
                    FROM Suppliers s
                    LEFT JOIN Accounts a ON s.account_id = a.id
                    WHERE s.name LIKE @kw OR s.phone LIKE @kw OR s.email LIKE @kw OR s.address LIKE @kw
                    ORDER BY s.name";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                con.Open();
                SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    list.Add(new Supplier
                    {
                        Id = Convert.ToInt64(r["id"]),
                        Name = r["name"].ToString(),
                        AccountId = r["account_id"] == DBNull.Value ? null : (long?)Convert.ToInt64(r["account_id"]),
                        AccountName = r["AccountName"]?.ToString(),
                        AccountCode = r["AccountCode"]?.ToString(),
                        Phone = r["phone"]?.ToString(),
                        Email = r["email"]?.ToString(),
                        Address = r["address"]?.ToString(),
                        IsActive = Convert.ToBoolean(r["is_active"]),
                        CreatedAt = Convert.ToDateTime(r["created_at"])
                    });
                }
            }
            return list;
        }
        public bool SetActiveStatus(long id, bool isActive)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = "UPDATE Suppliers SET is_active = @active WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@active", isActive);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        // ✅ Optional: check if supplier name already exists
        public bool CheckIfExists(string name)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Suppliers WHERE LOWER(name) = LOWER(@name)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }
}
