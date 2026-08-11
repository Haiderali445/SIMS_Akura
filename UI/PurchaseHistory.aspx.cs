using SIMS_Akura.BLL;
using SIMS_Akura.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS_Akura.UI
{
    public partial class PurchaseHistory : Page
    {
        private readonly PurchaseBLL _purchaseBLL = new PurchaseBLL();
        private readonly SupplierBLL _supplierBLL = new SupplierBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSuppliers();
                BindHistory();
            }
            else
            {
                // Keep modal open after postback if needed
                if (ViewState["CurrentInvoiceId"] != null)
                {
                    long invoiceId = (long)ViewState["CurrentInvoiceId"];
                    LoadInvoiceDetails(invoiceId);
                    ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalInvoice').modal('show');", true);
                }
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                var suppliers = _supplierBLL.GetAll();
                ddlSupplier.DataSource = suppliers;
                ddlSupplier.DataTextField = "Name";
                ddlSupplier.DataValueField = "Id";
                ddlSupplier.DataBind();
                ddlSupplier.Items.Insert(0, new ListItem("-- All Suppliers --", "0"));
            }
            catch (Exception ex)
            {
                ShowAlert("Failed to load suppliers: " + ex.Message, "danger");
            }
        }

        private void BindHistory()
        {
            try
            {
                DateTime? fromDate = string.IsNullOrWhiteSpace(txtFromDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtFromDate.Text);
                DateTime? toDate = string.IsNullOrWhiteSpace(txtToDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtToDate.Text);
                long? supplierId = ddlSupplier.SelectedValue == "0" ? (long?)null : Convert.ToInt64(ddlSupplier.SelectedValue);

                var list = _purchaseBLL.GetPurchaseHistory(fromDate, toDate, supplierId);
                gvHistory.DataSource = list;
                gvHistory.DataBind();

                if (list.Count == 0)
                    ShowAlert("No purchases found for the selected filters.", "warning");
                else
                    litAlert.Text = "";
            }
            catch (Exception ex)
            {
                ShowAlert("Failed to retrieve purchase history: " + ex.Message, "danger");
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindHistory();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = "";
            txtToDate.Text = "";
            ddlSupplier.SelectedIndex = 0;
            BindHistory();
        }

        protected void gvHistory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewInvoice")
            {
                long invoiceId = Convert.ToInt64(e.CommandArgument);
                ViewState["CurrentInvoiceId"] = invoiceId;
                LoadInvoiceDetails(invoiceId);
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalInvoice').modal('show');", true);
            }
        }

        private void LoadInvoiceDetails(long invoiceId)
        {
            try
            {
                var items = _purchaseBLL.GetInvoiceItems(invoiceId);
                if (items == null || items.Count == 0)
                {
                    invoiceDetails.InnerHtml = "<div class='alert alert-warning'>No items found for this invoice.</div>";
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<table class='table table-bordered'><thead><tr>");
                sb.Append("<th>Product</th><th>Qty</th><th>Rate</th><th>Total</th></tr></thead><tbody>");

                foreach (var item in items)
                {
                    decimal total = item.Qty * item.Rate;
                    sb.Append($"<tr><td>{item.ProductName}</td><td>{item.Qty:N2}</td><td>{item.Rate:N2}</td><td>{total:N2}</td></tr>");
                }

                sb.Append("</tbody></table>");

                var batches = _purchaseBLL.GetBatchesByInvoice(invoiceId);
                if (batches != null && batches.Count > 0)
                {
                    sb.Append("<h6 class='mt-3'>Stock Batches</h6>");
                    sb.Append("<table class='table table-sm table-striped'><thead><tr>");
                    sb.Append("<th>Batch</th><th>Product</th><th>Qty</th><th>Available</th><th>Cost</th><th>Expires</th></tr></thead><tbody>");

                    foreach (var b in batches)
                    {
                        sb.Append("<tr>");
                        sb.Append($"<td>{b.BatchCode ?? "-"}</td>");
                        sb.Append($"<td>{b.ProductName}</td>");
                        sb.Append($"<td>{b.Qty:N2}</td>");
                        sb.Append($"<td>{b.AvailableQty:N2}</td>");
                        sb.Append($"<td>{b.UnitCost:N2}</td>");
                        sb.Append($"<td>{(b.ExpiresAt.HasValue ? b.ExpiresAt.Value.ToString("yyyy-MM-dd") : "-")}</td>");
                        sb.Append("</tr>");
                    }

                    sb.Append("</tbody></table>");
                }

                invoiceDetails.InnerHtml = sb.ToString();
            }
            catch (Exception ex)
            {
                invoiceDetails.InnerHtml = $"<div class='alert alert-danger'>Error loading invoice details: {ex.Message}</div>";
            }
        }

        private void ShowAlert(string msg, string type)
        {
            litAlert.Text = $"<div class='alert alert-{type} alert-dismissible fade show' role='alert'>{msg}" +
                            $"<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
        }
    }
}
