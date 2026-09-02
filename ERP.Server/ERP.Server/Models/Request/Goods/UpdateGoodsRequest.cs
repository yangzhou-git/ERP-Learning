namespace ERP.Server.Models.Request
{
    public class UpdateGoodsRequest
    {
        public int GoodsID { get; set; }
        public string BarCode { get; set; } = string.Empty;
        public string GoodsCode { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public string GoodsTitle { get; set; } = string.Empty;
        public string? GoodsSubTitle { get; set; }
        public decimal GoodsPrice { get; set; }
    }
}
