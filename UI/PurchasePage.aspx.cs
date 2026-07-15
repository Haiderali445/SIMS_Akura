using SIMS_Akura.BLL;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace SIMS_Akura.UI
{
    public partial class PurchasePage : Page
    {
        private readonly PurchaseBLL _purchaseBLL = new PurchaseBLL();
        private readonly SupplierBLL _supplierBLL = new SupplierBLL();
        private readonly ProductBLL _productBLL = new ProductBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadLookups();
                ResetPage();
            }
        }

        #region Lookups & JS Data

        private void LoadLookups()
        {
            // Suppliers dropdown
            var suppliers = _supplierBLL.GetAll();
            ddlSupplier.DataSource = suppliers;
            ddlSupplier.DataTextField = "Name";
            ddlSupplier.DataValueField = "Id";
            ddlSupplier.DataBind();
            ddlSupplier.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Supplier --", "0"));

            // Products -> send to JS safely
            var prods = _productBLL.GetAll();
            var productDtos = prods.ConvertAll(p => new
            {
                Id = p.Id,
                Name = p.Name,
                DefaultPurchasePrice = p.DefaultPurchasePrice ?? 0,
                ProductCode = p.ProductCode
            });

            var serializer = new JavaScriptSerializer();
            string prodJson = serializer.Serialize(productDtos);

            string escaped = prodJson
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\r", "")
                .Replace("\n", "");

            litData.Text = $@"
<script>
document.addEventListener('DOMContentLoaded', function() {{
    try {{
        const prodArr = JSON.parse('{escaped}');
        loadClientProductsAndInit(prodArr);
    }} catch (e) {{
        console.error('Product JSON parse error:', e);
    }}
}});
</script>";
        }

        #endregion

        #region Page Initialization / Reset

        private void ResetPage()
        {
            // Generate unique invoice code using timestamp
            txtInvoiceCode.Text = "PUR-" + DateTime.UtcNow.Ticks.ToString();
            txtInvoiceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            hfItemsJson.Value = string.Empty;

            // Clear client-side items
            Page.ClientScript.RegisterStartupScript(this.GetType(), "clearClient", "clientItems=[]; renderItems();", true);
        }

        #endregion

        #region Button Handlers

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var json = hfItemsJson.Value;
                if (string.IsNullOrWhiteSpace(json))
                    throw new Exception("Please add at least one item.");

                var serializer = new JavaScriptSerializer();
                var items = serializer.Deserialize<List<PurchaseItem>>(json);
                if (items == null || items.Count == 0)
                    throw new Exception("No items parsed from client.");

                if (ddlSupplier.SelectedValue == "0")
                    throw new Exception("Select a supplier.");

                // Validate supplier/account
                var supplier = _supplierBLL.GetById(Convert.ToInt64(ddlSupplier.SelectedValue));
                if (supplier == null || !supplier.AccountId.HasValue)
                    throw new Exception("Selected supplier has no linked account.");

                decimal discount = 0, fare = 0;
                decimal.TryParse(Request.Form["txtDiscount"], out discount);
                decimal.TryParse(Request.Form["txtFare"], out fare);

                var invoice = new PurchaseInvoice
                {
                    InvoiceCode = txtInvoiceCode.Text,
                    AccountId = supplier.AccountId.Value,
                    TotalDiscount = discount,
                    Fare = fare,
                    ShopId = 1,
                    GrandTotal = ComputeGrandTotal(items, discount, fare),
                    CreatedBy = GetUserId(),
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var it in items)
                    it.CreatedBy = invoice.CreatedBy;

                // Save invoice
                long invoiceId = _purchaseBLL.CreatePurchase(invoice, items);

                ShowAlert($"✅ Purchase saved successfully (InvoiceId: {invoiceId})", "success");

                // Reset page for next invoice
                ResetPage();
            }
            catch (Exception ex)
            {
                ShowAlert($"❌ Error: {ex.Message}", "danger");
            }
        }

        #endregion

        #region Helpers

        private decimal ComputeGrandTotal(List<PurchaseItem> items, decimal discount, decimal fare)
        {
            decimal subtotal = 0;
            foreach (var it in items) subtotal += it.Total;
            return subtotal - discount + fare;
        }

        private long GetUserId()
        {
            return Session["UserId"] != null ? Convert.ToInt64(Session["UserId"]) : 1;
        }

        private void ShowAlert(string msg, string type)
        {
            litAlert.Text = $"<div class='alert alert-{type} alert-dismissible fade show' role='alert'>{msg}<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
        }

        #endregion
    }
}
