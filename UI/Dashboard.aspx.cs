using System;
using System.Collections.Generic;
using System.Linq;
using SIMS_Akura.BLL;
using SIMS_Akura.Models;
using Newtonsoft.Json;

namespace SIMS_Akura.UI
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected List<string> StockNames { get; set; } = new List<string>();
        protected List<decimal> CurrentStocks { get; set; } = new List<decimal>();
        protected List<decimal> MinimumStocks { get; set; } = new List<decimal>();
        protected List<string> Months { get; set; } = new List<string>();
        protected List<decimal> Purchases { get; set; } = new List<decimal>();
        protected List<decimal> Sales { get; set; } = new List<decimal>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadKPIs();
                LoadStockTable();
                LoadRecentPurchases();
                PrepareChartData();
            }
        }

        private void LoadKPIs()
        {
            var stockBll = new StockBLL();
            var purchaseBll = new PurchaseBLL();

            lblStockValue.InnerText = stockBll.GetValuationSummary().ToString("N2");
            lblTotalPurchases.InnerText = purchaseBll.GetPurchaseHistory(null, null, null).Count.ToString();
            lblLowStock.InnerText = stockBll.GetOverview().Count(p => p.CurrentStock <= p.MinimumStock).ToString();
        }

        private void LoadStockTable()
        {
            gvStock.DataSource = new StockBLL().GetOverview();
            gvStock.DataBind();
        }

        private void LoadRecentPurchases()
        {
            gvPurchases.DataSource = new PurchaseBLL().GetPurchaseHistory(null, null, null)
                                                        .OrderByDescending(x => x.CreatedAt)
                                                        .Take(5)
                                                        .ToList();
            gvPurchases.DataBind();
        }

        private void PrepareChartData()
        {
            var stockList = new StockBLL().GetOverview();
            StockNames = stockList.Select(x => x.ProductName).ToList();
            CurrentStocks = stockList.Select(x => x.CurrentStock).ToList();
            MinimumStocks = stockList.Select(x => x.MinimumStock).ToList();

            Months = Enumerable.Range(1, 12)
                               .Select(m => new DateTime(DateTime.Now.Year, m, 1).ToString("MMM"))
                               .ToList();

            Purchases = new PurchaseBLL().GetPurchaseHistory(null, null, null)
                                         .GroupBy(x => x.CreatedAt.Month)
                                         .Select(g => g.Sum(x => x.GrandTotal))
                                         .ToList();

            
        }
    }
}
