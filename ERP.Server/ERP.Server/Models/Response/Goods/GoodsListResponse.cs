namespace ERP.Server.Models.Response
{
    public class GoodsListResponse
    {
        public int GoodsID { get; set; }
        public string BarCode { get; set; } = string.Empty;
        public string GoodsCode { get; set; } = string.Empty;
        public string GoodsTitle { get; set; } = string.Empty;
        public string? GoodsSubTitle { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal GoodsPrice { get; set; }
        public bool IsActive { get; set; }
        public int CreateUserID { get; set; }
        public string CreateUserName { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public int? UpdateUserID { get; set; }
        public string? UpdateUserName { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}