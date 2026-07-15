using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.DAL
{
    public class AccountDAL
    {
        public List<Account> GetAll()
        {
            var list = new List<Account>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = "SELECT * FROM Accounts ORDER BY name";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(MapAccount(r));
                }
            }
            return list;
        }

        public Account GetById(long id)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = "SELECT * FROM Accounts WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    return MapAccount(r);
                }
            }
            return null;
        }

        // ✅ NEW: Get account by code
        public Account GetByCode(string code)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = "SELECT * FROM Accounts WHERE account_code = @code";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@code", code);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    return MapAccount(r);
                }
            }
            return null;
        }

        public bool Insert(Account acc)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    INSERT INTO Accounts (account_code, name, account_type, phone, email, address, current_balance, is_active, created_by, created_at)
                    VALUES (@code, @name, @type, @phone, @email, @address, @balance, @active, @createdBy, SYSUTCDATETIME())";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@code", acc.AccountCode);
                cmd.Parameters.AddWithValue("@name", acc.Name);
                cmd.Parameters.AddWithValue("@type", acc.AccountType);
                cmd.Parameters.AddWithValue("@phone", acc.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", acc.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@address", acc.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@balance", acc.CurrentBalance);
                cmd.Parameters.AddWithValue("@active", acc.IsActive);
                cmd.Parameters.AddWithValue("@createdBy", acc.CreatedBy.HasValue ? (object)acc.CreatedBy.Value : DBNull.Value);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateBalance(long accountId, decimal newBalance)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = "UPDATE Accounts SET current_balance = @balance WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", accountId);
                cmd.Parameters.AddWithValue("@balance", newBalance);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SetActiveStatus(long accountId, bool isActive)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = "UPDATE Accounts SET is_active = @active WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", accountId);
                cmd.Parameters.AddWithValue("@active", isActive);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<Account> Search(string keyword = "", string accountType = "", bool? isActive = null)
        {
            var list = new List<Account>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                var query = @"
            SELECT * FROM Accounts
            WHERE 
                (@keyword = '' OR 
                 name LIKE '%' + @keyword + '%' OR 
                 account_code LIKE '%' + @keyword + '%' OR 
                 phone LIKE '%' + @keyword + '%' OR 
                 email LIKE '%' + @keyword + '%')
                AND (@type = '' OR account_type = @type)
                AND (@active IS NULL OR is_active = @active)
            ORDER BY name";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@keyword", keyword ?? "");
                cmd.Parameters.AddWithValue("@type", accountType ?? "");
                cmd.Parameters.AddWithValue("@active", isActive.HasValue ? (object)isActive.Value : DBNull.Value);

                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(MapAccount(r));
                }
            }
            return list;
        }

        public bool Delete(long id)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = "DELETE FROM Accounts WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(Account acc)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    UPDATE Accounts SET
                        name = @name,
                        account_type = @type,
                        phone = @phone,
                        email = @email,
                        address = @address,
                        current_balance = @balance,
                        is_active = @active
                    WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", acc.Id);
                cmd.Parameters.AddWithValue("@name", acc.Name);
                cmd.Parameters.AddWithValue("@type", acc.AccountType);
                cmd.Parameters.AddWithValue("@phone", acc.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", acc.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@address", acc.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@balance", acc.CurrentBalance);
                cmd.Parameters.AddWithValue("@active", acc.IsActive);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // 🔹 Increment or decrement current_balance by delta (positive = increase, negative = decrease)
        public bool AdjustBalance(long accountId, decimal delta)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = "UPDATE Accounts SET current_balance = current_balance + @delta WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", accountId);
                cmd.Parameters.AddWithValue("@delta", delta);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 🔹 Record a transaction in AccountTransactions table
        public bool AddTransaction(AccountTransaction trx)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
            INSERT INTO AccountTransactions 
                (transaction_code, account_id, transaction_type, amount, reference_table, reference_id, note, created_by, created_at)
            VALUES 
                (@code, @accountId, @type, @amount, @refTable, @refId, @note, @createdBy, SYSUTCDATETIME())";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@code", trx.TransactionCode);
                cmd.Parameters.AddWithValue("@accountId", trx.AccountId);
                cmd.Parameters.AddWithValue("@type", trx.TransactionType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@amount", trx.Amount);
                cmd.Parameters.AddWithValue("@refTable", trx.ReferenceTable ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@refId", trx.ReferenceId.HasValue ? (object)trx.ReferenceId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@note", trx.Note ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@createdBy", trx.CreatedBy.HasValue ? (object)trx.CreatedBy.Value : DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        // 🔧 Helper: map SqlDataReader to Account object
        private Account MapAccount(SqlDataReader r)
        {
            return new Account
            {
                Id = Convert.ToInt64(r["id"]),
                AccountCode = r["account_code"].ToString(),
                Name = r["name"].ToString(),
                AccountType = r["account_type"].ToString(),
                Phone = r["phone"].ToString(),
                Email = r["email"].ToString(),
                Address = r["address"].ToString(),
                CurrentBalance = Convert.ToDecimal(r["current_balance"]),
                IsActive = Convert.ToBoolean(r["is_active"]),
                CreatedAt = Convert.ToDateTime(r["created_at"]),
                CreatedBy = r["created_by"] == DBNull.Value ? (long?)null : Convert.ToInt64(r["created_by"])
            };
        }
    }
}
