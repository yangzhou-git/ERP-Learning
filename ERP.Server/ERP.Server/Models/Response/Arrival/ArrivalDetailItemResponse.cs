namespace ERP.Server.Models.Response.Arrival
{
    public class ArrivalDetailItemResponse
    {
        public int ArrivalDetailID { get; set; } // 到货明细ID
        public int GoodsID { get; set; } // 商品ID
        public int GoodsUnitID { get; set; } // 商品单位关系ID
        public string? BarCode { get; set; } // 商品条码
        public string? GoodsCode { get; set; } // 商品编码
        public string GoodsTitle { get; set; } = string.Empty; // 商品名称
        public string? GoodsSubTitle { get; set; } // 商品副标题
        public string UnitName { get; set; } = string.Empty; // 单位名称
        public decimal ConversionRate { get; set; } // 换算率
        public decimal Quantity { get; set; } // 到货数量
        public decimal BaseQuantity { get; set; } // 到货基本数量
        public decimal GoodsPrice { get; set; } // 当前定价
        public decimal PurchaseUnitCost { get; set; } // 本次到货采购单位单价
        public decimal UnitCost { get; set; } // 到货单价
        public decimal Amount { get; set; } // 到货金额
        public decimal CurrentInventory { get; set; } // 当前库存
        public string? Remark { get; set; } // 备注
    }
}
