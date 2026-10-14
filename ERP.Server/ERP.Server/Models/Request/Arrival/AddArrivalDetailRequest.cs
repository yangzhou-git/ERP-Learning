namespace ERP.Server.Models.Request.Arrival
{
    public class AddArrivalDetailRequest
    {
        public int GoodsUnitID { get; set; }//商品单位关系ID
        public decimal Quantity { get; set; }//到货数量
        public decimal PurchaseUnitCost { get; set; } // 当前到货单位采购单价，例如48元/箱，不是基础单位成本
        public decimal Amount { get; set; }// 本条到货明细最终采购总金额，例如240元
        public string? Remark { get; set; }//明细备注
    }
}
