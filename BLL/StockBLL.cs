using System.Collections.Generic;
using SIMS_Akura.DAL;
using SIMS_Akura.Models;

namespace SIMS_Akura.BLL
{
    public class StockBLL
    {
        private readonly StockDAL dal = new StockDAL();
        public bool SetProductActive(long productId, bool isActive, long userId)
        {
            // Call DAL method
            var dal = new StockDAL();
            return dal.SetProductActive(productId, isActive, userId);
        }

        public List<StockView> GetOverview() => dal.GetOverview();

        public List<StockBatch> GetBatches(long productId) => dal.GetBatchesByProduct(productId);

        public bool AdjustStock(StockAdjustment adj) => dal.AdjustStock(adj);

        public decimal GetValuationSummary() => dal.GetValuationSummary();
    }
}
