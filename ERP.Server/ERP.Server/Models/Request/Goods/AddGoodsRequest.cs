namespace ERP.Server.Models.Request
{
    public class AddGoodsRequest
    {
        public string? BarCode { get; set; }
        public string? GoodsCode { get; set; }
        public int CategoryID { get; set; }
        public string GoodsTitle { get; set; } = string.Empty;
        public string? GoodsSubTitle { get; set; }
        public decimal GoodsPrice { get; set; }
        public string Unit { get; set; } = string.Empty;

    }
}
