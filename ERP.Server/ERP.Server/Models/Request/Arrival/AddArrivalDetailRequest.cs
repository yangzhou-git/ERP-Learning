namespace ERP.Server.Models.Request.Arrival
{
    public class AddArrivalDetailRequest
    {
        public int GoodsUnitID { get; set; }//商品单位关系ID
        public decimal Quantity { get; set; }//到货数量
        public decimal Amount { get; set; }//到货金额
        public string? Remark { get; set; }//明细备注
    }
}
