namespace ERP.Server.Models.Request
{
    public class GetGoodsCategoryListRequest
    {
        public string? CategoryName { get; set; }//分类名称
        public bool? IsActive { get; set; }//状态
        public int Page { get; set; } = 1;//页码
        public int PageSize { get; set; } = 10;//每页数量
    }
}
