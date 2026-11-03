namespace ERP.Server.Models.Response.Inventory
{
    /// <summary>
    /// 库存汇总列表返回模型
    /// 按站点 + 商品维度汇总
    /// </summary>
    public class InventorySummaryResponse
    {
        public int GoodsID { get; set; } // 商品ID
        public string GoodsTitle { get; set; } = string.Empty; // 商品名称
        public int LocationID { get; set; } // 站点ID
        public string LocationName { get; set; } = string.Empty; // 站点名称
        public decimal Quantity { get; set; } // 汇总库存数量（基础单位）
        public decimal TotalAmount { get; set; } // 汇总库存金额
        public string UnitName { get; set; } = string.Empty; // 商品基础单位名称
    }
}