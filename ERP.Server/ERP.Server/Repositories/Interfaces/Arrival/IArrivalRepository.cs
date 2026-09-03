using System.Data;
using ERP.Server.Models.DTO.Arrival;
using ERP.Server.Models.Entity;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IArrivalRepository
    {
        #region 新增到货单AddArrivalAsync
        Task<int> AddArrivalAsync(Arrival arrival, IDbTransaction transaction);
        #endregion
        #region 新增到货明细AddArrivalDetailsAsync
        Task<bool> AddArrivalDetailsAsync(List<ArrivalDetail> arrivalDetails, IDbTransaction transaction);
        #endregion
        #region 根据商品单位关系ID查询GetGoodsUnitByIDAsync
        Task<ArrivalGoodsUnitDto?> GetGoodsUnitByIDAsync(int goodsUnitID);
        #endregion
        #region 查询当天最大到货单编码GetMaxArrivalCodeAsync
        Task<string?> GetMaxArrivalCodeAsync(string prefix, IDbTransaction transaction);
        #endregion
    }
}
