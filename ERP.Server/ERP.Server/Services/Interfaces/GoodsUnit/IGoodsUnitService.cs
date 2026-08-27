using ERP.Server.Common;
using ERP.Server.Models.Request;

namespace ERP.Server.Services.Interfaces
{
    public interface IGoodsUnitService
    {
        #region 新增商品单位关系AddGoodsUnitAsync
        Task<ApiResponse<bool>> AddGoodsUnitAsync(AddGoodsUnitRequest request);
        #endregion
    }
}
