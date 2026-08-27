namespace ERP.Server.Models.Entity
{
    public class GoodsUnit
    {
        public int GoodsUnitID { get; set; }//商品单位关系ID
        public int GoodsID { get; set; }//商品ID
        public int UnitID { get; set; }//单位ID
        public decimal ConversionRate { get; set; }//换算率
        public bool IsBaseUnit { get; set; }//是否基础单位
        public bool IsActive { get; set; }//是否启用
        public int Sort { get; set; }//排序
        public int CreateUserID { get; set; }//创建人ID
        public DateTime CreateTime { get; set; }//创建时间
        public int? UpdateUserID { get; set; }//修改人ID
        public DateTime? UpdateTime { get; set; }//修改时间
    }
}
