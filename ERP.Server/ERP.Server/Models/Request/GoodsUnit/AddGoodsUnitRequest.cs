namespace ERP.Server.Models.Request
{
    public class AddGoodsUnitRequest
    {
        public int GoodsID { get; set; }//商品ID
        public int UnitID { get; set; }//单位ID
        public decimal ConversionRate { get; set; }//换算率
        public bool IsBaseUnit { get; set; }//是否基础单位
    }
}
