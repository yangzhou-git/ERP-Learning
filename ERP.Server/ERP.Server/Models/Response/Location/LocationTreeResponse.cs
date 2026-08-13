namespace ERP.Server.Models.Response
{
    public class LocationTreeResponse
    {
            public int LocationID { get; set; }
            public string LocationName { get; set; }
            public int? ParentID { get; set; }
            public List<LocationTreeResponse> Children { get; set; } = new();
    }
}
