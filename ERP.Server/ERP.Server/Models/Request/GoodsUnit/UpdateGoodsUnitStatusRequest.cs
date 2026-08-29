namespace ERP.Server.Models.Request.GoodsUnit
{
    public class UpdateGoodsUnitStatusRequest
    {
        public int GoodsUnitID { get; set; }//商品单位关系ID
        public bool IsActive { get; set; }//是否启用
    }
}
