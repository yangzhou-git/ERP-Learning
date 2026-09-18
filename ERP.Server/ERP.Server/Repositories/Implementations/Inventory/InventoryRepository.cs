using System.Data;
using Dapper;
using ERP.Server.Models.Entity.Inventory;
using ERP.Server.Models.Entity.InventoryTransaction;
using ERP.Server.Models.Enums;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IDbConnection _dbConnection;

        public InventoryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 根据公司站点商品供应商查询库存GetInventoryAsync
        public async Task<Inventory?> GetInventoryAsync (int companyID, int locationID, int goodsID, int supplierID, IDbTransaction transaction)
        {
            var sql = @"select * from Inventory with (updlock, holdlock, rowlock) where CompanyID = @companyID and LocationID = @locationID and GoodsID = @goodsID and SupplierID = @supplierID";
            return await _dbConnection.QueryFirstOrDefaultAsync<Inventory>(sql, new { companyID, locationID, goodsID, supplierID }, transaction);
        }
        #endregion

        #region 新增库存AddInventoryAsync
        public async Task<int> AddInventoryAsync(Inventory inventory, IDbTransaction transaction)
        {
            var sql = @"insert into Inventory (CompanyID, LocationID, GoodsID, SupplierID, Quantity, InventoryAmount, AverageCost, CreateUserID, CreateTime) 
                        values (@CompanyID, @LocationID, @GoodsID, @SupplierID, @Quantity, @InventoryAmount, @AverageCost, @CreateUserID, getdate());
                        select cast(SCOPE_IDENTITY() as int)";
            return await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                inventory.CompanyID,
                inventory.LocationID,
                inventory.GoodsID,
                inventory.SupplierID,
                inventory.Quantity,
                inventory.InventoryAmount,
                inventory.AverageCost,
                inventory.CreateUserID
            }, transaction);
        }
        #endregion

        #region 更新库存UpdateInventoryAsync
        public async Task<bool> UpdateInventoryAsync(Inventory inventory, IDbTransaction transaction)
        {
            var sql = @"update Inventory set Quantity = @Quantity, InventoryAmount = @InventoryAmount, AverageCost = @AverageCost, UpdateUserID = @UpdateUserID, UpdateTime = getdate() where InventoryID = @InventoryID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                inventory.InventoryID,
                inventory.Quantity,
                inventory.InventoryAmount,
                inventory.AverageCost,
                inventory.UpdateUserID
            }, transaction);
            return result > 0;
        }
        #endregion

        #region 新增库存流水AddInventoryTransactionAsync
        public async Task<bool> AddInventoryTransactionAsync(InventoryTransaction inventoryTransaction, IDbTransaction transaction)
        {
            var sql = @"insert into InventoryTransaction 
                        (InventoryID, CompanyID, LocationID, GoodsID, SupplierID, TransactionType, SourceID, SourceDetailID, SourceCode, 
                         BeforeQuantity, QuantityChange, AfterQuantity, 
                         BeforeInventoryAmount, AmountChange, AfterInventoryAmount, 
                         UnitCost, BeforeAverageCost, AfterAverageCost, 
                         Remark, CreateUserID, CreateTime)
                        values
                        (@InventoryID, @CompanyID, @LocationID, @GoodsID, @SupplierID, @TransactionType, @SourceID, @SourceDetailID, @SourceCode,
                         @BeforeQuantity, @QuantityChange, @AfterQuantity,
                         @BeforeInventoryAmount, @AmountChange, @AfterInventoryAmount,
                         @UnitCost, @BeforeAverageCost, @AfterAverageCost,
                         @Remark, @CreateUserID, getdate())";
            return await _dbConnection.ExecuteAsync(sql, new
            {
                inventoryTransaction.InventoryID,
                inventoryTransaction.CompanyID,
                inventoryTransaction.LocationID,
                inventoryTransaction.GoodsID,
                inventoryTransaction.SupplierID,
                inventoryTransaction.TransactionType,
                inventoryTransaction.SourceID,
                inventoryTransaction.SourceDetailID,
                inventoryTransaction.SourceCode,
                inventoryTransaction.BeforeQuantity,
                inventoryTransaction.QuantityChange,
                inventoryTransaction.AfterQuantity,
                inventoryTransaction.BeforeInventoryAmount,
                inventoryTransaction.AmountChange,
                inventoryTransaction.AfterInventoryAmount,
                inventoryTransaction.UnitCost,
                inventoryTransaction.BeforeAverageCost,
                inventoryTransaction.AfterAverageCost,
                inventoryTransaction.Remark,
                inventoryTransaction.CreateUserID
            }, transaction) > 0;
        }
        #endregion

        #region 根据到货明细查询原始入库流水GetArrivalStockInTransactionAsync
        public async Task<InventoryTransaction?> GetArrivalStockInTransactionAsync(int arrivalID, int arrivalDetailID, InventoryTransactionType transactionType, IDbTransaction transaction)
        {
            var sql = @"select * from InventoryTransaction with (updlock, holdlock, rowlock) where TransactionType = @transactionType and SourceDetailID = @SourceDetailID and SourceID = @SourceID";
            return await _dbConnection.QueryFirstOrDefaultAsync<InventoryTransaction>(sql, new { transactionType, SourceDetailID = arrivalDetailID, SourceID = arrivalID }, transaction);
        }
        #endregion
    }
}
