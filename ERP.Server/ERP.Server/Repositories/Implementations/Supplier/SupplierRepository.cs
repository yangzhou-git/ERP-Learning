using System.Data;
using Dapper;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly IDbConnection _dbConnection;

        public SupplierRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 新增供应商AddSupplierAsync
        public async Task<bool> AddSupplierAsync(Supplier supplier, IDbTransaction transaction)
        {
            var sql = @"insert into Supplier
                        (SupplierCode,SupplierName,Contact,Phone,Address,Remark,IsActive,CreateTime)
                        values
                        (@SupplierCode,@SupplierName,@Contact,@Phone,@Address,@Remark,1,getdate())";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                SupplierCode = supplier.SupplierCode,
                SupplierName = supplier.SupplierName,
                Contact = supplier.Contact,
                Phone = supplier.Phone,
                Address = supplier.Address,
                Remark = supplier.Remark
            }, transaction);
            return result > 0;
        }
        #endregion
        #region 查询供应商列表GetSupplierListAsync
        public async Task<List<Supplier>> GetSupplierListAsync(GetSupplierListRequest request)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var sql = @"select SupplierID,
                               SupplierCode,
                               SupplierName,
                               Contact,
                               Phone,
                               Address,
                               Remark,
                               IsActive,
                               CreateTime
                        from Supplier
                        where (@SupplierName is null
                               or @SupplierName = ''
                               or SupplierName like '%' + @SupplierName + '%')
                        order by SupplierID desc
                        offset @Skip rows
                        fetch next @PageSize rows only";
            var result = await _dbConnection.QueryAsync<Supplier>(sql, new
            {
                SupplierName = request.SupplierName,
                Skip = skip,
                PageSize = request.PageSize
            });
            return result.ToList();
        }
        #endregion
        #region 查询供应商总数GetSupplierCountAsync
        public async Task<int> GetSupplierCountAsync(GetSupplierListRequest request)
        {
            var sql = @"select count(1)
                        from Supplier
                        where (@SupplierName is null
                               or @SupplierName = ''
                               or SupplierName like '%' + @SupplierName + '%')";
            var result = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                SupplierName = request.SupplierName
            });
            return result;
        }
        #endregion
        #region 修改供应商状态UpdateSupplierStatusAsync
        public async Task<bool> UpdateSupplierStatusAsync(UpdateSupplierStatusRequest request)
        {
            var sql = @"update Supplier
                        set IsActive = @IsActive,
                            UpdateTime = getdate()
                        where SupplierID = @SupplierID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                IsActive = request.IsActive,
                SupplierID = request.SupplierID
            });
            return result > 0;
        }
        #endregion
        #region 修改供应商UpdateSupplierAsync
        public async Task<bool> UpdateSupplierAsync(UpdateSupplierRequest request)
        {
            var sql = @"update Supplier
                        set SupplierName = @SupplierName,
                            Contact = @Contact,
                            Phone = @Phone,
                            Address = @Address,
                            Remark = @Remark,
                            UpdateTime = getdate()
                        where SupplierID = @SupplierID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                SupplierID = request.SupplierID,
                SupplierName = request.SupplierName,
                Contact = request.Contact,
                Phone = request.Phone,
                Address = request.Address,
                Remark = request.Remark
            });
            return result > 0;
        }
        #endregion
    }
}