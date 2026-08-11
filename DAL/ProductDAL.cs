using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.DAL
{
    public class ProductDAL
    {
        public List<Product> GetAll()
        {
            List<Product> list = new List<Product>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT p.*, c.name AS CategoryName, u.name AS UnitName
                    FROM Products p
                    LEFT JOIN Categories c ON p.category_id = c.id
                    LEFT JOIN Units u ON p.unit_id = u.id
                    WHERE p.deleted = 0
                    ORDER BY p.name", con);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(Map(r));
                }
            }
            return list;
        }

        public Product GetById(long id)
        {
            Product p = null;
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT p.*, c.name AS CategoryName, u.name AS UnitName
                    FROM Products p
                    LEFT JOIN Categories c ON p.category_id = c.id
                    LEFT JOIN Units u ON p.unit_id = u.id
                    WHERE p.id=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read()) p = Map(r);
            }
            return p;
        }

        public bool Add(Product p)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Products 
                    (product_code, name, category_id, brand, description, barcode, default_purchase_price, 
                    default_sales_price, opening_stock, current_stock, minimum_stock, unit_id, 
                    is_service_product, is_active, created_by, created_at)
                    VALUES (@code,@name,@cat,@brand,@desc,@bar,@pp,@sp,@open,@curr,@min,@unit,@serv,@active,@user,GETUTCDATE())", con);

                cmd.Parameters.AddWithValue("@code", p.ProductCode);
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@cat", p.CategoryId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@brand", p.Brand ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@desc", p.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@bar", p.Barcode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@pp", p.DefaultPurchasePrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@sp", p.DefaultSalesPrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@open", p.OpeningStock);
                cmd.Parameters.AddWithValue("@curr", p.CurrentStock);
                cmd.Parameters.AddWithValue("@min", p.MinimumStock);
                cmd.Parameters.AddWithValue("@unit", p.UnitId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@serv", p.IsServiceProduct);
                cmd.Parameters.AddWithValue("@active", p.IsActive);
                cmd.Parameters.AddWithValue("@user", p.CreatedBy ?? (object)DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(Product p)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Products SET 
                    name=@name, category_id=@cat, brand=@brand, description=@desc, barcode=@bar,
                    default_purchase_price=@pp, default_sales_price=@sp, minimum_stock=@min,
                    unit_id=@unit, is_service_product=@serv, is_active=@active,
                    updated_by=@upd, updated_at=GETUTCDATE()
                    WHERE id=@id", con);

                cmd.Parameters.AddWithValue("@id", p.Id);
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@cat", p.CategoryId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@brand", p.Brand ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@desc", p.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@bar", p.Barcode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@pp", p.DefaultPurchasePrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@sp", p.DefaultSalesPrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@min", p.MinimumStock);
                cmd.Parameters.AddWithValue("@unit", p.UnitId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@serv", p.IsServiceProduct);
                cmd.Parameters.AddWithValue("@active", p.IsActive);
                cmd.Parameters.AddWithValue("@upd", p.UpdatedBy ?? (object)DBNull.Value);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(long id, long userId)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("UPDATE Products SET deleted=1, deleted_by=@user WHERE id=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@user", userId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public List<Product> Search(string keyword)
        {
            var list = new List<Product>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT p.*, c.name AS CategoryName, u.name AS UnitName
                    FROM Products p
                    LEFT JOIN Categories c ON p.category_id = c.id
                    LEFT JOIN Units u ON p.unit_id = u.id
                    WHERE p.deleted=0 AND 
                          (p.name LIKE @k OR p.product_code LIKE @k OR p.brand LIKE @k OR p.barcode LIKE @k)", con);
                cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");
                con.Open();
                var r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(Map(r));
            }
            return list;
        }
        private Product Map(SqlDataReader r)
        {
            return new Product
            {
                Id = Convert.ToInt64(r["id"]),
                ProductCode = r["product_code"].ToString(),
                Name = r["name"].ToString(),
                CategoryId = r["category_id"] == DBNull.Value ? null : (long?)Convert.ToInt64(r["category_id"]),
                CategoryName = r["CategoryName"].ToString(),
                Brand = r["brand"].ToString(),
                Description = r["description"].ToString(),
                Barcode = r["barcode"].ToString(),
                DefaultPurchasePrice = r["default_purchase_price"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["default_purchase_price"]),
                DefaultSalesPrice = r["default_sales_price"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["default_sales_price"]),
                OpeningStock = Convert.ToDecimal(r["opening_stock"]),
                CurrentStock = Convert.ToDecimal(r["current_stock"]),
                MinimumStock = Convert.ToDecimal(r["minimum_stock"]),
                UnitId = r["unit_id"] == DBNull.Value ? null : (long?)Convert.ToInt64(r["unit_id"]),
                UnitName = r["UnitName"].ToString(),
                IsServiceProduct = Convert.ToBoolean(r["is_service_product"]),
                IsActive = Convert.ToBoolean(r["is_active"]),
                CreatedAt = Convert.ToDateTime(r["created_at"])
            };
        }
    }
}
