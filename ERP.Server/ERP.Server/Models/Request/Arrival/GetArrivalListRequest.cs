using ERP.Server.Models.Enums;
using ERP.Server.Models.Common;

namespace ERP.Server.Models.Request.Arrival
{
    public class GetArrivalListRequest : PageRequest
    {
        public string? ArrivalCode { get; set; }//到货单号
        public int? LocationID { get; set; }//站点ID
        public ArrivalStatus? ArrivalStatus { get; set; }//到货状态
        public DateTime? StartDate { get; set; }//开始日期
        public DateTime? EndDate { get; set; }//结束日期
    }
}
