using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SIMS_Akura.BLL;
using SIMS_Akura.Models;

namespace SIMS_Akura.UI
{
    public partial class SupplierPage : Page
    {
        private readonly SupplierBLL supplierBLL = new SupplierBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            gvSuppliers.DataSource = supplierBLL.GetAll();
            gvSuppliers.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            gvSuppliers.DataSource = supplierBLL.Search(keyword);
            gvSuppliers.DataBind();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            chkActiveOnly.Checked = false;
            BindGrid();
        }

        protected void gvSuppliers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSuppliers.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvSuppliers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            long id = Convert.ToInt64(e.CommandArgument);

            if (e.CommandName == "EditSupplier")
            {
                var s = supplierBLL.GetById(id);
                if (s != null)
                {
                    hfSupplierId.Value = s.Id.ToString();
                    txtName.Text = s.Name;
                    txtPhone.Text = s.Phone;
                    txtEmail.Text = s.Email;
                    txtAddress.Text = s.Address;
                    txtAccountCode.Text = s.AccountCode ?? "";
                    chkIsActive.Checked = s.IsActive;

                    ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "showModal();", true);
                }
            }
            else if (e.CommandName == "ToggleActive")
            {
                var s = supplierBLL.GetById(id);
                if (s != null)
                {
                    s.IsActive = !s.IsActive;
                    var result = supplierBLL.Update(s);
                    ShowAlert(result.Success, result.Message);
                    BindGrid();
                }
            }
            else if (e.CommandName == "DeleteSupplier")
            {
                var result = supplierBLL.Delete(id);
                ShowAlert(result.Success, result.Message);
                BindGrid();
            }
        }

        protected void btnNewSupplier_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "showModal();", true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var s = new Supplier
            {
                Name = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                IsActive = chkIsActive.Checked
            };

            bool isUpdate = long.TryParse(hfSupplierId.Value, out long id) && id > 0;
            (bool Success, string Message) result;

            if (isUpdate)
            {
                s.Id = id;
                result = supplierBLL.Update(s);
            }
            else
            {
                result = supplierBLL.Add(s);
            }

            ShowAlert(result.Success, result.Message);
            BindGrid();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "hideModal();", true);
        }

        private void ClearForm()
        {
            hfSupplierId.Value = string.Empty;
            txtName.Text = txtPhone.Text = txtEmail.Text = txtAddress.Text = txtAccountCode.Text = "";
            chkIsActive.Checked = true;
        }

        private void ShowAlert(bool success, string message)
        {
            string alertType = success ? "success" : "danger";
            litAlert.Text = $"<div class='alert alert-{alertType} alert-dismissible fade show mt-2'>" +
                            $"{message}" +
                            "<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
        }
    }
}
