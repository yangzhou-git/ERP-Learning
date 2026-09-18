namespace ERP.Server.Models.Entity.Inventory
{
    public class Inventory
    {
        public int InventoryID { get; set; } // 库存ID
        public int CompanyID { get; set; } // 公司ID
        public int LocationID { get; set; } // 站点ID
        public int GoodsID { get; set; } // 商品ID
        public int SupplierID { get; set; } // 供应商ID
        public decimal Quantity { get; set; } // 基础单位库存数量
        public decimal InventoryAmount { get; set; } // 库存总金额
        public decimal AverageCost { get; set; } // 基础单位平均成本
        public int CreateUserID { get; set; } // 创建人ID
        public DateTime CreateTime { get; set; } // 创建时间
        public int? UpdateUserID { get; set; } // 修改人ID
        public DateTime? UpdateTime { get; set; } // 修改时间
    }
}
