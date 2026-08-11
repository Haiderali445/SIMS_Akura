using SIMS_Akura.BLL;
using SIMS_Akura.Models;
using SIMS_Akura.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS_Akura.UI
{
    public partial class Products : System.Web.UI.Page
    {
        private readonly ProductBLL _productService = new ProductBLL();
        private readonly CategoryBLL _categoryService = new CategoryBLL();
        private readonly UnitBLL _unitService = new UnitBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCategories();
                BindUnits();
                BindGrid();
            }
        }

        private void BindGrid(string keyword = "")
        {
            var list = string.IsNullOrWhiteSpace(keyword)
                ? _productService.GetAll()
                : _productService.Search(keyword);

            var data = list.Select((p, i) => new
            {
                RowNo = i + 1,
                p.Id,
                p.ProductCode,
                p.Name,
                p.Barcode,
                p.Brand,
                CategoryName = p.CategoryName ?? "",
                p.CurrentStock,
                Description = p.Description?.Length > 80 ? p.Description.Substring(0, 80) + "..." : p.Description
            }).ToList();

            gvProducts.DataSource = data;
            gvProducts.DataBind();
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProducts.PageIndex = e.NewPageIndex;
            BindGrid(txtSearch.Text.Trim());
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlFilterCategory.SelectedIndex = 0;
            BindGrid();
        }

        private void BindCategories()
        {
            ddlCategory.Items.Clear();
            ddlFilterCategory.Items.Clear();
            ddlCategory.Items.Add(new ListItem("-- Select --", ""));
            ddlFilterCategory.Items.Add(new ListItem("All Categories", ""));
            var cats = _categoryService.GetAll();
            foreach (var c in cats)
            {
                ddlCategory.Items.Add(new ListItem(c.Name, c.Id.ToString()));
                ddlFilterCategory.Items.Add(new ListItem(c.Name, c.Id.ToString()));
            }
        }

        private void BindUnits()
        {
            ddlUnit.Items.Clear();
            ddlUnit.Items.Add(new ListItem("-- Select --", ""));
            var units = _unitService.GetAll();
            foreach (var u in units)
                ddlUnit.Items.Add(new ListItem(u.Name, u.Id.ToString()));
        }

        // ✅ Simplified: Random but unique product code generator with single prefix
        private string GenerateProductCode()
        {
            var allProducts = _productService.GetAll();
            var existingCodes = new HashSet<string>(allProducts.Select(p => p.ProductCode));

            string newCode;
            var rand = new Random();

            do
            {
                string randomPart = rand.Next(1000000000, int.MaxValue).ToString(); // 10-digit random
                newCode = $"PRD-{randomPart}";
            }
            while (existingCodes.Contains(newCode));

            return newCode;
        }

        protected void btnNewProduct_Click(object sender, EventArgs e)
        {
            ClearForm();

            string productCode = GenerateProductCode();
            txtProductCode.Text = productCode;

            string barcode = CodeGenerator.GenerateBarcode(productCode);
            txtBarcode.Text = barcode;

            imgBarcode.ImageUrl = BarcodeImageGenerator.GenerateBarcodeImageUrl(barcode);

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNew = string.IsNullOrEmpty(hfProductId.Value) || hfProductId.Value == "0";

                // Always regenerate code + barcode for new products
                string productCode = isNew ? GenerateProductCode() : txtProductCode.Text;
                string barcode = isNew ? CodeGenerator.GenerateBarcode(productCode) : txtBarcode.Text;

                Product p = new Product
                {
                    Id = isNew ? 0 : Convert.ToInt64(hfProductId.Value),
                    ProductCode = productCode,
                    Name = txtName.Text.Trim(),
                    Barcode = barcode,
                    CategoryId = string.IsNullOrEmpty(ddlCategory.SelectedValue) ? (long?)null : Convert.ToInt64(ddlCategory.SelectedValue),
                    Brand = txtBrand.Text.Trim(),
                    UnitId = string.IsNullOrEmpty(ddlUnit.SelectedValue) ? (long?)null : Convert.ToInt64(ddlUnit.SelectedValue),
                    DefaultPurchasePrice = decimal.TryParse(txtPurchase.Text, out var pp) ? pp : 0,
                    DefaultSalesPrice = decimal.TryParse(txtSales.Text, out var sp) ? sp : 0,
                    MinimumStock = decimal.TryParse(txtMinStock.Text, out var ms) ? ms : 0,
                    Description = txtDescription.Text.Trim(),
                    CreatedBy = 1
                };

                var result = isNew ? _productService.Add(p) : _productService.Update(p);

                ShowAlert(result.Message, result.Success ? "success" : "danger");

                if (result.Success)
                {
                    BindGrid();
                    ScriptManager.RegisterStartupScript(this, GetType(), "HideModal", "hideModal();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error: " + ex.Message, "danger");
            }
        }

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditProduct")
            {
                long id = Convert.ToInt64(e.CommandArgument);
                LoadProduct(id);
            }
            else if (e.CommandName == "DeleteProduct")
            {
                long id = Convert.ToInt64(e.CommandArgument);
                var result = _productService.Delete(id, 1);
                ShowAlert(result.Message, result.Success ? "success" : "danger");
                BindGrid();
            }
        }

        private void LoadProduct(long id)
        {
            var p = _productService.GetById(id);
            if (p == null) { ShowAlert("Product not found.", "warning"); return; }

            hfProductId.Value = p.Id.ToString();
            txtProductCode.Text = p.ProductCode;
            txtName.Text = p.Name;
            txtBarcode.Text = p.Barcode;
            ddlCategory.SelectedValue = p.CategoryId?.ToString() ?? "";
            ddlUnit.SelectedValue = p.UnitId?.ToString() ?? "";
            txtBrand.Text = p.Brand;
            txtPurchase.Text = p.DefaultPurchasePrice?.ToString("0.##") ?? "";
            txtSales.Text = p.DefaultSalesPrice?.ToString("0.##") ?? "";
            txtMinStock.Text = p.MinimumStock.ToString("0.##");
            txtDescription.Text = p.Description;

            imgBarcode.ImageUrl = BarcodeImageGenerator.GenerateBarcodeImageUrl(p.Barcode);

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            BindGrid(keyword);
        }

        private void ClearForm()
        {
            hfProductId.Value = "";
            txtName.Text = txtBrand.Text = txtBarcode.Text = txtDescription.Text = "";
            txtPurchase.Text = txtSales.Text = txtMinStock.Text = "";
            ddlCategory.SelectedIndex = 0;
            ddlUnit.SelectedIndex = 0;
            chkManageStock.Checked = false;
            imgBarcode.ImageUrl = "";
        }

        private void ShowAlert(string message, string type)
        {
            litAlert.Text = $"<div class='alert alert-{type} alert-dismissible fade show' role='alert'>{message}<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
        }
    }
}
