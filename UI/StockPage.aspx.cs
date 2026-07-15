using SIMS_Akura.BLL;
using SIMS_Akura.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS_Akura.UI
{
    public partial class StockPage : Page
    {
        private readonly StockBLL _stockBLL = new StockBLL();
        private readonly SupplierBLL _supplierBLL = new SupplierBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSuppliers();
                BindStock();
            }
        }

        private void LoadSuppliers()
        {
            var suppliers = _supplierBLL.GetAll();
            ddlSupplier.DataSource = suppliers;
            ddlSupplier.DataTextField = "Name";
            ddlSupplier.DataValueField = "Id";
            ddlSupplier.DataBind();
            ddlSupplier.Items.Insert(0, new ListItem("-- All Suppliers --", "0"));
        }

        private void BindStock()
        {
            long? supplierId = ddlSupplier.SelectedValue == "0" ? (long?)null : Convert.ToInt64(ddlSupplier.SelectedValue);
            var products = _stockBLL.GetOverview();

            if (supplierId.HasValue)
                products = products.FindAll(p => p.SupplierId == supplierId.Value);

            gvStock.DataSource = products;
            gvStock.DataBind();

            litAlert.Text = products.Count == 0 ? "<div class='alert alert-warning'>No products found.</div>" : "";
        }

        protected void ddlSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindStock();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            ddlSupplier.SelectedIndex = 0;
            BindStock();
        }

        protected void chkActive_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            long productId = Convert.ToInt64(chk.Attributes["data-productid"]);
            _stockBLL.SetProductActive(productId, chk.Checked, 1); // 1 = userId
            BindStock();
        }


        protected void gvStock_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewBatches")
            {
                long productId = Convert.ToInt64(e.CommandArgument);
                LoadBatches(productId);
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalBatches').modal('show');", true);
            }
        }

        private void LoadBatches(long productId)
        {
            var batches = _stockBLL.GetBatches(productId);
            if (batches == null || batches.Count == 0)
            {
                batchDetails.InnerHtml = "<div class='alert alert-warning'>No batches found for this product.</div>";
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='table table-sm table-striped'><thead><tr>");
            sb.Append("<th>Batch</th><th>Qty</th><th>Available</th><th>Unit Cost</th><th>Expires</th></tr></thead><tbody>");

            foreach (var b in batches)
            {
                sb.Append($"<tr><td>{b.BatchCode}</td><td>{b.Qty:N2}</td><td>{b.AvailableQty:N2}</td><td>{b.UnitCost:N2}</td><td>{(b.ExpiresAt.HasValue ? b.ExpiresAt.Value.ToString("yyyy-MM-dd") : "-")}</td></tr>");
            }

            sb.Append("</tbody></table>");
            batchDetails.InnerHtml = sb.ToString();
        }
    }
}
