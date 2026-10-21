using System.Data;
using ERP.Server.Models.Entity.Inventory;
using ERP.Server.Models.Entity.InventoryTransaction;
using ERP.Server.Models.Enums;

namespace ERP.Server.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        #region 根据公司站点商品供应商查询库存GetInventoryAsync
        Task<Inventory?> GetInventoryAsync(int companyID, int locationID, int goodsID, int supplierID, IDbTransaction transaction);
        #endregion
        #region 新增库存AddInventoryAsync
        Task<int> AddInventoryAsync(Inventory inventory, IDbTransaction transaction);
        #endregion
        #region 更新库存UpdateInventoryAsync
        Task<bool> UpdateInventoryAsync(Inventory inventory, int companID, IDbTransaction transaction);
        #endregion
        #region 新增库存流水AddInventoryTransactionAsync
        Task<bool> AddInventoryTransactionAsync(InventoryTransaction inventoryTransaction, IDbTransaction transaction);
        #endregion
        #region 根据到货明细查询原始入库流水GetArrivalStockInTransactionAsync
        Task<InventoryTransaction?> GetArrivalStockInTransactionAsync(int arrivalID, int arrivalDetailID, int companyID, InventoryTransactionType transactionType, IDbTransaction transaction);
        #endregion
        #region 查询指定库存原始入库后的后续流水GetSubsequentInventoryTransactionsAsync
        Task<List<InventoryTransaction>> GetSubsequentInventoryTransactionsAsync(int inventoryID, int companyID, int originalTransactionID, IDbTransaction transaction);
        #endregion
        #region 根据库存流水ID查询原始库存流水GetInventoryTransactionByIDAsync
        Task<InventoryTransaction?> GetInventoryTransactionByIDAsync(int inventoryTransactionID, int companyID, IDbTransaction transaction);
        #endregion
    }
}
