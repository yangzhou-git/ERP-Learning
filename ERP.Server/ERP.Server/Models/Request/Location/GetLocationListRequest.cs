namespace ERP.Server.Models.Request
{
    public class GetLocationListRequest
    {
        public string? LocationName { get; set; }//站点名称
        public int? CompanyID { get; set; }//公司ID
        public bool? IsActive { get; set; }//是否启用
        public int Page { get; set; } = 1;//页码
        public int PageSize { get; set; } = 10;//每页数量
    }
}
