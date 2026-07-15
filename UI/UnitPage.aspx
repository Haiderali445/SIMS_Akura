<%@ Page Title="Units" Language="C#" MasterPageFile="~/LayOuts/DefaultLayout.Master" AutoEventWireup="true" CodeBehind="UnitPage.aspx.cs" Inherits="SIMS_Akura.UI.UnitPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <!-- Navbar-style top actions -->
    <nav class="navbar navbar-expand-lg bg-light rounded shadow-sm px-3 mb-3">
        <div class="container-fluid">
            <span class="navbar-brand fw-bold">📐 Units</span>
            <div class="d-flex gap-2">
                <asp:Button ID="btnNewUnit" runat="server" CssClass="btn btn-primary btn-sm" Text="+ Add Unit" OnClick="btnNewUnit_Click" />
                <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-outline-secondary btn-sm" Text="⟳ Refresh" OnClick="btnRefresh_Click" />
            </div>
        </div>
    </nav>

    <!-- Search Bar -->
    <div class="row mb-3 g-2">
        <div class="col-md-5">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Code or Name" />
        </div>
        <div class="col-md-2">
            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-outline-primary" Text="Search" OnClick="btnSearch_Click" />
        </div>
    </div>

    <asp:Literal ID="litAlert" runat="server" />

    <!-- Units Grid -->
    <div class="table-responsive">
        <asp:GridView ID="gvUnits" runat="server" CssClass="table table-striped table-sm align-middle"
            AutoGenerateColumns="False" DataKeyNames="Id"
            AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gvUnits_PageIndexChanging"
            OnRowCommand="gvUnits_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="#" DataField="RowNo" />
                <asp:BoundField HeaderText="Code" DataField="Code" />
                <asp:BoundField HeaderText="Name" DataField="Name" />
                <asp:BoundField HeaderText="Created At" DataField="CreatedAt" DataFormatString="{0:dd MMM yyyy}" />
                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-sm btn-outline-primary me-1"
                            CommandName="EditUnit" CommandArgument='<%# Eval("Id") %>'>
                            <i class="fa-solid fa-pen"></i>
                        </asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-outline-danger"
                            CommandName="DeleteUnit" CommandArgument='<%# Eval("Id") %>'
                            OnClientClick="return confirm('Delete this unit?');">
                            <i class="fa-solid fa-trash"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Add/Edit Modal -->
    <div class="modal fade" id="unitModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-md modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Unit</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfUnitId" runat="server" />
                    <div class="mb-3">
                        <label class="form-label">Code *</label>
                        <asp:TextBox ID="txtCode" runat="server" CssClass="form-control" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Name *</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <script>
        function showModal() {
            setTimeout(function () {
                var modal = new bootstrap.Modal(document.getElementById('unitModal'));
                modal.show();
            }, 100);
        }

        function hideModal() {
            var modal = bootstrap.Modal.getInstance(document.getElementById('unitModal'));
            if (modal) modal.hide();
        }
    </script>
</asp:Content>
