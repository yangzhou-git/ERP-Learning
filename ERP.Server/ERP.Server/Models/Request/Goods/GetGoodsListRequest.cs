namespace ERP.Server.Models.Request
{
    public class GetGoodsListRequest
    {
        public string? GoodsTitle { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
