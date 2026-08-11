<%@ Page Title="Customers" Language="C#" MasterPageFile="~/LayOuts/DefaultLayout.Master" AutoEventWireup="true" CodeBehind="CustomerPage.aspx.cs" Inherits="SIMS_Akura.UI.CustomerPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <!-- Navbar -->
    <nav class="navbar navbar-expand-lg bg-light rounded shadow-sm px-3 mb-3">
        <div class="container-fluid">
            <span class="navbar-brand fw-bold">👥 Customers</span>
            <div class="d-flex gap-2">
                <asp:Button ID="btnNewCustomer" runat="server" CssClass="btn btn-primary btn-sm" Text="+ Add Customer" OnClick="btnNewCustomer_Click" />
                <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-outline-secondary btn-sm" Text="⟳ Refresh" OnClick="btnRefresh_Click" />
                <a href="AccountPage.aspx" class="btn btn-outline-info btn-sm">💼 Accounts</a>
            </div>
        </div>
    </nav>

    <!-- Search + Filters -->
    <div class="row mb-3 g-2">
        <div class="col-md-6">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Name, Phone, Email, Address, Account Code" />
        </div>
        <div class="col-md-2">
            <asp:CheckBox ID="chkActiveOnly" runat="server" Text="Active Only" CssClass="form-check-input me-2" />
        </div>
        <div class="col-md-2">
            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-outline-primary" Text="Search" OnClick="btnSearch_Click" />
        </div>
    </div>

    <!-- Alerts -->
    <asp:Literal ID="litAlert" runat="server" />

    <!-- Grid -->
    <div class="table-responsive">
        <asp:GridView ID="gvCustomers" runat="server" CssClass="table table-striped table-sm align-middle"
            AutoGenerateColumns="False" DataKeyNames="AccountId"
            AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gvCustomers_PageIndexChanging"
            OnRowCommand="gvCustomers_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="#" DataField="RowNo" />
                <asp:BoundField HeaderText="Name" DataField="Name" />
                <asp:TemplateField HeaderText="Account Code">
                    <ItemTemplate>
                        <%# string.IsNullOrWhiteSpace(Eval("AccountCode") as string)
                            ? "<span class='text-muted'>—</span>"
                            : $"<a href='AccountPage.aspx?code={Eval("AccountCode")}' class='text-decoration-none'>{Eval("AccountCode")}</a>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Type" DataField="AccountType" />
                <asp:BoundField HeaderText="Phone" DataField="Phone" />
                <asp:BoundField HeaderText="Email" DataField="Email" />
                <asp:BoundField HeaderText="Address" DataField="Address" />
                <asp:TemplateField HeaderText="Active">
                    <ItemTemplate>
                        <%# (bool)Eval("IsActive") ? "<span class='badge bg-success'>Yes</span>" : "<span class='badge bg-secondary'>No</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Created At" DataField="CreatedAt" DataFormatString="{0:dd MMM yyyy}" />
                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="220px">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-sm btn-outline-primary me-1"
                            CommandName="EditCustomer" CommandArgument='<%# Eval("AccountId") %>'>
                            <i class="fa-solid fa-pen"></i>
                        </asp:LinkButton>
                        <asp:LinkButton ID="lnkToggle" runat="server" CssClass="btn btn-sm btn-outline-warning me-1"
                            CommandName="ToggleActive" CommandArgument='<%# Eval("AccountId") %>'>
                            <i class="fa-solid fa-power-off"></i>
                        </asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-outline-danger"
                            CommandName="DeleteCustomer" CommandArgument='<%# Eval("Id") %>'
                            OnClientClick="return confirm('Delete this customer?');">
                            <i class="fa-solid fa-trash"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="customerModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Customer</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfCustomerId" runat="server" />
                    <asp:HiddenField ID="hfAccountId" runat="server" />
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label">Name *</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Linked account code</label>
                            <asp:TextBox ID="txtAccountCode" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Account Type</label>
                            <asp:TextBox ID="txtAccountType" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Phone</label>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-12">
                            <label class="form-label">Address</label>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="col-12 form-check form-switch mt-2">
                            <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" />
                            <label class="form-check-label" for="chkIsActive">Active</label>
                        </div>
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
                var modal = new bootstrap.Modal(document.getElementById('customerModal'));
                modal.show();
            }, 100);
        }
        function hideModal() {
            var modal = bootstrap.Modal.getInstance(document.getElementById('customerModal'));
            if (modal) modal.hide();
        }
    </script>
</asp:Content>
