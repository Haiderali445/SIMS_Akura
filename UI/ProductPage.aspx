<%@ Page Title="Products" Language="C#" MasterPageFile="~/LayOuts/DefaultLayout.Master" AutoEventWireup="true" CodeBehind="ProductPage.aspx.cs" Inherits="SIMS_Akura.UI.Products" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

   <!-- Top Navbar for Actions -->
<nav class="navbar navbar-expand-lg bg-light rounded shadow-sm px-3 mb-3">
    <div class="container-fluid">
        <span class="navbar-brand fw-bold">🗂️ Products</span>
        <div class="d-flex gap-2">
            <asp:Button ID="btnNewProduct" runat="server" CssClass="btn btn-primary btn-sm" Text="+ Add Product" OnClick="btnNewProduct_Click" />
            <a href="StockPage.aspx" class="btn btn-outline-dark btn-sm">
                <i class="fa-solid fa-boxes-stacked me-1"></i> Manage Stock
            </a>
            <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-outline-secondary btn-sm" Text="⟳ Refresh"
                OnClick="btnRefresh_Click" />
        </div>
    </div>
</nav>



        <!-- Search Bar -->
        <div class="row mb-3 g-2">
            <div class="col-md-4">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Name / Code / Brand" />
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-outline-primary" Text="Search" OnClick="btnSearch_Click" />
            </div>
        </div>

        <asp:Literal ID="litAlert" runat="server" />

        <!-- Products Grid -->
        <div class="table-responsive">
            <asp:GridView ID="gvProducts" runat="server" CssClass="table table-striped table-sm align-middle"
                AutoGenerateColumns="False" DataKeyNames="Id" AllowPaging="true"
    PageSize="10"
    OnPageIndexChanging="gvProducts_PageIndexChanging" OnRowCommand="gvProducts_RowCommand">
               <Columns>
    <asp:BoundField HeaderText="#" DataField="RowNo" />
    <asp:BoundField HeaderText="Code" DataField="ProductCode" />
    <asp:BoundField HeaderText="Name" DataField="Name" />
    <asp:BoundField HeaderText="Barcode" DataField="Barcode" />
    <asp:BoundField HeaderText="Category" DataField="CategoryName" />
    <asp:BoundField HeaderText="Brand" DataField="Brand" />
    <asp:BoundField HeaderText="Stock" DataField="CurrentStock" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
    <asp:BoundField HeaderText="Description" DataField="Description" HtmlEncode="false" />
    <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px">
        <ItemTemplate>
            <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-sm btn-outline-primary me-1"
                CommandName="EditProduct" CommandArgument='<%# Eval("Id") %>'>
                <i class="fa-solid fa-pen"></i>
            </asp:LinkButton>
            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-outline-danger"
                CommandName="DeleteProduct" CommandArgument='<%# Eval("Id") %>'
                OnClientClick="return confirm('Delete this product?');">
                <i class="fa-solid fa-trash"></i>
            </asp:LinkButton>
        </ItemTemplate>
    </asp:TemplateField>
</Columns>

            </asp:GridView>
        </div>
    

    <!-- Add/Edit Modal -->
    <div class="modal fade" id="productModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Product</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfProductId" runat="server" />
                    <div class="row g-2">
                        <div class="col-md-6">
                            <label class="form-label">Product Code</label>
                            <asp:TextBox ID="txtProductCode" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Barcode</label>
                            <asp:TextBox ID="txtBarcode" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                            <!-- Barcode -->

                        <div class="mt-2">
                            <div class="col-12 mt-3">
                                <label class="form-label">Barcode Preview</label>
                                <div class="text-center">
                                    <asp:Image ID="imgBarcode" runat="server" CssClass="img-fluid mb-2 border rounded shadow-sm"
                                        ToolTip="Hover to preview barcode" Style="transition: transform 0.3s ease;"
                                        onmouseover="this.style.transform='scale(1.05)'"
                                        onmouseout="this.style.transform='scale(1)'" />
                                    <br />
                                    <button type="button" class="btn btn-outline-secondary btn-sm me-2 mt-2" onclick="downloadBarcode()">📥 Download</button>
                                    <button type="button" class="btn btn-outline-secondary btn-sm mt-2" onclick="printBarcode()">🖨️ Print</button>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <label class="form-label">Name *</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Category</label>
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Brand</label>
                            <asp:TextBox ID="txtBrand" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Unit</label>
                            <asp:DropDownList ID="ddlUnit" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-12">
                            <div class="form-check form-switch">
                                <asp:CheckBox ID="chkManageStock" runat="server" CssClass="form-check-input" onclick="toggleStockFields()" />
                                <label class="form-check-label" for="chkManageStock">Manage Stock</label>
                            </div>
                        </div>

                        <div class="col-md-4">
                            <label class="form-label">Purchase Price</label>
                            <asp:TextBox ID="txtPurchase" runat="server" CssClass="form-control" TextMode="Number" disabled="true" />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Sales Price</label>
                            <asp:TextBox ID="txtSales" runat="server" CssClass="form-control" TextMode="Number" disabled="true" />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Minimum Stock</label>
                            <asp:TextBox ID="txtMinStock" runat="server" CssClass="form-control" TextMode="Number" disabled="true" />
                        </div>
                        <div class="col-12">
                            <label class="form-label">Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" />
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
                var modal = new bootstrap.Modal(document.getElementById('productModal'));
                modal.show();
            }, 100);
        }

        function hideModal() {
            var modal = bootstrap.Modal.getInstance(document.getElementById('productModal'));
            if (modal) modal.hide();
        }

        function toggleStockFields() {
            const isChecked = document.getElementById('<%= chkManageStock.ClientID %>').checked;
            document.getElementById('<%= txtPurchase.ClientID %>').disabled = !isChecked;
            document.getElementById('<%= txtSales.ClientID %>').disabled = !isChecked;
            document.getElementById('<%= txtMinStock.ClientID %>').disabled = !isChecked;
        }

        function downloadBarcode() {
            const img = document.getElementById('<%= imgBarcode.ClientID %>');
            if (!img || !img.src) return;

            const link = document.createElement('a');
            link.href = img.src;
            link.download = 'barcode.png';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }

        function printBarcode() {
            const img = document.getElementById('<%= imgBarcode.ClientID %>');
            if (!img || !img.src) return;

            const win = window.open('', '_blank');
            win.document.write(`<html><head><title>Print Barcode</title></head><body style="text-align:center;"><img src="${img.src}" style="max-width:100%;"/><script>window.print();`);
            win.document.close();
        }
    </script>

</asp:Content>
