using ERP.Server.Models.Common;

namespace ERP.Server.Models.Request.Arrival
{
    public class GetArrivalDetailRequest : PageRequest
    {
        public int ArrivalID { get; set; }
    }
}
