using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.DAL
{
    public class UsersDAL
    {
        public List<User> GetAll()
        {
            List<User> list = new List<User>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Users ORDER BY name", con);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new User
                    {
                        Id = Convert.ToInt64(r["id"]),
                        UserCode = r["user_code"].ToString(),
                        Name = r["name"].ToString(),
                        Email = r["email"].ToString(),
                        Role = r["role"].ToString(),
                        IsActive = Convert.ToBoolean(r["is_active"]),
                        CreatedAt = Convert.ToDateTime(r["created_at"])
                    });
                }
            }
            return list;
        }

        public bool Add(User u)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Users (user_code, name, email, password, role, is_active, created_at) VALUES (@code,@name,@email,@pass,@role,@active,GETUTCDATE())", con);
                cmd.Parameters.AddWithValue("@code", u.UserCode);
                cmd.Parameters.AddWithValue("@name", u.Name);
                cmd.Parameters.AddWithValue("@email", u.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@pass", u.Password ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@role", u.Role);
                cmd.Parameters.AddWithValue("@active", u.IsActive);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
