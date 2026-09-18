namespace ERP.Server.Models.Request.Arrival
{
    public class UpdateArrivalDetailRequest
    {
        public int GoodsUnitID { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string? Remark { get; set; }
    }
}
