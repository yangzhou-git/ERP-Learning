namespace ERP.Server.Models.Request
{
    public class UpdateGoodsStatusRequest
    {
        public int GoodsID { get; set; }
        public bool IsActive { get; set; }
    }
}
