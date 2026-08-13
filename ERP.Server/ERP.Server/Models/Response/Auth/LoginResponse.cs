namespace ERP.Server.Models.Response
{
    public class LoginResponse
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public int CompanyID { get; set; }
    }
}
