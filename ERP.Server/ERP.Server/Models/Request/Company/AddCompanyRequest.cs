namespace ERP.Server.Models.Request
{
    public class AddCompanyRequest
    {
        public string CompanyCode { get; set; } = string.Empty;//公司编码
        public string CompanyName { get; set; } = string.Empty;//公司名称
        public string? Contact { get; set; }//联系人
        public string? Phone { get; set; }//联系电话
    }
}
