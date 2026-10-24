using System.Data;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;

namespace ERP.Server.Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        #region 新增供应商AddSupplierAsync
        Task<bool> AddSupplierAsync(Supplier supplier, IDbTransaction transaction);
        #endregion
        #region 查询供应商列表GetSupplierListAsync
        Task<List<Supplier>> GetSupplierListAsync(GetSupplierListRequest request);
        #endregion
        #region 查询供应商数量GetSupplierCountAsync
        Task<int> GetSupplierCountAsync(GetSupplierListRequest request);
        #endregion
        #region 修改供应商状态UpdateSupplierStatusAsync
        Task<bool> UpdateSupplierStatusAsync(UpdateSupplierStatusRequest request);
        #endregion
        #region 修改供应商UpdateSupplierAsync
        Task<bool> UpdateSupplierAsync(UpdateSupplierRequest request);
        #endregion
    }
}