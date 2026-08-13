namespace ERP.Server.Models.Request
{
    public class UpdateUserStatusRequest
    {
        public int UserID { get; set; }
        public bool IsActive { get; set; }
    }
}
