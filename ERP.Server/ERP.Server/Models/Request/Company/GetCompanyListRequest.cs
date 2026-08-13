namespace ERP.Server.Models.Request
{
    public class GetCompanyListRequest
    {
        public string? CompanyName { get; set; }//公司名称
        public bool? IsActive { get; set; }//状态筛选
        public int Page { get; set; } = 1;//页码
        public int PageSize { get; set; } = 10;//每页数量
    }
}
