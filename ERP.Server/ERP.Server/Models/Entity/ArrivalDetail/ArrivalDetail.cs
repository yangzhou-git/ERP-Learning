namespace ERP.Server.Models.Entity
{
    public class ArrivalDetail
    {
        public int ArrivalDetailID { get; set; }//到货明细ID
        public int ArrivalID { get; set; }//到货单ID
        public int GoodsID { get; set; }//商品ID
        public int GoodsUnitID { get; set; }//商品单位关系ID
        public decimal Quantity { get; set; } // 到货单位数量，例如10箱
        public decimal PurchaseUnitCost { get; set; } // 当前到货单位采购单价，例如48元/箱，来自Request
        public decimal UnitCost { get; set; } // 基础单位采购成本，例如2元/瓶，由Amount/BaseQuantity计算
        public decimal Amount { get; set; } // 本条到货明细最终采购总金额，例如480元
        public string? Remark { get; set; }//备注
        public int CreateUserID { get; set; }//创建人ID
        public DateTime CreateTime { get; set; }//创建时间
        public int? UpdateUserID { get; set; }//最后修改人ID
        public DateTime? UpdateTime { get; set; }//最后修改时间
        public string? BarCode { get; set; }//商品条码
        public string? GoodsCode { get; set; }//商品编码
        public string GoodsTitle { get; set; } = string.Empty;//商品名称
        public string? GoodsSubTitle { get; set; }//商品副标题
        public int UnitID { get; set; }//单位ID
        public string UnitCode { get; set; } = string.Empty;//单位编码
        public string UnitName { get; set; } = string.Empty;//单位名称
        public decimal ConversionRate { get; set; } // 当前到货单位换算率，例如1箱=24瓶
        public decimal BaseQuantity { get; set; } // 基础单位数量，例如10箱×24=240瓶
        public bool IsDeleted { get; set; } // 是否删除
    }
}
