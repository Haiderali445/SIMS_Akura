using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;

namespace SIMS_Akura.DAL
{
    public class PurchaseDAL
    {
        // 🔹 Create Purchase Invoice
        public long AddPurchaseInvoice(PurchaseInvoice invoice)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@InvoiceCode", invoice.InvoiceCode ?? string.Empty),
                new SqlParameter("@AccountId", invoice.AccountId),
                new SqlParameter("@GrandTotal", invoice.GrandTotal),
                new SqlParameter("@TotalDiscount", invoice.TotalDiscount),
                new SqlParameter("@CreatedBy", invoice.CreatedBy),
                new SqlParameter("@ShopId", invoice.ShopId > 0 ? invoice.ShopId : 1),
                new SqlParameter("@Fare", invoice.Fare)
            };

            DataTable dt = DBConnection.ExecuteQuery("sp_AddPurchaseInvoiceWithBatch", parameters);
            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("InvoiceId"))
                return Convert.ToInt64(dt.Rows[0]["InvoiceId"]);

            if (dt != null && dt.Rows.Count > 0)
                return Convert.ToInt64(dt.Rows[0][0]);

            throw new ApplicationException("No InvoiceId returned from sp_AddPurchaseInvoiceWithBatch");
        }

        // 🔹 Get Purchase History
        public List<PurchaseView> GetPurchaseHistory(DateTime? fromDate, DateTime? toDate, long? supplierId)
        {
            var result = new List<PurchaseView>();

            SqlParameter[] parameters =
            {
                new SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value),
                new SqlParameter("@ToDate", toDate ?? (object)DBNull.Value),
                new SqlParameter("@SupplierId", supplierId ?? (object)DBNull.Value)
            };

            DataTable dt = DBConnection.ExecuteQuery("sp_GetPurchaseHistory", parameters);

            foreach (DataRow row in dt.Rows)
            {
                result.Add(new PurchaseView
                {
                    InvoiceId = Convert.ToInt64(row["InvoiceId"]),
                    InvoiceCode = row["InvoiceCode"].ToString(),
                    SupplierName = row["SupplierName"].ToString(),
                    GrandTotal = Convert.ToDecimal(row["GrandTotal"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])

                });
            }

            return result;
        }

        // 🔹 Get Items for a Purchase Invoice
        public List<PurchaseItem> GetInvoiceItems(long invoiceId)
        {
            var result = new List<PurchaseItem>();

            SqlParameter[] parameters = { new SqlParameter("@InvoiceId", invoiceId) };
            DataTable dt = DBConnection.ExecuteQuery("sp_GetPurchaseItemsByInvoice", parameters);

            foreach (DataRow row in dt.Rows)
            {
                result.Add(new PurchaseItem
                {
                    InvoiceId = Convert.ToInt64(row["invoice_id"]), // match DB
                    ProductId = Convert.ToInt64(row["product_id"]),
                    ProductName = row["ProductName"].ToString(),
                    ProductCode = row["ProductCode"].ToString(),
                    Qty = Convert.ToDecimal(row["qty"]),
                    Rate = Convert.ToDecimal(row["rate"]),
                    BatchCode = row.Table.Columns.Contains("batch_code") ? row["batch_code"].ToString() : "-"
                });
            }

            return result;
        }

        // 🔹 Get Batches Linked to Invoice
        public List<StockBatch> GetBatchesByInvoice(long invoiceId)
        {
            var result = new List<StockBatch>();

            SqlParameter[] parameters = { new SqlParameter("@InvoiceId", invoiceId) };
            DataTable dt = DBConnection.ExecuteQuery("sp_GetBatchesByInvoice", parameters);

            foreach (DataRow row in dt.Rows)
            {
                result.Add(new StockBatch
                {
                    Id = Convert.ToInt64(row["id"]),
                    ProductId = Convert.ToInt64(row["product_id"]),
                    ProductName = row["ProductName"].ToString(),
                    ProductCode = row["ProductCode"].ToString(),
                    BatchCode = row.Table.Columns.Contains("batch_code") ? row["batch_code"].ToString() : "-",
                    Qty = Convert.ToDecimal(row["qty"]),
                    AvailableQty = Convert.ToDecimal(row["available_qty"]),
                    UnitCost = Convert.ToDecimal(row["unit_cost"]),
                    CreatedAt = Convert.ToDateTime(row["created_at"]),
                    ExpiresAt = row.Table.Columns.Contains("expires_at") && row["expires_at"] != DBNull.Value
                                ? (DateTime?)Convert.ToDateTime(row["expires_at"])
                                : null,
                    SupplierId = row.Table.Columns.Contains("account_id") ? (long?)Convert.ToInt64(row["account_id"]) : null,
                    SupplierName = row.Table.Columns.Contains("SupplierName") ? row["SupplierName"].ToString() : null,
                    PurchaseInvoiceId = invoiceId,
                    InvoiceCode = row.Table.Columns.Contains("invoice_code") ? row["invoice_code"].ToString() : null
                });
            }

            return result;
        }

        // 🔹 Add single item to purchase invoice
        public void AddPurchaseItem(PurchaseItem item)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@InvoiceId", item.InvoiceId),
                new SqlParameter("@ProductId", item.ProductId),
                new SqlParameter("@Qty", item.Qty),
                new SqlParameter("@Rate", item.Rate),
                new SqlParameter("@CreatedBy", item.CreatedBy),
                new SqlParameter("@BatchCode", item.BatchCode ?? string.Empty)
            };

            DBConnection.ExecuteNonQuery("sp_AddPurchaseItemBatch", parameters);
        }

        // 🔹 Process Supplier Return
        public void ProcessSupplierReturn(long batchId, decimal returnQty, string reasonNote, long createdBy)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@BatchId", batchId),
                new SqlParameter("@ReturnQty", returnQty),
                new SqlParameter("@ReasonNote", reasonNote),
                new SqlParameter("@CreatedBy", createdBy)
            };

            DBConnection.ExecuteNonQuery("sp_ProcessSupplierReturn", parameters);
        }
    }
}
