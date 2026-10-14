using System.Data;
using ERP.Server.Common;
using ERP.Server.Models.Entity.BusinessCodeSeed;
using ERP.Server.Models.Enums;
using ERP.Server.Models.Request.BusinessCodeSeed;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;
using ERP.Server.Services.Interfaces.BusinessCode;
using FluentValidation;

namespace ERP.Server.Services.Implementations.BusinessCode
{
    public class BusinessCodeService : IBusinessCodeService
    {
        private readonly IBusinessCodeSeedRepository _businessCodeSeedRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDbConnection _dbConnection;
        private readonly IValidator<AddBusinessCodeSeedRequest> _addBusinessCodeSeedValidator;
        private readonly IValidator<UpdateBusinessCodeSeedRequest> _updateBusinessCodeSeedValidator;

        public BusinessCodeService(IBusinessCodeSeedRepository businessCodeSeedRepository, ICurrentUserService currentUserService, IValidator<AddBusinessCodeSeedRequest> addBusinessCodeSeedValidator, IValidator<UpdateBusinessCodeSeedRequest> updateBusinessCodeSeedValidator, IDbConnection dbConnection)
        {
            _businessCodeSeedRepository = businessCodeSeedRepository;
            _currentUserService = currentUserService;
            _addBusinessCodeSeedValidator = addBusinessCodeSeedValidator;
            _updateBusinessCodeSeedValidator = updateBusinessCodeSeedValidator;
            _dbConnection = dbConnection;
        }

        #region 获取下一个业务编号GetNextCodeAsync
        public async Task<ApiResponse<string?>> GetNextCodeAsync(BusinessCodeType businessCodeType, IDbTransaction transaction)
        {
            var companyID = _currentUserService.CompanyID;
            var businessCodeSeed = await _businessCodeSeedRepository.GetBusinessCodeSeedForUpdateAsync(companyID, businessCodeType, transaction);
            if (businessCodeSeed == null)
            {
                return ApiResponse<string?>.Error("当前公司未配置该业务编号规则,无法生成业务编号");
            }
            if(businessCodeSeed.ResetType == BusinessCodeResetType.Never)
            {
                businessCodeSeed.CurrentNumber++;
            } 
            else if (businessCodeSeed.ResetType == BusinessCodeResetType.Daily)
            {
                if (businessCodeSeed.NumberDate?.Date == DateTime.Today) 
                {
                    businessCodeSeed.CurrentNumber++;
                }
                else
                {
                    businessCodeSeed.CurrentNumber = 1;
                    businessCodeSeed.NumberDate = DateTime.Today;
                }
            }
            else
            {
                return ApiResponse<string?>.Error("业务编号重置类型不合法,无法生成业务编号");
            }
            businessCodeSeed.HasBeenUsed = true;
            businessCodeSeed.UpdateUserID = _currentUserService.UserID;
            var updateResult = await _businessCodeSeedRepository.UpdateBusinessCodeSeedAsync(companyID, businessCodeSeed, transaction);
            if (!updateResult)
            {
                return ApiResponse<string?>.Error("业务编号种子更新失败,无法生成业务编号");
            }
            string code;
            if (businessCodeSeed.ResetType == BusinessCodeResetType.Never)
            {
                code = $"{businessCodeSeed.Prefix}" +
                       $"{businessCodeSeed.CurrentNumber.ToString($"D{businessCodeSeed.NumberLength}")}";
            }
            else
            {
                code = $"{businessCodeSeed.Prefix}" +
                       $"{businessCodeSeed.NumberDate:yyyyMMdd}" +
                       $"{businessCodeSeed.CurrentNumber.ToString($"D{businessCodeSeed.NumberLength}")}";
            }
            return ApiResponse<string?>.Success(code, "业务编号生成成功");
        }
        #endregion

        #region 新增业务编号规则AddBusinessCodeSeedAsync
        public async Task<ApiResponse<bool>> AddBusinessCodeSeedAsync(AddBusinessCodeSeedRequest request)
        {
            var companyID = _currentUserService.CompanyID;
            var createUserID = _currentUserService.UserID;
            var validationResult = await _addBusinessCodeSeedValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ApiResponse<bool>.Error(validationResult.Errors[0].ErrorMessage);
            }
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            try{
                var businessCodeSeed = await _businessCodeSeedRepository.GetBusinessCodeSeedForUpdateAsync(companyID, request.BusinessType, transaction);
                if (businessCodeSeed != null)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("当前公司已配置该业务编号规则,无法重复添加");
                }
                var newBusinessCodeSeed = new BusinessCodeSeed
                {
                    CompanyID = companyID,
                    BusinessType = request.BusinessType,
                    Prefix = request.Prefix.Trim(),
                    NumberLength = request.NumberLength,
                    ResetType = request.ResetType,
                    CurrentNumber = 0,
                    NumberDate = null,
                    CreateUserID = createUserID
                };
                var result = await _businessCodeSeedRepository.AddBusinessCodeSeedAsync(newBusinessCodeSeed, transaction);
                if(!result)
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("新增业务编号规则失败");
                }
                transaction.Commit();
                return ApiResponse<bool>.Success(true, "新增业务编号规则成功");
            }
            catch
            {                
                transaction.Rollback();
                throw;
            }
        }
        #endregion

        #region 修改业务编号规则UpdateBusinessCodeSeedAsync
        public async Task<ApiResponse<bool>> UpdateBusinessCodeSeedAsync(UpdateBusinessCodeSeedRequest request)
        {
            var companyID = _currentUserService.CompanyID;
            var updateUserID = _currentUserService.UserID;
            var updateValidationResult = await _updateBusinessCodeSeedValidator.ValidateAsync(request);
            if(!updateValidationResult.IsValid)
            {
                return ApiResponse<bool>.Error(updateValidationResult.Errors[0].ErrorMessage);
            }
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            try 
            { 
                var originalSeed = await _businessCodeSeedRepository.GetBusinessCodeSeedByIDForUpdateAsync(request.BusinessCodeSeedID, companyID, transaction);
                if (originalSeed == null) 
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("当前公司未配置该业务编号规则,无法修改");
                }
                if (originalSeed.HasBeenUsed &&
                    (originalSeed.BusinessType != request.BusinessType ||
                     originalSeed.Prefix != request.Prefix.Trim() ||
                     originalSeed.NumberLength != request.NumberLength ||
                     originalSeed.ResetType != request.ResetType))
                {
                    transaction.Rollback();
                    return ApiResponse<bool>.Error("当前业务编号规则已被使用,无法修改关键配置");
                }
                if (originalSeed.BusinessType != request.BusinessType)
                {
                    var targetSeed = await _businessCodeSeedRepository.GetBusinessCodeSeedForUpdateAsync(companyID, request.BusinessType, transaction);
                    if (targetSeed != null)
                    {
                        transaction.Rollback();
                        return ApiResponse<bool>.Error("当前公司已配置该业务编号规则,无法修改");
                    }
                }
                originalSeed.BusinessType = request.BusinessType;
                originalSeed.Prefix = request.Prefix.Trim();
                originalSeed.NumberLength = request.NumberLength;
                originalSeed.ResetType = request.ResetType;
                originalSeed.UpdateUserID = updateUserID;
                var updateResult = await _businessCodeSeedRepository.UpdateBusinessCodeSeedConfigAsync(companyID ,originalSeed, transaction);
                if (!updateResult)
                {
                    return ApiResponse<bool>.Error("业务编号规则更新失败,无法修改");
                }
                transaction.Commit();
                return ApiResponse<bool>.Success(true, "业务编号规则修改成功");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        #endregion
    }
}
