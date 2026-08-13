namespace ERP.Server.Models.Request
{
    public class GetGoodsListRequest
    {
        public string? GoodsTitle { get; set; }
        public int Page {  get; set; }
        public int PageSize { get; set; }
    }
}
