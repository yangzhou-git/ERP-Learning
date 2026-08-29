namespace ERP.Server.Models.Request.GoodsUnit
{
    public class UpdateGoodsUnitRequest
    {
        public int GoodsUnitID { get; set; }//商品单位关系ID
        public int UnitID { get; set; }//单位ID
        public decimal ConversionRate { get; set; }//换算率
        public int Sort { get; set; }//排序
    }
}
