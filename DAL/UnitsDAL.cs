using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.DAL
{
    public class UnitsDAL
    {
        public List<Unit> GetAll()
        {
            List<Unit> list = new List<Unit>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Units ORDER BY name", con);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new Unit
                    {
                        Id = Convert.ToInt64(r["id"]),
                        Code = r["code"].ToString(),
                        Name = r["name"].ToString(),
                        CreatedAt = Convert.ToDateTime(r["created_at"])
                    });
                }
            }
            return list;
        }

        public bool Add(Unit u)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Units (code, name, created_at) VALUES (@code,@name,GETUTCDATE())", con);
                cmd.Parameters.AddWithValue("@code", u.Code);
                cmd.Parameters.AddWithValue("@name", u.Name);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        //  Update unit
        public bool Update(Unit u)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"UPDATE Units SET code = @code, name = @name WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", u.Id);
                cmd.Parameters.AddWithValue("@code", u.Code);
                cmd.Parameters.AddWithValue("@name", u.Name);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //  Delete unit
        public bool Delete(long id)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"DELETE FROM Units WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

    }
}
