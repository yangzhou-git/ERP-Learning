namespace ERP.Server.Models.Response.Inventory
{
    public class InventoryDetailResponse
    {
        public int InventoryID { get; set; }           // 库存ID
        public int SupplierID { get; set; }            // 供应商ID
        public string SupplierName { get; set; } = string.Empty; // 供应商名称
        public string GoodsTitle { get; set; } = string.Empty;   // 商品名称
        public decimal Quantity { get; set; }          // 当前库存数量（基础单位）
        public decimal InventoryAmount { get; set; }   // 当前库存金额
        public decimal AverageCost { get; set; }           // 移动加权平均单位成本
    }
}
