using System.Data;
using Dapper;
using ERP.Server.Models.DTO.Arrival;
using ERP.Server.Models.Entity;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class ArrivalRepository : IArrivalRepository
    {
        private readonly IDbConnection _dbConnection;

        public ArrivalRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 新增到货单AddArrivalAsync
        public async Task<int> AddArrivalAsync(Arrival arrival, IDbTransaction transaction)
        {
            var sql = @"insert into Arrival 
            (ArrivalCode, SupplierID, CompanyID, LocationID, ArrivalStatus, ArrivalTime, TotalAmount, Remark, CreateUserID, CreateTime)
            output inserted.ArrivalID
            values 
            (@ArrivalCode, @SupplierID, @CompanyID, @LocationID, @ArrivalStatus, @ArrivalTime, @TotalAmount, @Remark, @CreateUserID, getdate())";
            var result = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                ArrivalCode = arrival.ArrivalCode,
                SupplierID = arrival.SupplierID,
                CompanyID = arrival.CompanyID,
                LocationID = arrival.LocationID,
                ArrivalStatus = arrival.ArrivalStatus,
                ArrivalTime = arrival.ArrivalTime,
                TotalAmount = arrival.TotalAmount,
                Remark = arrival.Remark,
                CreateUserID = arrival.CreateUserID
            }, transaction);
            return result;
        }
        #endregion

        #region 新增到货明细AddArrivalDetailsAsync
        public async Task<bool> AddArrivalDetailsAsync(List<ArrivalDetail> arrivalDetails, IDbTransaction transaction)
        {
            var sql = @"insert into ArrivalDetail
                        (ArrivalID, GoodsID, GoodsUnitID, Quantity, UnitCost, Amount, Remark, CreateUserID, CreateTime, BarCode, GoodsCode, GoodsTitle, GoodsSubTitle, UnitID, UnitCode, UnitName, ConversionRate, BaseQuantity)
                  values(@ArrivalID, @GoodsID, @GoodsUnitID, @Quantity, @UnitCost, @Amount, @Remark, @CreateUserID, getdate(), @BarCode, @GoodsCode, @GoodsTitle, @GoodsSubTitle, @UnitID, @UnitCode, @UnitName, @ConversionRate, @BaseQuantity)";
            var parameters = arrivalDetails.Select(arrivalDetail => new
            {
                ArrivalID = arrivalDetail.ArrivalID,
                GoodsID = arrivalDetail.GoodsID,
                GoodsUnitID = arrivalDetail.GoodsUnitID,
                Quantity = arrivalDetail.Quantity,
                UnitCost = arrivalDetail.UnitCost,
                Amount = arrivalDetail.Amount,
                Remark = arrivalDetail.Remark,
                CreateUserID = arrivalDetail.CreateUserID,
                BarCode = arrivalDetail.BarCode,
                GoodsCode = arrivalDetail.GoodsCode,
                GoodsTitle = arrivalDetail.GoodsTitle,
                GoodsSubTitle = arrivalDetail.GoodsSubTitle,
                UnitID = arrivalDetail.UnitID,
                UnitCode = arrivalDetail.UnitCode,
                UnitName = arrivalDetail.UnitName,
                ConversionRate = arrivalDetail.ConversionRate,
                BaseQuantity = arrivalDetail.BaseQuantity
            });
            var result = await _dbConnection.ExecuteAsync(sql, parameters, transaction);
            return result == arrivalDetails.Count;
        }
        #endregion

        #region 根据商品单位关系ID查询GetGoodsUnitByIDAsync
        public async Task<ArrivalGoodsUnitDto?> GetGoodsUnitByIDAsync(int goodsUnitID)
        {
            var sql = @"select 
                        gu.GoodsUnitID,
                        gu.GoodsID,
                        gu.UnitID,
                        gu.ConversionRate,
                        gu.IsActive,
                        g.BarCode,
                        g.GoodsCode,
                        g.GoodsTitle,
                        g.GoodsSubTitle,
                        u.UnitCode,
                        u.UnitName
                    from GoodsUnit gu
                    join Goods g on gu.GoodsID = g.GoodsID
                    join Unit u on gu.UnitID = u.UnitID
                    where gu.GoodsUnitID = @goodsUnitID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<ArrivalGoodsUnitDto>(sql, new { goodsUnitID });
            return result;
        }
        #endregion

        #region 查询当天最大到货单编码GetMaxArrivalCodeAsync
        public async Task<string?> GetMaxArrivalCodeAsync(string prefix, IDbTransaction transaction)
        {
            var sql = @"select max(ArrivalCode) from Arrival with (updlock, holdlock) where ArrivalCode like @prefix + '%'";
            var result = await _dbConnection.ExecuteScalarAsync<string?>(sql, new { prefix }, transaction);
            return result;
        }
        #endregion
    }
}
