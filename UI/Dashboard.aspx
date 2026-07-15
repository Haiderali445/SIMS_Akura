<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SIMS_Akura.UI.Dashboard" MasterPageFile="~/LayOuts/DefaultLayout.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="container-fluid mt-3">

    <h4 class="mb-3">📊 Dashboard Overview</h4>

    <!-- KPI Cards -->
    <div class="row mb-4">
        <div class="col-md-3">
            <a href="StockPage.aspx" class="text-decoration-none">
                <div class="card text-white bg-primary mb-3 shadow-sm hover-scale">
                    <div class="card-body d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="card-title">Total Stock Value</h6>
                            <h4 id="lblStockValue" runat="server"></h4>
                        </div>
                        <i class="bi bi-box display-4 opacity-75"></i>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-md-3">
            <a href="PurchaseHistory.aspx" class="text-decoration-none">
                <div class="card text-white bg-success mb-3 shadow-sm hover-scale">
                    <div class="card-body d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="card-title">Total Purchases</h6>
                            <h4 id="lblTotalPurchases" runat="server"></h4>
                        </div>
                        <i class="bi bi-cart display-4 opacity-75"></i>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-md-3">
            <a href="SalesPage.aspx" class="text-decoration-none">
                <div class="card text-white bg-warning mb-3 shadow-sm hover-scale">
                    <div class="card-body d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="card-title">Total Sales</h6>
                            <h4 id="lblTotalSales" runat="server"></h4>
                        </div>
                        <i class="bi bi-bag display-4 opacity-75"></i>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-md-3">
            <a href="#" class="text-decoration-none">
                <div class="card text-white bg-danger mb-3 shadow-sm hover-scale">
                    <div class="card-body d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="card-title">Low Stock Products</h6>
                            <h4 id="lblLowStock" runat="server"></h4>
                        </div>
                        <i class="bi bi-exclamation-triangle display-4 opacity-75"></i>
                    </div>
                </div>
            </a>
        </div>
    </div>

    <!-- Charts -->
    <div class="row mb-4">
        <div class="col-md-6">
            <div class="card shadow-sm">
                <div class="card-header bg-secondary text-white">Stock vs Minimum Stock</div>
                <div class="card-body">
                    <canvas id="chartStock"></canvas>
                </div>
            </div>
        </div>
        <div class="col-md-6">
            <div class="card shadow-sm">
                <div class="card-header bg-secondary text-white">Purchase vs Sales (Monthly)</div>
                <div class="card-body">
                    <canvas id="chartPurchaseSales"></canvas>
                </div>
            </div>
        </div>
    </div>

    <!-- Stock Table -->
    <h5>Stock Overview</h5>
    <asp:GridView ID="gvStock" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover">
        <Columns>
            <asp:BoundField DataField="ProductCode" HeaderText="Code" />
            <asp:BoundField DataField="ProductName" HeaderText="Name" />
            <asp:BoundField DataField="CategoryName" HeaderText="Category" />
            <asp:BoundField DataField="UnitName" HeaderText="Unit" />
            <asp:BoundField DataField="CurrentStock" HeaderText="Stock" DataFormatString="{0:N2}" />
            <asp:BoundField DataField="MinimumStock" HeaderText="Min Stock" DataFormatString="{0:N2}" />
            <asp:BoundField DataField="PurchasePrice" HeaderText="Purchase Price" DataFormatString="{0:N2}" />
            <asp:BoundField DataField="SalesPrice" HeaderText="Sales Price" DataFormatString="{0:N2}" />
            <asp:BoundField DataField="LastUpdated" HeaderText="Last Updated" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
        </Columns>
    </asp:GridView>

    <!-- Recent Purchases -->
    <h5 class="mt-4">Recent Purchases</h5>
    <asp:GridView ID="gvPurchases" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover">
        <Columns>
            <asp:BoundField DataField="InvoiceCode" HeaderText="Invoice" />
            <asp:BoundField DataField="SupplierName" HeaderText="Supplier" />
            <asp:BoundField DataField="GrandTotal" HeaderText="Total" DataFormatString="{0:N2}" />
            <asp:BoundField DataField="CreatedAt" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
        </Columns>
    </asp:GridView>

</div>

<!-- Chart.js -->
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script>
    document.addEventListener("DOMContentLoaded", function () {
        // Stock vs Min Stock
        const stockCtx = document.getElementById('chartStock').getContext('2d');
        const stockChart = new Chart(stockCtx, {
            type: 'bar',
            data: {
                labels: <%= Newtonsoft.Json.JsonConvert.SerializeObject(StockNames) %>,
            datasets: [
                { label: 'Current Stock', data: <%= Newtonsoft.Json.JsonConvert.SerializeObject(CurrentStocks) %>, backgroundColor: 'rgba(54,162,235,0.7)' },
                { label: 'Minimum Stock', data: <%= Newtonsoft.Json.JsonConvert.SerializeObject(MinimumStocks) %>, backgroundColor: 'rgba(255,99,132,0.7)' }
            ]
        },
        options: { responsive: true, plugins: { legend: { position: 'top' } } }
    });

    // Purchase vs Sales
    const psCtx = document.getElementById('chartPurchaseSales').getContext('2d');
    const psChart = new Chart(psCtx, {
        type: 'line',
        data: {
            labels: <%= Newtonsoft.Json.JsonConvert.SerializeObject(Months) %>,
            datasets: [
                { label: 'Purchases', data: <%= Newtonsoft.Json.JsonConvert.SerializeObject(Purchases) %>, borderColor: 'blue', fill: false },
                { label: 'Sales', data: <%= Newtonsoft.Json.JsonConvert.SerializeObject(Sales) %>, borderColor: 'green', fill: false }
            ]
        },
        options: { responsive: true, plugins: { legend: { position: 'top' } } }
    });
});
</script>

<style>
.hover-scale:hover { transform: scale(1.05); transition: 0.3s ease-in-out; cursor: pointer; }
</style>


</asp:Content>
