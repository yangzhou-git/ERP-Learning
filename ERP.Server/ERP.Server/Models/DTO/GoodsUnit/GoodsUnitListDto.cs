namespace ERP.Server.Models.DTO.GoodsUnit
{
    public class GoodsUnitListDto
    {
        public int GoodsUnitID { get; set; }
        public int GoodsID { get; set; }
        public int UnitID { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string UnitCode { get; set; } = string.Empty;
        public decimal ConversionRate { get; set; }
        public bool IsBaseUnit { get; set; }
        public bool IsActive { get; set; }
        public int Sort { get; set; }
        public int CreateUserID { get; set; }
        public string CreateUserName { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public int? UpdateUserID { get; set; }
        public string? UpdateUserName { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
