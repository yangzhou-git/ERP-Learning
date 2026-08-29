namespace ERP.Server.Models.Request.GoodsUnit
{
    public class GetGoodsUnitListRequest
    {
        public int GoodsID { get; set; }//商品ID
        public string? UnitName { get; set; }//单位名称
        public bool? IsBaseUnit { get; set; }//是否基础单位
        public bool? IsActive { get; set; }//是否启用
        public int Page { get; set; } = 1;//当前页
        public int PageSize { get; set; } = 10;//每页条数
    }
}
