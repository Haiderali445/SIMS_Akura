using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SIMS_Akura.BLL;
using SIMS_Akura.Models;

namespace SIMS_Akura.UI
{
    public partial class CustomerPage : Page
    {
        private readonly CustomerBLL bll = new CustomerBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid(string keyword = "")
        {
            var list = string.IsNullOrWhiteSpace(keyword)
                ? bll.GetAllAccountsCustomers(onlyActiveAccounts: chkActiveOnly.Checked)
                : bll.SearchAccountsCustomers(keyword, onlyActiveAccounts: chkActiveOnly.Checked);

            for (int i = 0; i < list.Count; i++)
                list[i].RowNo = i + 1;

            gvCustomers.DataSource = list;
            gvCustomers.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(txtSearch.Text.Trim());
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            chkActiveOnly.Checked = false;
            BindGrid();
        }

        protected void gvCustomers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCustomers.PageIndex = e.NewPageIndex;
            BindGrid(txtSearch.Text.Trim());
        }

        protected void gvCustomers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditCustomer")
            {
                long accountId = Convert.ToInt64(e.CommandArgument);
                LoadCustomer(accountId);
            }
            else if (e.CommandName == "ToggleActive")
            {
                long accountId = Convert.ToInt64(e.CommandArgument);
                var customer = bll.GetByAccountId(accountId);

                if (customer != null)
                {
                    customer.IsActive = !customer.IsActive;
                    var result = customer.Id == 0 ? bll.Add(customer) : bll.Update(customer);
                    ShowAlert(result.Message, result.Success ? "info" : "danger");
                    BindGrid();
                }
            }
            else if (e.CommandName == "DeleteCustomer")
            {
                long id = Convert.ToInt64(e.CommandArgument);
                if (id > 0)
                {
                    var result = bll.Delete(id);
                    ShowAlert(result.Message, result.Success ? "success" : "danger");
                    BindGrid();
                }
                else
                {
                    ShowAlert("No Customer record found to delete.", "warning");
                }
            }
        }

        protected void btnNewCustomer_Click(object sender, EventArgs e)
        {
            ClearForm();
            litAlert.Text = "";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNew = string.IsNullOrEmpty(hfCustomerId.Value) || hfCustomerId.Value == "0";

                var customer = new Customer
                {
                    Id = isNew ? 0 : Convert.ToInt64(hfCustomerId.Value),
                    AccountId = string.IsNullOrEmpty(hfAccountId.Value) ? (long?)null : Convert.ToInt64(hfAccountId.Value),
                    Name = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    IsActive = chkIsActive.Checked,
                    AccountCode = txtAccountCode.Text.Trim(),
                    AccountType = txtAccountType.Text.Trim()
                };

                var result = isNew ? bll.Add(customer) : bll.Update(customer);

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

        private void LoadCustomer(long accountId)
        {
            var c = bll.GetByAccountId(accountId);
            if (c == null)
            {
                ShowAlert("Customer not found.", "warning");
                return;
            }

            hfCustomerId.Value = c.Id.ToString();
            hfAccountId.Value = c.AccountId?.ToString() ?? "";
            txtName.Text = c.Name;
            txtPhone.Text = c.Phone;
            txtEmail.Text = c.Email;
            txtAddress.Text = c.Address;
            chkIsActive.Checked = c.IsActive;
            txtAccountCode.Text = c.AccountCode;
            txtAccountType.Text = c.AccountType;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        private void ClearForm()
        {
            hfCustomerId.Value = "";
            hfAccountId.Value = "";
            txtName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            chkIsActive.Checked = true;
            txtAccountCode.Text = "";
            txtAccountType.Text = "";
        }

        private void ShowAlert(string message, string type)
        {
            litAlert.Text = $@"
                <div class='alert alert-{type} alert-dismissible fade show' role='alert'>
                    {message}
                    <button type='button' class='btn-close' data-bs-dismiss='alert'></button>
                </div>";
        }
    }
}
