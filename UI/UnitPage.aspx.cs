using SIMS_Akura.BLL;
using UnitModel = SIMS_Akura.Models.Unit;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS_Akura.UI
{
    public partial class UnitPage : Page
    {
        private readonly UnitBLL _unitService = new UnitBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid(string keyword = "")
        {
            var list = _unitService.GetAll();

            if (!string.IsNullOrWhiteSpace(keyword))
                list = list.Where(u =>
                    (u.Name != null && u.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (u.Code != null && u.Code.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();

            var data = list.Select((u, i) => new
            {
                RowNo = i + 1,
                u.Id,
                u.Code,
                u.Name,
                u.CreatedAt
            }).ToList();

            gvUnits.DataSource = data;
            gvUnits.DataBind();
        }

        protected void btnNewUnit_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                UnitModel u = new UnitModel
                {
                    Id = string.IsNullOrEmpty(hfUnitId.Value) ? 0 : Convert.ToInt64(hfUnitId.Value),
                    Code = txtCode.Text.Trim(),
                    Name = txtName.Text.Trim()
                };

                var result = u.Id == 0 ? _unitService.Add(u) : _unitService.Update(u);

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

        protected void gvUnits_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUnit")
            {
                long id = Convert.ToInt64(e.CommandArgument);
                LoadUnit(id);
            }
            else if (e.CommandName == "DeleteUnit")
            {
                long id = Convert.ToInt64(e.CommandArgument);
                var result = _unitService.Delete(id);
                ShowAlert(result.Message, result.Success ? "success" : "danger");
                BindGrid();
            }
        }

        private void LoadUnit(long id)
        {
            var u = _unitService.GetAll().FirstOrDefault(x => x.Id == id);
            if (u == null)
            {
                ShowAlert("Unit not found.", "warning");
                return;
            }

            hfUnitId.Value = u.Id.ToString();
            txtCode.Text = u.Code;
            txtName.Text = u.Name;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(txtSearch.Text.Trim());
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            BindGrid();
        }

        protected void gvUnits_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvUnits.PageIndex = e.NewPageIndex;
            BindGrid(txtSearch.Text.Trim());
        }

        private void ClearForm()
        {
            hfUnitId.Value = "";
            txtCode.Text = txtName.Text = "";
        }

        private void ShowAlert(string message, string type)
        {
            litAlert.Text = $"<div class='alert alert-{type} alert-dismissible fade show' role='alert'>{message}<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
        }
    }
}
