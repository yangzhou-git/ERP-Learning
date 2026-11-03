using ERP.Server.Models.Common;

namespace ERP.Server.Models.Request.Inventory
{
    public class GetInventorySummaryListRequest : PageRequest
    {
        public string? GoodsTitle { get; set; }     // 商品名称
        public int? LocationID { get; set; }       // 站点ID
    }
}
