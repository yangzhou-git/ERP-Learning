namespace ERP.Server.Common
{
    public class PageResponse<T>
    {
        public int TotalCount { get; set; }
        public List<T> List { get; set; } = new();
    }
}
