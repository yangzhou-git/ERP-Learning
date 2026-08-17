namespace ERP.Server.Models.Request
{
    public class UpdateUserRequest
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int CompanyID { get; set; }
    }
}