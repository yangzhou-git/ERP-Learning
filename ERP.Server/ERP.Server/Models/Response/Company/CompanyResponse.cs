namespace ERP.Server.Models.Response
{
    public class CompanyResponse
    {
        public int CompanyID { get; set; }//公司ID
        public string CompanyCode { get; set; } = string.Empty;//公司编码
        public string CompanyName { get; set; } = string.Empty;//公司名称
        public string? Contact { get; set; }//联系人
        public string? Phone { get; set; }//联系电话
        public bool IsActive { get; set; }//是否启用
        public DateTime CreateTime { get; set; }//创建时间
        public DateTime? UpdateTime { get; set; }//修改时间
    }
}
