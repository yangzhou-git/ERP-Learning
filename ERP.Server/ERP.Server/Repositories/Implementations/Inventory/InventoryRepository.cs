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
            var sql = @"select * from Inventory with (updlock, holdlock, rowlock)
                        where CompanyID = @companyID
                        and LocationID = @locationID
                        and GoodsID = @goodsID
                        and SupplierID = @supplierID";
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
        public async Task<bool> UpdateInventoryAsync(Inventory inventory, int companyID, IDbTransaction transaction)
        {
            var sql = @"UPDATE Inventory
                        SET Quantity = @Quantity,
                            InventoryAmount = @InventoryAmount,
                            AverageCost = @AverageCost,
                            UpdateUserID = @UpdateUserID,
                            UpdateTime = GETDATE()
                        WHERE InventoryID = @InventoryID
                          AND CompanyID = @companyID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                inventory.InventoryID,
                inventory.Quantity,
                inventory.InventoryAmount,
                inventory.AverageCost,
                inventory.UpdateUserID,
                companyID
            }, transaction);
            return result == 1;
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
                         Remark, CreateUserID, ReversalOfTransactionID, CreateTime)
                        values
                        (@InventoryID, @CompanyID, @LocationID, @GoodsID, @SupplierID, @TransactionType, @SourceID, @SourceDetailID, @SourceCode,
                         @BeforeQuantity, @QuantityChange, @AfterQuantity,
                         @BeforeInventoryAmount, @AmountChange, @AfterInventoryAmount,
                         @UnitCost, @BeforeAverageCost, @AfterAverageCost,
                         @Remark, @CreateUserID, @ReversalOfTransactionID, getdate())";
            var result = await _dbConnection.ExecuteAsync(sql, new
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
                inventoryTransaction.CreateUserID,
                inventoryTransaction.ReversalOfTransactionID
            }, transaction);
            return result == 1;
        }
        #endregion

        #region 根据到货明细查询原始入库流水GetArrivalStockInTransactionAsync
        public async Task<InventoryTransaction?> GetArrivalStockInTransactionAsync(int arrivalID, int arrivalDetailID, int companyID, InventoryTransactionType transactionType, IDbTransaction transaction)
        {
            var sql = @"SELECT A.*
                        FROM InventoryTransaction AS A WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
                        WHERE A.TransactionType = @transactionType
                        AND A.SourceDetailID = @SourceDetailID
                        AND A.SourceID = @SourceID
                        AND A.CompanyID = @companyID
                        AND NOT EXISTS
                        (
                            SELECT 1
                            FROM InventoryTransaction AS B
                            WHERE B.ReversalOfTransactionID = A.InventoryTransactionID
                            AND B.CompanyID = A.CompanyID
                        )";
            return await _dbConnection.QuerySingleOrDefaultAsync<InventoryTransaction>(sql, new { transactionType, SourceDetailID = arrivalDetailID, SourceID = arrivalID, companyID }, transaction);
        }
        #endregion

        #region 查询指定库存原始入库后的后续流水GetSubsequentInventoryTransactionsAsync
        public async Task<List<InventoryTransaction>> GetSubsequentInventoryTransactionsAsync(int inventoryID, int companyID, int originalTransactionID, IDbTransaction transaction)
        {
            var sql = @"SELECT *
                        FROM InventoryTransaction AS A
                        WHERE A.InventoryID = @inventoryID
                          AND A.CompanyID = @companyID
                          AND A.InventoryTransactionID > @originalTransactionID
                        ORDER BY A.InventoryTransactionID ASC
                ";

            return (await _dbConnection.QueryAsync<InventoryTransaction>(sql, new { inventoryID, companyID, originalTransactionID }, transaction)).ToList();
        }
        #endregion

        #region 根据库存流水ID查询原始库存流水GetInventoryTransactionByIDAsync
        public async Task<InventoryTransaction?> GetInventoryTransactionByIDAsync(int inventoryTransactionID, int companyID, IDbTransaction transaction)
        {
            var sql = @"select * from InventoryTransaction where InventoryTransactionID = @inventoryTransactionID and CompanyID = @companyID";
            return await _dbConnection.QueryFirstOrDefaultAsync<InventoryTransaction>(sql, new { inventoryTransactionID, companyID }, transaction);
        }
        #endregion
    }
}
