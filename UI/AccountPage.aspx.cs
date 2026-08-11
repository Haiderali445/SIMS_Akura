using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using SIMS_Akura.BLL;
using SIMS_Akura.Models;

namespace SIMS_Akura.UI
{
    public partial class AccountPage : Page
    {
        private readonly AccountBLL bll = new AccountBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            string keyword = txtSearch.Text.Trim();
            string type = ddlTypeFilter.SelectedValue;
            bool? activeOnly = chkActiveOnly.Checked ? true : (bool?)null;

            var list = bll.Search(keyword, type, activeOnly);

            for (int i = 0; i < list.Count; i++)
                list[i].RowNo = i + 1;

            gvAccounts.DataSource = list;
            gvAccounts.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlTypeFilter.SelectedIndex = 0;
            chkActiveOnly.Checked = false;
            BindGrid();
        }

        protected void btnNewAccount_Click(object sender, EventArgs e)
        {
            ClearForm();
            litAlert.Text = "";

            // Always generate ACT- code
            txtCode.Text = GenerateCode();

            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "showModal();", true);
        }

        // ✅ Simplified: always ACT- prefix
        private string GenerateCode()
        {
            var allAccounts = bll.Search("", "", null);
            var existingCodes = new HashSet<string>();

            foreach (var acc in allAccounts)
                existingCodes.Add(acc.AccountCode);

            string newCode;
            var rand = new Random();

            do
            {
                string randomPart = rand.Next(1000000000, int.MaxValue).ToString(); // 10-digit random
                newCode = $"ACT-{randomPart}";
            }
            while (existingCodes.Contains(newCode));

            return newCode;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNew = string.IsNullOrEmpty(hfAccountId.Value) || hfAccountId.Value == "0";
                string accountCode = isNew ? GenerateCode() : txtCode.Text.Trim();

                var acc = new Account
                {
                    Id = isNew ? 0 : Convert.ToInt64(hfAccountId.Value),
                    AccountCode = accountCode,
                    Name = txtName.Text.Trim(),
                    AccountType = ddlType.SelectedValue, // type stored separately
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    CurrentBalance = decimal.TryParse(txtBalance.Text.Trim(), out decimal bal) ? bal : 0,
                    IsActive = chkIsActive.Checked,
                    CreatedBy = 1
                };

                bool success = bll.Save(acc);
                ShowAlert(success ? "Account saved successfully." : "Failed to save account.", success ? "success" : "danger");

                BindGrid();
                ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "hideModal();", true);
            }
            catch (Exception ex)
            {
                ShowAlert("Error: " + ex.Message, "danger");
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            hfAccountId.Value = "";
            txtCode.Text = "";
            txtName.Text = "";
            ddlType.SelectedIndex = 0;
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            txtBalance.Text = "";
            chkIsActive.Checked = true;
        }

        protected void gvAccounts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAccounts.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            long id = Convert.ToInt64(e.CommandArgument);
            if (e.CommandName == "EditAccount")
            {
                var acc = bll.GetById(id);
                if (acc != null)
                {
                    hfAccountId.Value = acc.Id.ToString();
                    txtCode.Text = acc.AccountCode;
                    txtName.Text = acc.Name;
                    ddlType.SelectedValue = acc.AccountType;
                    txtPhone.Text = acc.Phone;
                    txtEmail.Text = acc.Email;
                    txtAddress.Text = acc.Address;
                    txtBalance.Text = acc.CurrentBalance.ToString("N2");
                    chkIsActive.Checked = acc.IsActive;

                    litAlert.Text = "";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "showModal();", true);
                }
            }
            else if (e.CommandName == "ToggleActive")
            {
                var acc = bll.GetById(id);
                if (acc != null)
                {
                    bool newStatus = !acc.IsActive;
                    bool success = bll.SetActiveStatus(id, newStatus);
                    ShowAlert(success
                        ? "Account " + (newStatus ? "activated." : "deactivated.")
                        : "Failed to update status.",
                        success ? "info" : "danger");

                    BindGrid();
                }
            }
            else if (e.CommandName == "DeleteAccount")
            {
                bool success = bll.Delete(id);
                ShowAlert(success ? "Account deleted successfully." : "Failed to delete account.", success ? "danger" : "warning");
                BindGrid();
            }
        }

        // ✅ WebMethod simplified: always ACT- prefix
        [System.Web.Services.WebMethod]
        public static string GetGeneratedCode(string type)
        {
            var bll = new AccountBLL();
            var allAccounts = bll.Search("", "", null);
            var existingCodes = new HashSet<string>();

            foreach (var acc in allAccounts)
                existingCodes.Add(acc.AccountCode);

            string newCode;
            var rand = new Random();

            do
            {
                string randomPart = rand.Next(1000000000, int.MaxValue).ToString();
                newCode = $"ACT-{randomPart}";
            }
            while (existingCodes.Contains(newCode));

            return newCode;
        }

        private void ShowAlert(string message, string type)
        {
            litAlert.Text = @"
                <div class='alert alert-" + type + @" alert-dismissible fade show' role='alert'>
                    " + message + @"
                    <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
                </div>";
        }
    }
}
