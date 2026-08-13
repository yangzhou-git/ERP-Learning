namespace ERP.Server.Models.Response
{
    public class LocationResponse
    {
        public int LocationID { get; set; }
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }//公司名称
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public int? ParentID { get; set; }
        public bool CanSale { get; set; }
        public bool CanStock { get; set; }
        public bool CanReceive { get; set; }
        public string? ProvinceID { get; set; }
        public string? CityID { get; set; }
        public string? DistrictID { get; set; }
        public string? Address { get; set; }
        public string? Contact { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
