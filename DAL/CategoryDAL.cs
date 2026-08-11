using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.DAL
{
    public class CategoryDAL
    {
        // ✅ Get all active and non-deleted categories
        public List<Category> GetAllCategories()
        {
            var list = new List<Category>();

            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"SELECT id, name, description, is_active, created_at
                                 FROM Categories 
                                 ORDER BY name";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new Category
                    {
                        Id = Convert.ToInt64(r["id"]),
                        Name = r["name"].ToString(),
                        Description = r["description"]?.ToString(),
                        IsActive = Convert.ToBoolean(r["is_active"]),
                        CreatedAt = Convert.ToDateTime(r["created_at"]),
                    });
                }
                con.Close();
            }

            return list;
        }

        // ✅ Get category by ID
        public Category GetById(long id)
        {
            Category category = null;

            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"SELECT id, name, description, is_active, created_at  
                                 FROM Categories 
                                 WHERE id = @id ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                SqlDataReader r = cmd.ExecuteReader();

                if (r.Read())
                {
                    category = new Category
                    {
                        Id = Convert.ToInt64(r["id"]),
                        Name = r["name"].ToString(),
                        Description = r["description"]?.ToString(),
                        IsActive = Convert.ToBoolean(r["is_active"]),
                        CreatedAt = Convert.ToDateTime(r["created_at"]),
                    };
                }
                con.Close();
            }

            return category;
        }

        // ✅ Check if category already exists (for duplicate validation)
        public bool CheckIfExists(string name)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"SELECT COUNT(*) FROM Categories 
                                 WHERE LOWER(name) = LOWER(@name)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;

            }

        }

        // ✅ Insert new category
        public bool InsertCategory(Category c)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    INSERT INTO Categories (name, description, is_active, created_at)
                    VALUES (@name, @desc, @active, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@name", c.Name);
                cmd.Parameters.AddWithValue("@desc", (object)c.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@active", c.IsActive);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ✅ Update category
        public bool UpdateCategory(Category c)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    UPDATE Categories SET 
                        name = @name,
                        description = @desc,
                        is_active = @active,
                   
                        updated_at = GETDATE()
                    WHERE id = @id AND deleted = 0";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@id", c.Id);
                cmd.Parameters.AddWithValue("@name", c.Name);
                cmd.Parameters.AddWithValue("@desc", (object)c.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@active", c.IsActive);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        // ✅ Soft delete category
        public bool hardDeleteCategory(long id)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    Delete Categories
                    WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //  Keyword search
        public List<Category> Search(string keyword)
        {
            var list = new List<Category>();

            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
            SELECT id, name, description, is_active, created_at
            FROM Categories
            WHERE name LIKE @kw OR description LIKE @kw
            ORDER BY name";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                con.Open();
                SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    list.Add(new Category
                    {
                        Id = Convert.ToInt64(r["id"]),
                        Name = r["name"].ToString(),
                        Description = r["description"]?.ToString(),
                        IsActive = Convert.ToBoolean(r["is_active"]),
                        CreatedAt = Convert.ToDateTime(r["created_at"])
                    });
                }
            }

            return list;
        }
    }
}
