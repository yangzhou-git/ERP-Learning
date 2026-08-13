using System.Data;
using System.Net;
using Dapper;
using ERP.Server.Common;
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
        public async Task<bool> AddSupplierAsync(Supplier supplier)
        {
            var Sql = @"insert into Supplier (SupplierName,Contact,Phone,Address,Remark) values (@SupplierName,@Contact,@Phone,@Address,@Remark)";
            var result = await _dbConnection.ExecuteAsync(Sql, new
            {
                SupplierName = supplier.SupplierName,
                Contact = supplier.Contact,
                Phone = supplier.Phone,
                Address = supplier.Address,
                Remark = supplier.Remark
            });
            return result > 0;
        }
        #endregion
        #region 查询供应商列表
        public async Task<List<Supplier>> GetSupplierListAsync(GetSupplierListRequest request)
        {
            int Skip = (request.Page - 1) * request.PageSize;
            var Sql = @"select * from Supplier where (@SupplierName is null or @SupplierName = '' or SupplierName like '%' + @SupplierName + '%' ) order by SupplierID desc offset @Skip rows fetch next @PageSize rows only";
            var Result = await _dbConnection.QueryAsync<Supplier>(Sql, new
            {
                SupplierName = request.SupplierName,
                Skip,
                PageSize = request.PageSize
            });
            return Result.ToList();
        }
        #endregion
        #region 查询供应商总数 GetSupplierCountAsync
        public async Task<int> GetSupplierCountAsync (GetSupplierListRequest request)
        {
            var Sql = @"select count(1) from Supplier where (@SupplierName is null or @SupplierName = ''or SupplierName like '%' + @SupplierName + '%')";
            int result = await _dbConnection.ExecuteScalarAsync<int>(Sql, new
            {
                SupplierName = request.SupplierName
            });
            return result;
        }
        #endregion
        #region 修改供应商状态UpdateSupplierStatusAsync
        public async Task<bool> UpdateSupplierStatusAsync(UpdateSupplierStatusRequest request)
        {
            var sql = @"update Supplier set IsActive = @IsActive ,UpdateTime = getdate() where SupplierID = @SupplierID";
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
            var sql = @"update Supplier set SupplierName = @SupplierName, Contact = @Contact, Phone = @Phone, Address = @Address, Remark = @Remark, UpdateTime = getdate() where SupplierID = @SupplierID";
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
