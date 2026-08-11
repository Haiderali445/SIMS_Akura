using SIMS_Akura.BLL;
using SIMS_Akura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS_Akura.UI
{
    public partial class CategoryPage : Page
    {
        private readonly CategoryBLL _categoryService = new CategoryBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid(string keyword = "")
        {
            var list = string.IsNullOrWhiteSpace(keyword)
                ? _categoryService.GetAll()
                : _categoryService.Search(keyword);

            var data = list.Select((c, i) => new
            {
                RowNo = i + 1,
                c.Id,
                c.Name,
                c.Description,
                c.IsActive,
                c.CreatedAt
            }).ToList();

            gvCategories.DataSource = data;
            gvCategories.DataBind();
        }

        protected void btnNewCategory_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Category c = new Category
                {
                    Id = string.IsNullOrEmpty(hfCategoryId.Value) ? 0 : Convert.ToInt64(hfCategoryId.Value),
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    IsActive = chkIsActive.Checked,
                    CreatedBy = 1 // You can replace with actual user ID if available
                };

                var result = c.Id == 0 ? _categoryService.Add(c) : _categoryService.Update(c);

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

        protected void gvCategories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditCategory")
            {
                long id = Convert.ToInt64(e.CommandArgument);
                LoadCategory(id);
            }
            else if (e.CommandName == "DeleteCategory")
            {
                long id = Convert.ToInt64(e.CommandArgument);
                var result = _categoryService.Delete(id);
                ShowAlert(result.Message, result.Success ? "success" : "danger");
                BindGrid();
            }
        }

        private void LoadCategory(long id)
        {
            var c = _categoryService.GetById(id);
            if (c == null)
            {
                ShowAlert("Category not found.", "warning");
                return;
            }

            hfCategoryId.Value = c.Id.ToString();
            txtName.Text = c.Name;
            txtDescription.Text = c.Description;
            chkIsActive.Checked = c.IsActive;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            BindGrid(keyword);
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            BindGrid();
        }

        protected void gvCategories_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategories.PageIndex = e.NewPageIndex;
            BindGrid(txtSearch.Text.Trim());
        }

        private void ClearForm()
        {
            hfCategoryId.Value = "";
            txtName.Text = txtDescription.Text = "";
            chkIsActive.Checked = true;
        }

        private void ShowAlert(string message, string type)
        {
            litAlert.Text = $"<div class='alert alert-{type} alert-dismissible fade show' role='alert'>{message}<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
        }
    }
}
