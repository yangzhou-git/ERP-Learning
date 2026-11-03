using ERP.Server.Models.Common;

namespace ERP.Server.Models.Request.Inventory
{
    public class GetInventoryDetailListRequest : PageRequest
    {
        public int LocationID { get; set; } //站点ID
        public int GoodsID { get; set; } //商品ID
    }
}
