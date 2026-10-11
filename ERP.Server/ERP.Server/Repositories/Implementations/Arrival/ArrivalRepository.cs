using System.Data;
using System.Formats.Asn1;
using Dapper;
using ERP.Server.Common;
using ERP.Server.Models.DTO.Arrival;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Enums;
using ERP.Server.Models.Request.Arrival;
using ERP.Server.Models.Response.Arrival;
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
                        gu.IsActive as GoodsUnitIsActive,
                        g.BarCode,
                        g.GoodsCode,
                        g.GoodsTitle,
                        g.GoodsSubTitle,
                        g.IsActive as GoodsIsActive,
                        u.UnitCode,
                        u.UnitName,
                        u.IsActive as UnitIsActive
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

        #region 根据供应商ID查询供应商GetSupplierByIDAsync
        public async Task<Supplier?> GetSupplierByIDAsync(int supplierID)
        {
            var sql = @"select * from Supplier where SupplierID = @supplierID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Supplier>(sql, new { supplierID });
            return result;
        }
        #endregion

        #region 根据站点ID查询站点GetLocationByIDAsync
        public async Task<Location?> GetLocationByIDAsync(int locationID)
        {
            var sql = @"select * from Location where LocationID = @locationID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Location>(sql, new { locationID });
            return result;
        }
        #endregion

        #region 根据到货单ID查询GetArrivalByIDAsync
        public async Task<Arrival?> GetArrivalByIDAsync(int arrivalID, int companyID)
        {
            var sql = @"select * from Arrival where ArrivalID = @arrivalID and CompanyID = @companyID";
            return await _dbConnection.QueryFirstOrDefaultAsync<Arrival>(sql, new { arrivalID, companyID });
        }
        #endregion

        #region 根据到货单ID查询待修改到货单GetArrivalForUpdateAsync
        public async Task<Arrival?> GetArrivalForUpdateAsync(int arrivalID, int companyID, IDbTransaction transaction)
        {
            var sql = @"select * from Arrival with (updlock, holdlock, rowlock) where ArrivalID = @arrivalID and CompanyID = @companyID";
            return await _dbConnection.QueryFirstOrDefaultAsync<Arrival>(sql, new { arrivalID, companyID }, transaction);
        }
        #endregion

        #region 根据到货单ID查询到货明细GetArrivalDetailsByArrivalIDAsync
        public async Task<List<ArrivalDetail>> GetArrivalDetailsByArrivalIDAsync(int arrivalID, IDbTransaction transaction)
        {
            var sql = @"select * from ArrivalDetail where ArrivalID = @arrivalID";
            var result = await _dbConnection.QueryAsync<ArrivalDetail>(sql, new { arrivalID }, transaction);
            return result.ToList();
        }
        #endregion

        #region 查询到货单列表GetArrivalListAsync
        public async Task<List<ArrivalListResponse>> GetArrivalListAsync(GetArrivalListRequest request, int companyID)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var sql = @"select a.ArrivalID, a.ArrivalCode, a.ArrivalStatus, s.SupplierName, l.LocationName, a.TotalAmount, a.ArrivalTime, u.UserName as CreateUserName, a.CreateTime
                        from Arrival a
                        join Supplier s on a.SupplierID = s.SupplierID
                        join Location l on a.LocationID = l.LocationID
                        join Users u on a.CreateUserID = u.UserID
                        where a.CompanyID = @companyID
                        and (@ArrivalCode is null or @ArrivalCode = '' or a.ArrivalCode like '%' + @ArrivalCode + '%')
                        and (@LocationID is null or a.LocationID = @LocationID)
                        and (@ArrivalStatus is null or a.ArrivalStatus = @ArrivalStatus)
                        and (@StartDate is null or a.CreateTime >= @StartDate)
                        and (@EndDate is null or a.CreateTime < @EndDate)
                        order by a.CreateTime desc
                        offset @skip rows 
                        fetch next @pageSize rows only";
            var result = await _dbConnection.QueryAsync<ArrivalListResponse>(sql, new
            {
                companyID,
                request.ArrivalCode,
                request.LocationID,
                request.ArrivalStatus,
                request.StartDate,
                request.EndDate,
                skip,
                request.PageSize
            });
            return result.ToList();
        }
        #endregion

        #region 查询到货单总数GetArrivalCountAsync
        public async Task<int> GetArrivalCountAsync(GetArrivalListRequest request, int companyID)
        {
            var sql = @"select count(1)
                        from Arrival a
                        where a.CompanyID = @companyID
                        and (@ArrivalCode is null or @ArrivalCode = '' or a.ArrivalCode like '%' + @ArrivalCode + '%')
                        and (@LocationID is null or a.LocationID = @LocationID)
                        and (@ArrivalStatus is null or a.ArrivalStatus = @ArrivalStatus)
                        and (@StartDate is null or a.CreateTime >= @StartDate)
                        and (@EndDate is null or a.CreateTime < @EndDate)";
            var result = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                companyID,
                request.ArrivalCode,
                request.LocationID,
                request.ArrivalStatus,
                request.StartDate,
                request.EndDate
            });
            return result;
        }
        #endregion

        #region 查询到货单详情GetArrivalDetailAsync
        public async Task<ArrivalDetailResponse?> GetArrivalDetailAsync(int arrivalID, int companyID)
        {
            var sql = @"select
                        a.ArrivalID,
                        a.ArrivalCode,
                        a.ArrivalStatus,
                        a.CompanyID,
                        c.CompanyName,
                        a.SupplierID,
                        s.SupplierName,
                        a.LocationID,
                        l.LocationName,
                        a.ArrivalTime,
                        (select count(distinct ad.GoodsID)
                         from ArrivalDetail ad
                         where ad.ArrivalID = a.ArrivalID)
                        as GoodsCount,
                        (select isnull(sum(ad.Quantity), 0)
                         from ArrivalDetail ad
                         where ad.ArrivalID = a.ArrivalID)
                         as TotalQuantity,
                        a.TotalAmount,
                        a.Remark,
                        createUser.UserName as CreateUserName,
                        a.CreateTime,
                        auditUser.UserName as AuditUserName,
                        a.AuditTime,
                        stockInUser.UserName as StockInUserName,
                        a.StockInTime,
                        voidUser.UserName as VoidUserName,
                        a.VoidTime
                    from Arrival a
                    join Company c on a.CompanyID = c.CompanyID
                    join Supplier s on a.SupplierID = s.SupplierID
                    join Location l on a.LocationID = l.LocationID
                    join Users createUser on a.CreateUserID = createUser.UserID
                    left join Users auditUser on a.AuditUserID = auditUser.UserID
                    left join Users stockInUser on a.StockInUserID = stockInUser.UserID
                    left join Users voidUser on a.VoidUserID = voidUser.UserID
                    where a.ArrivalID = @arrivalID and a.CompanyID = @companyID;";
            return await _dbConnection.QueryFirstOrDefaultAsync<ArrivalDetailResponse>(sql, new
            {
                arrivalID,
                companyID
            });
        }
        #endregion

        #region 查询到货单明细GetArrivalDetailItemsAsync
        public async Task<List<ArrivalDetailItemResponse>> GetArrivalDetailItemsAsync(GetArrivalDetailRequest request, int companyID, int locationID)
        {
            var skip = (request.Page - 1) * request.PageSize;

            var sql = @"select
                        ad.BarCode,
                        ad.GoodsCode,
                        ad.GoodsTitle,
                        ad.GoodsSubTitle,
                        ad.UnitName,
                        ad.ConversionRate,
                        ad.Quantity,
                        ad.BaseQuantity,
                        g.GoodsPrice,
                        ad.UnitCost,
                        ad.Amount,
                        isnull(i.Quantity, 0) as CurrentInventory,
                        ad.Remark
                    from ArrivalDetail ad
                    join Arrival a on ad.ArrivalID = a.ArrivalID
                    join Goods g on ad.GoodsID = g.GoodsID
                    left join Inventory i
                        on i.CompanyID = @companyID
                        and i.LocationID = @locationID
                        and i.GoodsID = ad.GoodsID
                    where ad.ArrivalID = @ArrivalID
                        and a.CompanyID = @companyID
                    order by ad.ArrivalDetailID
                    offset @skip rows
                    fetch next @PageSize rows only;";
            var result = await _dbConnection.QueryAsync<ArrivalDetailItemResponse>(sql, new
            {
                request.ArrivalID,
                companyID,
                locationID,
                skip,
                request.PageSize
            });
            return result.ToList();
        }
        #endregion

        #region 查询到货单明细数量GetArrivalDetailItemCountAsync
        public async Task<int> GetArrivalDetailItemCountAsync(int arrivalID, int companyID)
        {
            var sql = @"select count(1)
                from ArrivalDetail ad
                join Arrival a on ad.ArrivalID = a.ArrivalID
                where ad.ArrivalID = @arrivalID
                and a.CompanyID = @companyID";
            return await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                arrivalID,
                companyID
            });
        }
        #endregion

        #region 审核到货单UpdateArrivalAuditAsync
        public async Task<bool> UpdateArrivalAuditAsync(int arrivalID, int companyID, int auditUserID, ArrivalStatus arrivalStatus)
        {
            var sql = @"update Arrival set ArrivalStatus = @arrivalStatus, AuditUserID = @auditUserID, AuditTime = getdate() where ArrivalID = @arrivalID and CompanyID = @companyID and ArrivalStatus = @pendingStatus";
            return await _dbConnection.ExecuteAsync(sql, new
            {
                arrivalID,
                auditUserID,
                arrivalStatus,
                companyID,
                pendingStatus = ArrivalStatus.Pending
            }) > 0;
        }
        #endregion

        #region 审核到货单UpdateArrivalAuditAsync
        public async Task<bool> UpdateArrivalAuditAsync(int arrivalID, int companyID, int auditUserID, ArrivalStatus currentStatus, ArrivalStatus targetStatus)
        {
            var sql = @"update Arrival set ArrivalStatus = @targetStatus, AuditUserID = @auditUserID, AuditTime = getdate() where ArrivalID = @arrivalID and CompanyID = @companyID and ArrivalStatus = @currentStatus";
            return await _dbConnection.ExecuteAsync(sql, new
            {
                arrivalID,
                auditUserID,
                targetStatus,
                companyID,
                currentStatus
            }) > 0;
        }
        #endregion

        #region 到货单入库UpdateArrivalStockInAsync
        public async Task<bool> UpdateArrivalStockInAsync(int arrivalID, int companyID, int stockInUserID, ArrivalStatus currentStatus, ArrivalStatus targetStatus, IDbTransaction transaction)
        {
            var sql = @"update Arrival set ArrivalStatus = @targetStatus, StockInUserID = @stockInUserID, StockInTime = getdate() where ArrivalID = @arrivalID and CompanyID = @companyID and ArrivalStatus = @currentStatus";
            return await _dbConnection.ExecuteAsync(sql, new
            {
                arrivalID,
                stockInUserID,
                targetStatus,
                companyID,
                currentStatus
            }, transaction) > 0;
        }
        #endregion

        #region 撤销到货单入库UpdateArrivalCancelStockInAsync
        public async Task<bool> UpdateArrivalCancelStockInAsync(int arrivalID, int companyID, int cancelStockInUserID, ArrivalStatus currentStatus, ArrivalStatus targetStatus, IDbTransaction transaction)
        {
            var sql = @"update Arrival set ArrivalStatus = @targetStatus, CancelStockInUserID = @cancelStockInUserID, CancelStockInTime = getdate(), UpdateUserID = @cancelStockInUserID, UpdateTime = getdate() where ArrivalID = @arrivalID and CompanyID = @companyID and ArrivalStatus = @currentStatus";
            return await _dbConnection.ExecuteAsync(sql, new
            {
                arrivalID,
                companyID,
                cancelStockInUserID,
                currentStatus,
                targetStatus
            }, transaction) > 0;
        }
        #endregion

        #region 反审核到货单UpdateArrivalCancelAuditAsync
        public async Task<bool> UpdateArrivalCancelAuditAsync(int arrivalID, int companyID, int cancelAuditUserID, ArrivalStatus currentStatus, ArrivalStatus targetStatus)
        {
            var sql = @"update Arrival set ArrivalStatus = @targetStatus, CancelAuditUserID = @cancelAuditUserID, CancelAuditTime = getdate(), UpdateUserID = @cancelAuditUserID, UpdateTime = getdate() where ArrivalID = @arrivalID and CompanyID = @companyID and ArrivalStatus = @currentStatus";
            return await _dbConnection.ExecuteAsync(sql, new
            {
                arrivalID,
                companyID,
                cancelAuditUserID,
                currentStatus,
                targetStatus
            }) > 0;
        }
        #endregion

        #region 修改到货单UpdateArrivalAsync
        public async Task<bool> UpdateArrivalAsync(Arrival arrival, int companyID, ArrivalStatus currentStatus, IDbTransaction transaction)
        {
            var sql = @"update Arrival
                set SupplierID = @SupplierID,
                    LocationID = @LocationID,
                    ArrivalTime = @ArrivalTime,
                    TotalAmount = @TotalAmount,
                    Remark = @Remark,
                    UpdateUserID = @UpdateUserID,
                    UpdateTime = getdate()
                where ArrivalID = @ArrivalID
                  and CompanyID = @companyID
                  and ArrivalStatus = @currentStatus";
            return await _dbConnection.ExecuteAsync(sql, new
            {
                arrival.SupplierID,
                arrival.LocationID,
                arrival.ArrivalTime,
                arrival.TotalAmount,
                arrival.Remark,
                arrival.UpdateUserID,
                arrival.ArrivalID,
                companyID,
                currentStatus
            },transaction) > 0;
        }
        #endregion

        #region 删除到货明细DeleteArrivalDetailsAsync
        public async Task<bool> DeleteArrivalDetailsAsync(int arrivalID, int companyID, IDbTransaction transaction)
        {
            var sql = @"delete from ArrivalDetail where ArrivalID = @arrivalID and exists(select 1 from Arrival where ArrivalID = @arrivalID and CompanyID = @companyID)";
            return await _dbConnection.ExecuteAsync(sql, new { arrivalID, companyID },transaction) > 0;
        }
        #endregion
    }
}
