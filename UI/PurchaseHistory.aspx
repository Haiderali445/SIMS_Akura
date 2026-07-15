<%@ Page Title="" Language="C#" MasterPageFile="~/LayOuts/DefaultLayout.Master" AutoEventWireup="true" CodeBehind="PurchaseHistory.aspx.cs" Inherits="SIMS_Akura.UI.PurchaseHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- ScriptManager must come first -->
    <asp:ScriptManager ID="scriptManager" runat="server" />

    <div class="container-fluid mt-3">
        <div class="card shadow-sm">
            <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                <h5 class="mb-0">📜 Purchase History</h5>
                <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-light btn-sm" Text="↻ Refresh" OnClick="btnRefresh_Click" />
            </div>

            <div class="card-body">
                <!-- Filter -->
                <div class="row g-2 mb-3">
                    <div class="col-md-3">
                        <label class="form-label">From Date</label>
                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">To Date</label>
                        <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Supplier</label>
                        <asp:DropDownList ID="ddlSupplier" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-3 d-flex align-items-end">
                        <asp:Button ID="btnFilter" runat="server" CssClass="btn btn-primary w-100" Text="🔍 Apply Filter" OnClick="btnFilter_Click" />
                    </div>
                </div>

                <!-- Grid inside UpdatePanel -->
                <asp:UpdatePanel ID="updHistory" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="False"
                            CssClass="table table-striped table-hover"
                            OnRowCommand="gvHistory_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="InvoiceCode" HeaderText="Invoice Code" />
                                <asp:BoundField DataField="SupplierName" HeaderText="Supplier" />
                                <asp:BoundField DataField="GrandTotal" HeaderText="Grand Total" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="CreatedAt" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <asp:Button ID="btnView" runat="server" CommandName="ViewInvoice" 
                                            CommandArgument='<%# Eval("InvoiceId") %>' Text="👁 View" 
                                            CssClass="btn btn-outline-primary btn-sm" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:Literal ID="litAlert" runat="server"></asp:Literal>
            </div>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="modalInvoice" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title">Invoice Details</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div id="invoiceDetails" runat="server"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
