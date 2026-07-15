<%@ Page Title="Stock Center" Language="C#" MasterPageFile="~/LayOuts/DefaultLayout.Master"
    AutoEventWireup="true" CodeBehind="StockPage.aspx.cs" Inherits="SIMS_Akura.UI.StockPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <!-- 🔷 Header -->
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h4 class="fw-bold text-primary">📦 Stock Center</h4>
      </div>
    <div class="container-fluid mt-3">

        <h4 class="mb-3">📦 Stock Management</h4>

        <div class="row mb-3">
            <div class="col-md-3">
                <asp:DropDownList ID="ddlSupplier" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSupplier_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-primary" Text="↻ Refresh" OnClick="btnRefresh_Click" />
            </div>
        </div>

        <asp:GridView ID="gvStock" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover" OnRowCommand="gvStock_RowCommand">
            <Columns>
                <asp:BoundField DataField="ProductCode" HeaderText="Code" />
                <asp:BoundField DataField="ProductName" HeaderText="Name" />
                <asp:BoundField DataField="SupplierName" HeaderText="Supplier" />
                <asp:BoundField DataField="CurrentStock" HeaderText="Current Stock" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="MinimumStock" HeaderText="Min Stock" DataFormatString="{0:N2}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
<asp:CheckBox ID="chkActive" runat="server" Checked='<%# Eval("IsActive") %>' AutoPostBack="true" OnCheckedChanged="chkActive_CheckedChanged" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnViewBatches" runat="server" Text="📦 Batches" CommandName="ViewBatches" CommandArgument='<%# Eval("ProductId") %>' CssClass="btn btn-sm btn-outline-info" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Literal ID="litAlert" runat="server"></asp:Literal>

        <!-- Modal for stock batches -->
        <div class="modal fade" id="modalBatches" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">Stock Batches</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div id="batchDetails" runat="server"></div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

    </div>
  
</asp:Content>
