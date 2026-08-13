namespace ERP.Server.Models.Request
{
    public class UpdateUserRequest
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int CompanyID { get; set; }
    }
}
