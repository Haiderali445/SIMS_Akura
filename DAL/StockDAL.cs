using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.DAL
{
    public class StockDAL
    {
        // 1️⃣ Stock overview
        public List<StockView> GetOverview()
        {
            var list = new List<StockView>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT p.id AS ProductId, p.product_code, p.name AS ProductName,
                           c.name AS CategoryName, u.name AS UnitName,
                           p.current_stock AS CurrentStock, p.minimum_stock,
                           p.default_purchase_price AS PurchasePrice,
                           p.default_sales_price AS SalesPrice,
                           p.is_service_product, p.is_active,
                           p.updated_at AS LastUpdated
                    FROM Products p
                    LEFT JOIN Categories c ON p.category_id = c.id
                    LEFT JOIN Units u ON p.unit_id = u.id
                    WHERE p.deleted = 0
                    ORDER BY p.name";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new StockView
                    {
                        ProductId = Convert.ToInt64(reader["ProductId"]),
                        ProductCode = reader["product_code"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        CategoryName = reader["CategoryName"]?.ToString(),
                        UnitName = reader["UnitName"]?.ToString(),
                        CurrentStock = Convert.ToDecimal(reader["CurrentStock"]),
                        MinimumStock = Convert.ToDecimal(reader["minimum_stock"]),
                        PurchasePrice = Convert.ToDecimal(reader["PurchasePrice"]),
                        SalesPrice = Convert.ToDecimal(reader["SalesPrice"]),
                        IsServiceProduct = Convert.ToBoolean(reader["is_service_product"]),
                        IsActive = Convert.ToBoolean(reader["is_active"]),
                        LastUpdated = reader["LastUpdated"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["LastUpdated"])
                    });
                }
            }
            return list;
        }

        // 2️⃣ Get batches for a product
        public List<StockBatch> GetBatchesByProduct(long productId)
        {
            var list = new List<StockBatch>();
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT id, product_id, batch_code, qty, available_qty, unit_cost,
                           created_at, expires_at, created_by
                    FROM StockBatches
                    WHERE product_id = @pid
                    ORDER BY created_at DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", productId);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new StockBatch
                    {
                        Id = Convert.ToInt64(reader["id"]),
                        ProductId = Convert.ToInt64(reader["product_id"]),
                        BatchCode = reader["batch_code"].ToString(),
                        Qty = Convert.ToDecimal(reader["qty"]),
                        AvailableQty = Convert.ToDecimal(reader["available_qty"]),
                        UnitCost = Convert.ToDecimal(reader["unit_cost"]),
                        CreatedAt = Convert.ToDateTime(reader["created_at"]),
                        ExpiresAt = reader["expires_at"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["expires_at"]),
                        CreatedBy = reader["created_by"] == DBNull.Value ? null : (long?)Convert.ToInt64(reader["created_by"])
                    });
                }
            }
            return list;
        }

        // 3️⃣ Adjust stock
        public bool AdjustStock(StockAdjustment adj)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
                    BEGIN TRAN;
                    UPDATE Products SET current_stock = current_stock + @qty WHERE id = @pid;
                    INSERT INTO StockMovements (product_id, change_qty, movement_type, reason_note, created_by, created_at)
                    VALUES (@pid, @qty, 'Adjustment', @note, @user, SYSUTCDATETIME());
                    COMMIT;";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", adj.ProductId);
                cmd.Parameters.AddWithValue("@qty", adj.QtyChange);
                cmd.Parameters.AddWithValue("@note", adj.ReasonNote ?? "");
                cmd.Parameters.AddWithValue("@user", adj.CreatedBy);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public bool SetProductActive(long productId, bool isActive, long userId)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                string query = @"
            UPDATE Products
            SET is_active = @active,
                updated_at = SYSUTCDATETIME(),
                updated_by = @user
            WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", productId);
                cmd.Parameters.AddWithValue("@active", isActive);
                cmd.Parameters.AddWithValue("@user", userId);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        // 4️⃣ Stock valuation
        public decimal GetValuationSummary()
        {
            decimal total = 0;
            var products = GetOverview();
            foreach (var p in products)
            {
                total += p.CurrentStock * p.PurchasePrice;
            }
            return total;
        }
    }
}
