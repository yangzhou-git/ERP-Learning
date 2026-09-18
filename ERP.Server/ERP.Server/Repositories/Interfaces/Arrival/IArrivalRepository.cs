using System.Data;
using ERP.Server.Models.DTO.Arrival;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Enums;
using ERP.Server.Models.Request.Arrival;
using ERP.Server.Models.Response.Arrival;

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
        #region 根据供应商ID查询供应商GetSupplierByIDAsync
        Task<Supplier?> GetSupplierByIDAsync(int supplierID);
        #endregion
        #region 根据站点ID查询站点GetLocationByIDAsync
        Task<Location?> GetLocationByIDAsync(int locationID);
        #endregion
        #region 根据到货单ID查询到货单GetArrivalByIDAsync
        Task<Arrival?> GetArrivalByIDAsync(int arrivalID, int companyID);
        #endregion
        #region 根据到货单ID查询待修改到货单GetArrivalForUpdateAsync
        Task<Arrival?> GetArrivalForUpdateAsync(int arrivalID,int companyID,IDbTransaction transaction);
        #endregion
        #region 根据到货单ID查询到货明细GetArrivalDetailsByArrivalIDAsync
        Task<List<ArrivalDetail>> GetArrivalDetailsByArrivalIDAsync(int arrivalID,IDbTransaction transaction);
        #endregion
        #region 查询到货单列表GetArrivalListAsync
        Task<List<ArrivalListResponse>> GetArrivalListAsync(GetArrivalListRequest request, int companyID);
        #endregion
        #region 查询到货单总数GetArrivalCountAsync
        Task<int> GetArrivalCountAsync(GetArrivalListRequest request, int companyID);
        #endregion
        #region 查询到货单详情GetArrivalDetailAsync
        Task<ArrivalDetailResponse?> GetArrivalDetailAsync(int arrivalID, int companyID);
        #endregion
        #region 查询到货单明细GetArrivalDetailItemsAsync
        Task<List<ArrivalDetailItemResponse>> GetArrivalDetailItemsAsync(GetArrivalDetailRequest request, int companyID, int locationID);
        #endregion
        #region 查询到货单明细数量GetArrivalDetailItemCountAsync
        Task<int> GetArrivalDetailItemCountAsync(int arrivalID, int companyID);
        #endregion
        #region 审核到货单UpdateArrivalAuditAsync
        Task<bool> UpdateArrivalAuditAsync(int arrivalID, int companyID, int auditUserID, ArrivalStatus currentStatus, ArrivalStatus targetStatus);
        #endregion
        #region 到货单入库UpdateArrivalStockInAsync
        Task<bool> UpdateArrivalStockInAsync(int arrivalID, int companyID, int stockInUserID, ArrivalStatus currentStatus , ArrivalStatus targetStatus , IDbTransaction transaction);
        #endregion
        #region 撤销到货单入库UpdateArrivalCancelStockInAsync
        Task<bool> UpdateArrivalCancelStockInAsync(int arrivalID, int companyID, int cancelStockInUserID, ArrivalStatus currentStatus, ArrivalStatus targetStatus, IDbTransaction transaction);
        #endregion
        #region 反审核到货单UpdateArrivalCancelAuditAsync
        Task<bool> UpdateArrivalCancelAuditAsync(int arrivalID, int companyID, int cancelAuditUserID, ArrivalStatus currentStatus, ArrivalStatus targetStatus);
        #endregion
        #region 修改到货单UpdateArrivalAsync
        Task<bool> UpdateArrivalAsync(Arrival arrival, int companyID, ArrivalStatus currentStatus, IDbTransaction transaction);
        #endregion
        #region 删除到货明细DeleteArrivalDetailsAsync
        Task<bool> DeleteArrivalDetailsAsync(int arrivalID, int companyID, IDbTransaction transaction);
        #endregion
    }
}
