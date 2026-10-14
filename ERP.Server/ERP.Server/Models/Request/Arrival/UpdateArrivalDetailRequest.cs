namespace ERP.Server.Models.Request.Arrival
{
    public class UpdateArrivalDetailRequest
    {
        public int GoodsUnitID { get; set; } // 商品单位关系ID
        public decimal Quantity { get; set; } // 本次到货数量，例如10箱
        public decimal PurchaseUnitCost { get; set; } // 当前到货单位采购单价，例如48元/箱
        public decimal Amount { get; set; } // 本条到货明细最终采购总金额，例如480元
        public string? Remark { get; set; } // 明细备注
    }
}
