namespace ERP.Server.Models.Request
{
    public class GetUnitListRequest
    {
        public string? UnitName { get; set; }
        public string? UnitCode { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
