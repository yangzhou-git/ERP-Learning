namespace ERP.Server.Models.DTO.Arrival
{
    public class ArrivalGoodsUnitDto
    {
        public int GoodsUnitID { get; set; }//商品单位关系ID
        public int GoodsID { get; set; }//商品ID
        public int UnitID { get; set; }//单位ID
        public decimal ConversionRate { get; set; }//单位换算率
        public bool IsActive { get; set; }//商品单位关系是否启用

        public string? BarCode { get; set; }//商品条码
        public string? GoodsCode { get; set; }//商品编码
        public string GoodsTitle { get; set; } = string.Empty;//商品名称
        public string? GoodsSubTitle { get; set; }//商品副标题

        public string UnitCode { get; set; } = string.Empty;//单位编码
        public string UnitName { get; set; } = string.Empty;//单位名称
    }
}
