namespace ERP.Server.Models.Response
{
    public class GoodsListResponse
    {
        public int GoodsID { get; set; }
        public string BarCode { get; set; } = string.Empty;
        public string GoodsTitle { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal GoodsPrice { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
