namespace ERP.Server.Models.Request
{
    public class UpdateCompanyStatusRequest
    {
        public int CompanyID { get; set; }//公司ID
        public bool IsActive { get; set; }//状态
    }
}
