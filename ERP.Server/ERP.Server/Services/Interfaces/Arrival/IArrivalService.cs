using ERP.Server.Common;
using ERP.Server.Models.Request.Arrival;

namespace ERP.Server.Services.Interfaces.Arrival
{
    public interface IArrivalService
    {
        #region 新增到货单AddArrivalAsync
        Task<ApiResponse<bool>> AddArrivalAsync(AddArrivalRequest request);
        #endregion
    }
}
