namespace ERP.Server.Models.Response
{
    public class GoodsCategoryTreeResponse
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<GoodsCategoryTreeResponse> Children { get; set; } = new();
    }
}
