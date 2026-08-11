using SIMS_Akura.DAL;
using SIMS_Akura.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SIMS_Akura.BLL
{
    public class PurchaseBLL
    {
        private readonly PurchaseDAL _dal;

        public PurchaseBLL()
        {
            _dal = new PurchaseDAL();
        }

        // Create a new purchase invoice along with its items
        public long CreatePurchase(PurchaseInvoice invoice, List<PurchaseItem> items)
        {
            if (invoice == null) throw new ArgumentNullException("Invoice cannot be null.");
            if (items == null || items.Count == 0) throw new ArgumentException("At least one purchase item is required.");
            if (invoice.AccountId <= 0) throw new ArgumentException("Supplier must be selected.");
            if (invoice.GrandTotal <= 0) throw new ArgumentException("Grand total must be greater than zero.");

            try
            {
                // Create invoice header
                long invoiceId = _dal.AddPurchaseInvoice(invoice);
                if (invoiceId <= 0)
                    throw new ApplicationException("Failed to create invoice header.");

                // Add each purchase item
                foreach (var item in items)
                {
                    if (item.ProductId <= 0) throw new ArgumentException("Invalid product selected.");
                    if (item.Qty <= 0) throw new ArgumentException($"Quantity must be positive for product {item.ProductId}.");
                    if (item.Rate <= 0) throw new ArgumentException($"Rate must be positive for product {item.ProductId}.");

                    item.InvoiceId = invoiceId;
                    item.CreatedBy = invoice.CreatedBy;

                    _dal.AddPurchaseItem(item); // DAL handles batch + stock movement
                }

                // Update supplier account balance
                var accountBll = new AccountBLL();
                accountBll.UpdateBalance(invoice.AccountId, invoice.GrandTotal);

                // Record ledger transaction
                accountBll.AddTransaction(new AccountTransaction
                {
                    AccountId = invoice.AccountId,
                    TransactionCode = "TRX-" + invoice.InvoiceCode,
                    TransactionType = "Purchase",
                    Amount = invoice.GrandTotal,
                    ReferenceTable = "Invoices",
                    ReferenceId = invoiceId,
                    Note = "Purchase invoice " + invoice.InvoiceCode,
                    CreatedBy = invoice.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                });

                return invoiceId;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database error while creating purchase invoice.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error during purchase creation.", ex);
            }
        }

        // Retrieve purchase history with optional filters
        public List<PurchaseView> GetPurchaseHistory(DateTime? fromDate, DateTime? toDate, long? supplierId)
        {
            try
            {
                return _dal.GetPurchaseHistory(fromDate, toDate, supplierId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve purchase history.", ex);
            }
        }

        // Retrieve items for a specific invoice
        public List<PurchaseItem> GetInvoiceItems(long invoiceId)
        {
            if (invoiceId <= 0)
                throw new ArgumentException("Invalid invoice ID.");

            try
            {
                return _dal.GetInvoiceItems(invoiceId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve invoice items.", ex);
            }
        }

        // Retrieve batches linked to a specific invoice
        public List<StockBatch> GetBatchesByInvoice(long invoiceId)
        {
            if (invoiceId <= 0)
                throw new ArgumentException("Invalid invoice ID.");

            try
            {
                return _dal.GetBatchesByInvoice(invoiceId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve batches for invoice.", ex);
            }
        }

        // Process a supplier return for a batch
        public void ProcessSupplierReturn(long batchId, decimal returnQty, string reasonNote, long createdBy)
        {
            if (batchId <= 0)
                throw new ArgumentException("Invalid batch ID.");

            if (returnQty <= 0)
                throw new ArgumentException("Return quantity must be positive.");

            if (string.IsNullOrWhiteSpace(reasonNote))
                throw new ArgumentException("Reason note is required.");

            try
            {
                _dal.ProcessSupplierReturn(batchId, returnQty, reasonNote, createdBy);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to process supplier return.", ex);
            }
        }
    }
}
