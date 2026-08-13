using System.Data;
using System.Net;
using System.Numerics;
using Dapper;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;

namespace ERP.Server.Repositories.Implementations
{
    public class LocationRepository : ILocationRepository
    {
        private readonly IDbConnection _dbConnection;

        public LocationRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region 查询站点列表GetLocationListAsync
        public async Task<List<Location>> GetLocationListAsync(GetLocationListRequest request)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var sql = @"select l.LocationID, l.CompanyID, c.CompanyName, l.LocationCode, l.LocationName, l.ParentID, l.CanSale, l.CanStock, l.CanReceive, l.ProvinceID, l.CityID, l.DistrictID, l.Address, l.Contact, l.Phone, l.IsActive, l.CreateTime ,l.UpdateTime
                        from Location l
                        left join Company c on l.CompanyID = c.CompanyID
                        where (@LocationName is null or @LocationName = '' or l.LocationName like '%' + @LocationName + '%')
                          and (@CompanyID is null or l.CompanyID = @CompanyID)
                          and (@IsActive is null or l.IsActive = @IsActive)
                        order by l.LocationID desc
                        offset @Skip rows
                        fetch next @PageSize rows only";
            var result = await _dbConnection.QueryAsync<Location>(sql, new
            {
                LocationName = request.LocationName,
                CompanyID = request.CompanyID,
                IsActive = request.IsActive,
                Skip = skip,
                PageSize = request.PageSize
            });
            return result.ToList();
        }
        #endregion
        #region 查询站点数量GetLocationCountAsync
        public async Task<int> GetLocationCountAsync(GetLocationListRequest request)
        {
            var sql = @"select count(1) from Location where (@LocationName is null or @LocationName = '' or LocationName like '%' + @LocationName + '%')
                                                            and(@CompanyID is null or CompanyID=@CompanyID)
                                                            and(@IsActive is null or IsActive = @IsActive)";
            var result = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                LocationName = request.LocationName,
                CompanyID = request.CompanyID,
                IsActive = request.IsActive
            });
            return result;
        }
        #endregion
        #region 根据站点编码查询GetLocationByCodeAsync
        public async Task<Location?> GetLocationByCodeAsync(int CompanyID, string LocationCode)
        {
            var sql = @"select LocationID, CompanyID, LocationCode, LocationName, ParentID, CanSale, CanStock, CanReceive, ProvinceID, CityID, DistrictID, Address, Contact, Phone, IsActive, CreateTime, UpdateTime
                        from Location
                        where CompanyID = @CompanyID and LocationCode = @LocationCode";

            var result = await _dbConnection.QueryFirstOrDefaultAsync<Location>(sql, new
            {
                CompanyID,
                LocationCode
            });
            return result;
        }
        #endregion
        #region 查询站点树GetLocationTreeAsync
        public async Task<List<Location>> GetLocationTreeAsync()
        {
            var sql = @"select LocationID,LocationName,ParentID,IsActive
                        from Location
                        where IsActive = 1";
            var result = await _dbConnection.QueryAsync<Location>(sql);
            return result.ToList();
        }
        #endregion
        #region 根据站点ID查询GetLocationByIdAsync
        public async Task<Location?> GetLocationByIdAsync(int LocationID)
        {
            var sql = @"select LocationID,CompanyID,LocationCode,LocationName,ParentID,CanSale,CanStock,CanReceive,ProvinceID,CityID,DistrictID,Address,Contact,Phone,IsActive,CreateTime,UpdateTime 
                        from Location 
                        where LocationID = @LocationID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Location>(sql, new
            {
                LocationID
            });
            return result;
        }
        #endregion
        #region 新增站点AddLocationAsync
        public async Task<bool> AddLocationAsync(Location location)
        {
            var sql = @"insert into Location (CompanyID, LocationCode, LocationName, ParentID, CanSale, CanStock, CanReceive, ProvinceID, CityID, DistrictID, Address, Contact, Phone, IsActive, CreateTime)
                                      values (@CompanyID, @LocationCode, @LocationName, @ParentID, @CanSale, @CanStock, @CanReceive, @ProvinceID, @CityID, @DistrictID, @Address, @Contact, @Phone, @IsActive, getdate())";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                CompanyID = location.CompanyID,
                LocationCode = location.LocationCode,
                LocationName = location.LocationName,
                ParentID = location.ParentID,
                CanSale = location.CanSale,
                CanStock = location.CanStock,
                CanReceive = location.CanReceive,
                ProvinceID = location.ProvinceID,
                CityID = location.CityID,
                DistrictID = location.DistrictID,
                Address = location.Address,
                Contact = location.Contact,
                Phone = location.Phone,
                IsActive = location.IsActive
            });
            return result > 0;
        }
        #endregion
        #region 修改站点UpdateLocationAsync
        public async Task<bool> UpdateLocationAsync(Location location)
        {
            var sql = @"update Location set LocationName = @LocationName, LocationCode = @LocationCode, ParentID = @ParentID, CanSale = @CanSale, CanStock = @CanStock, CanReceive = @CanReceive, ProvinceID = @ProvinceID, CityID = @CityID, DistrictID = @DistrictID, Address = @Address, Contact = @Contact, Phone = @Phone, UpdateTime = getdate()
                      where LocationID = @LocationID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                LocationID = location.LocationID,
                LocationName = location.LocationName,
                LocationCode = location.LocationCode,
                ParentID = location.ParentID,
                CanSale = location.CanSale,
                CanStock = location.CanStock,
                CanReceive = location.CanReceive,
                ProvinceID = location.ProvinceID,
                CityID = location.CityID,
                DistrictID = location.DistrictID,
                Address = location.Address,
                Contact = location.Contact,
                Phone = location.Phone
            });
            return result > 0;
        }
        #endregion
        #region 修改站点状态UpdateLocationStatusAsync
        public async Task<bool> UpdateLocationStatusAsync(Location location)
        {
            var sql = @"update Location set IsActive = @IsActive, UpdateTime = getdate() where LocationID = @LocationID";
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                LocationID = location.LocationID,
                IsActive = location.IsActive
            });
            return result > 0;
        }
        #endregion
    }
}
