namespace ERP.Server.Models.Request
{
    public class UpdateLocationStatusRequest
    {
        public int LocationID { get; set; }//站点ID
        public bool IsActive { get; set; }//是否启用
    }
}
