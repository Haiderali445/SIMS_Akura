<%@ Page Title="New Purchase" Language="C#" MasterPageFile="~/LayOuts/DefaultLayout.Master"
    AutoEventWireup="true" CodeBehind="PurchasePage.aspx.cs" Inherits="SIMS_Akura.UI.PurchasePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="sm" runat="server" />

    <div class="container my-3">
        <div class="card shadow-sm">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h4 class="m-0">🧾 New Purchase</h4>
                    <div>
                        <asp:Button ID="btnHistory" runat="server" CssClass="btn btn-outline-secondary btn-sm" Text="📜 History" />
                        <asp:Button ID="btnNew" runat="server" CssClass="btn btn-primary btn-sm ms-2" Text="+ New"  />
                    </div>
                </div>

                <asp:Literal ID="litAlert" runat="server"></asp:Literal>

                <div class="row g-3 mb-3">
                    <div class="col-md-3">
                        <label class="form-label">Invoice #</label>
                        <asp:TextBox ID="txtInvoiceCode" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Date</label>
                        <asp:TextBox ID="txtInvoiceDate" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Supplier *</label>
                        <asp:DropDownList ID="ddlSupplier" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Freight / Fare</label>
                        <input id="txtFare" class="form-control text-end" value="0.00" />
                    </div>
                </div>

                <div class="d-flex mb-2">
                    <input id="txtDiscount" class="form-control w-auto text-end me-2" value="0.00" placeholder="Discount" />
                    <button id="btnOpenAddItem" type="button" class="btn btn-success ms-auto" onclick="openAddItemModal()">+ Add Item</button>
                </div>

                <!-- Items table (client-side rendered) -->
                <div class="table-responsive mb-3">
                    <table class="table table-sm table-striped" id="tblItems">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Product</th>
                                <th class="text-end">Qty</th>
                                <th class="text-end">Rate</th>
                                <th class="text-end">Total</th>
                                <th>Batch</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody id="tblBodyItems">
                            <!-- client rows -->
                        </tbody>
                    </table>
                </div>

                <div class="row mb-4">
                    <div class="col-md-4 offset-md-8">
                        <div class="d-flex justify-content-between">
                            <strong>Subtotal:</strong><span id="lblSubtotal">0.00</span>
                        </div>
                        <div class="d-flex justify-content-between">
                            <strong>Discount:</strong><span id="lblDiscount">0.00</span>
                        </div>
                        <div class="d-flex justify-content-between">
                            <strong>Fare:</strong><span id="lblFare">0.00</span>
                        </div>
                        <hr />
                        <div class="d-flex justify-content-between fs-5">
                            <strong>Grand Total:</strong><span id="lblGrandTotal">0.00</span>
                        </div>
                    </div>
                </div>

                <div class="text-end">
                    <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary px-4" Text="💾 Save Purchase" OnClick="btnSave_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Hidden field to carry JSON items to server -->
    <asp:HiddenField ID="hfItemsJson" runat="server" />

    <!-- Add Item Modal -->
    <div class="modal fade" id="itemModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-md modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Purchase Item</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Product *</label>
                        <!-- we'll fill this select client-side from JSON injected below -->
                        <select id="selProduct" class="form-select"></select>
                    </div>
                    <div class="row g-2">
                        <div class="col-4">
                            <label class="form-label">Qty</label>
                            <input id="inpQty" class="form-control text-end" value="1" />
                        </div>
                        <div class="col-4">
                            <label class="form-label">Rate</label>
                            <input id="inpRate" class="form-control text-end" value="0.00" />
                        </div>
                        <div class="col-4">
                            <label class="form-label">Batch Code</label>
                            <input id="inpBatch" class="form-control" readonly />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button id="btnAddItemClient" type="button" class="btn btn-success" onclick="addItemClient()">Add</button>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Inject product JSON & small JS -->
    <asp:Literal ID="litData" runat="server" />

   <script>
       let products = [];
       let clientItems = [];

       function setProducts(list) {
           products = list;
           const sel = document.getElementById('selProduct');
           sel.innerHTML = '<option value="0">-- Select Product --</option>';
           for (let p of products) {
               const opt = document.createElement('option');
               opt.value = p.Id;
               opt.text = `${p.Name}${p.ProductCode ? ' (' + p.ProductCode + ')' : ''}`;
               sel.appendChild(opt);
           }
       }

       function openAddItemModal() {
           document.getElementById('inpBatch').value = 'BATCH-' + Date.now().toString().slice(-6);
           document.getElementById('inpQty').value = '1';
           document.getElementById('inpRate').value = '0.00';
           new bootstrap.Modal(document.getElementById('itemModal')).show();
       }

       document.addEventListener('DOMContentLoaded', function () {
           const sel = document.getElementById('selProduct');
           if (sel) {
               sel.addEventListener('change', function () {
                   const prod = products.find(x => x.Id == this.value);
                   document.getElementById('inpRate').value = prod ? (prod.DefaultPurchasePrice || 0).toFixed(2) : '0.00';
               });
           }
       });

       function addItemClient() {
           const sel = document.getElementById('selProduct');
           if (sel.value === "0") return alert('Select a product first.');

           const prod = products.find(x => x.Id == sel.value);
           const qty = parseFloat(document.getElementById('inpQty').value) || 0;
           const rate = parseFloat(document.getElementById('inpRate').value) || 0;
           const batch = document.getElementById('inpBatch').value || '';
           const total = +(qty * rate).toFixed(2);

           const item = { ProductId: prod.Id, ProductName: prod.Name, Qty: qty, Rate: rate, BatchCode: batch, Total: total };

           const existing = clientItems.find(x => x.ProductId == item.ProductId && x.BatchCode == item.BatchCode);
           if (existing) {
               existing.Qty += item.Qty;
               existing.Total = +(existing.Qty * existing.Rate).toFixed(2);
           } else clientItems.push(item);

           renderItems();
           bootstrap.Modal.getInstance(document.getElementById('itemModal')).hide();
       }

       function renderItems() {
           const tbody = document.getElementById('tblBodyItems');
           tbody.innerHTML = '';
           let subtotal = 0, idx = 1;

           for (let it of clientItems) {
               const tr = document.createElement('tr');
               tr.innerHTML = `
                <td>${idx++}</td>
                <td>${it.ProductName}</td>
                <td class="text-end">${it.Qty}</td>
                <td class="text-end">${it.Rate.toFixed(2)}</td>
                <td class="text-end">${it.Total.toFixed(2)}</td>
                <td>${it.BatchCode}</td>
                <td><button class="btn btn-sm btn-outline-danger" onclick="removeItem(${idx - 2})">Remove</button></td>`;
               tbody.appendChild(tr);
               subtotal += it.Total;
           }

           const discount = parseFloat(document.getElementById('txtDiscount').value) || 0;
           const fare = parseFloat(document.getElementById('txtFare').value) || 0;

           document.getElementById('lblSubtotal').innerText = subtotal.toFixed(2);
           document.getElementById('lblDiscount').innerText = discount.toFixed(2);
           document.getElementById('lblFare').innerText = fare.toFixed(2);
           document.getElementById('lblGrandTotal').innerText = (subtotal - discount + fare).toFixed(2);

           document.getElementById('<%= hfItemsJson.ClientID %>').value = JSON.stringify(clientItems);
       }

       function removeItem(i) {
           clientItems.splice(i, 1);
           renderItems();
       }

       function loadClientProductsAndInit(prodArr) {
           setProducts(prodArr);
       }
   </script>

</asp:Content>
