namespace ERP.Server.Models.Response
{
    public class UserResponse
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; }
        public int CompanyID { get; set; }//所属公司ID
        public bool IsActive { get; set; }
    }
}
