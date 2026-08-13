namespace ERP.Server.Models.Request
{
    public class UpdateGoodsCategoryStatusRequest
    {
        public int CategoryID { get; set; }//分类ID
        public bool IsActive { get; set; }//状态
    }
}
