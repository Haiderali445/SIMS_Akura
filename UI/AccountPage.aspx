<%@ Page Title="Accounts" Language="C#" MasterPageFile="~/LayOuts/DefaultLayout.Master" AutoEventWireup="true" CodeBehind="AccountPage.aspx.cs" Inherits="SIMS_Akura.UI.AccountPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <!-- Top Navbar -->
    <nav class="navbar navbar-expand-lg bg-light rounded shadow-sm px-3 mb-3">
        <div class="container-fluid">
            <span class="navbar-brand fw-bold">💼 Accounts</span>
            <div class="d-flex gap-2">
                <asp:Button ID="btnNewAccount" runat="server" CssClass="btn btn-primary btn-sm" Text="+ Add Account" OnClick="btnNewAccount_Click" />
                <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-outline-secondary btn-sm" Text="⟳ Refresh" OnClick="btnRefresh_Click" />
            </div>
        </div>
    </nav>

    <!-- Search -->
    <div class="row mb-3 g-2">
        <div class="col-md-5">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Name, Code, Phone, Email" />
        </div>
        <div class="col-md-3">
            <asp:DropDownList ID="ddlTypeFilter" runat="server" CssClass="form-select">
                <asp:ListItem Text="All Types" Value="" />
                <asp:ListItem Text="Supplier" Value="Supplier" />
                <asp:ListItem Text="Customer" Value="Customer" />
                <asp:ListItem Text="Other" Value="Other" />
            </asp:DropDownList>
        </div>
        <div class="col-md-2">
            <asp:CheckBox ID="chkActiveOnly" runat="server" Text="Active Only" CssClass="form-check-input me-2" />
        </div>
        <div class="col-md-2">
            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-outline-primary" Text="Search" OnClick="btnSearch_Click" />
        </div>
    </div>

    <!-- Alert -->
    <asp:Literal ID="litAlert" runat="server" />

    <!-- Grid -->
    <div class="table-responsive">
        <asp:GridView ID="gvAccounts" runat="server" CssClass="table table-striped table-sm align-middle"
            AutoGenerateColumns="False" DataKeyNames="Id"
            AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gvAccounts_PageIndexChanging"
            OnRowCommand="gvAccounts_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="#" DataField="RowNo" />
                <asp:BoundField HeaderText="Code" DataField="AccountCode" />
                <asp:BoundField HeaderText="Name" DataField="Name" />
                <asp:BoundField HeaderText="Type" DataField="AccountType" />
                <asp:BoundField HeaderText="Phone" DataField="Phone" />
                <asp:BoundField HeaderText="Email" DataField="Email" />
                <asp:BoundField HeaderText="Balance" DataField="CurrentBalance" DataFormatString="{0:N2}" />
                <asp:TemplateField HeaderText="Active">
                    <ItemTemplate>
                        <%# (bool)Eval("IsActive") ? "<span class='badge bg-success'>Yes</span>" : "<span class='badge bg-secondary'>No</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Created At" DataField="CreatedAt" DataFormatString="{0:dd MMM yyyy}" />
                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-sm btn-outline-primary me-1"
                            CommandName="EditAccount" CommandArgument='<%# Eval("Id") %>'>
                            <i class="fa-solid fa-pen"></i>
                        </asp:LinkButton>
                        <asp:LinkButton ID="lnkToggle" runat="server" CssClass="btn btn-sm btn-outline-warning"
                            CommandName="ToggleActive" CommandArgument='<%# Eval("Id") %>'>
                            <i class="fa-solid fa-power-off"></i>
                        </asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-outline-danger"
    CommandName="DeleteAccount" CommandArgument='<%# Eval("Id") %>'>
    <i class="fa-solid fa-trash"></i>
</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="accountModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-md modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Account</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfAccountId" runat="server" />
                    <div class="mb-3">
                        <label class="form-label">Account Code *</label>
                        <asp:TextBox ID="txtCode" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Auto-generated..." />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Name *</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                    </div>
        <div class="mb-3">
    <label class="form-label">Type *</label>
    <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select">
        <asp:ListItem Text="Supplier" Value="Supplier" />
        <asp:ListItem Text="Customer" Value="Customer" />
        <asp:ListItem Text="Other" Value="Other" />
    </asp:DropDownList>
</div>

                    <div class="mb-3">
                        <label class="form-label">Phone</label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Address</label>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Opening Balance</label>
                        <asp:TextBox ID="txtBalance" runat="server" CssClass="form-control" />
                    </div>
                    <div class="form-check form-switch mb-3">
                        <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label" for="chkIsActive">Active</label>
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
                var modal = new bootstrap.Modal(document.getElementById('accountModal'));
                modal.show();
            }, 100);
        }

        function hideModal() {
            var modal = bootstrap.Modal.getInstance(document.getElementById('accountModal'));
            if (modal) modal.hide();
        }
      

    </script>
</asp:Content>
