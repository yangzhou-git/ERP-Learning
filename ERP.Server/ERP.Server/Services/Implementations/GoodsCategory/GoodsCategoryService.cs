using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Implementations;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class GoodsCategoryService : IGoodsCategoryService
    {
        private readonly IGoodsCategoryRepository _GoodsCategoryRepository;

        public GoodsCategoryService(IGoodsCategoryRepository GoodsCategoryRepository)
        {
            _GoodsCategoryRepository = GoodsCategoryRepository;
        }
        #region 获取商品分类列表GetGoodsCategoryListAsync
        public async Task<ApiResponse<PageResponse<GoodsCategoryResponse>>> GetGoodsCategoryListAsync(GetGoodsCategoryListRequest request)
        {
            var categoryList = await _GoodsCategoryRepository.GetGoodsCategoryListAsync(request);
            var totalCount = await _GoodsCategoryRepository.GetGoodsCategoryCountAsync(request);
            var responseList = new List<GoodsCategoryResponse>();
            foreach (var category in categoryList)
            {
                var categoryResponse = new GoodsCategoryResponse
                {
                    CategoryID = category.CategoryID,
                    ParentID = category.ParentID,
                    CategoryName = category.CategoryName,
                    CategoryCode = category.CategoryCode,
                    Sort = category.Sort,
                    IsActive = category.IsActive,
                    CreateTime = category.CreateTime,
                    UpdateTime = category.UpdateTime
                };
                responseList.Add(categoryResponse);
            }
            var pageResponse = new PageResponse<GoodsCategoryResponse>
            {
                TotalCount = totalCount,
                List = responseList
            };
            return ApiResponse<PageResponse<GoodsCategoryResponse>>.Success(pageResponse,"查询成功");
        }
        #endregion
        #region 查询商品分类树GetGoodsCategoryTreeAsync
        public async Task<ApiResponse<List<GoodsCategoryTreeResponse>>> GetGoodsCategoryTreeAsync()
        {
            var categoryList = await _GoodsCategoryRepository.GetGoodsCategoryTreeAsync();
            var rootCategoryList = categoryList.Where(x => x.ParentID == null).ToList();
            var result = new List<GoodsCategoryTreeResponse>();
            foreach (var item in rootCategoryList)
            {
                var treeItem = BuildGoodsCategoryTree(item, categoryList);
                result.Add(treeItem);
            }
            return ApiResponse<List<GoodsCategoryTreeResponse>>.Success(result, "查询商品分类树成功");
        }
        #endregion
        #region 递归构建商品分类树BuildGoodsCategoryTree
        private GoodsCategoryTreeResponse BuildGoodsCategoryTree(GoodsCategory category, List<GoodsCategory> categoryList)
        {
            var treeItem = new GoodsCategoryTreeResponse
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName
            };
            var children = categoryList.Where(x => x.ParentID == category.CategoryID).ToList();
            foreach (var child in children)
            {
                var childTreeItem = BuildGoodsCategoryTree(child, categoryList);
                treeItem.Children.Add(childTreeItem);
            }
            return treeItem;
        }
        #endregion
        #region 新增商品分类AddGoodsCategoryAsync
        public async Task<ApiResponse<bool>> AddGoodsCategoryAsync(AddGoodsCategoryRequest request)
        {
            var existingCategory = await _GoodsCategoryRepository.GetGoodsCategoryByParentAndNameAsync(request.ParentID, request.CategoryName);
            if (existingCategory != null)
            {
                return ApiResponse<bool>.Error($"商品分类【{existingCategory.CategoryName}】已存在,请勿重复添加");
            }
            var existingCode = await _GoodsCategoryRepository.GetGoodsCategoryByCodeAsync(request.CategoryCode);
            if (existingCode != null)
            {
                return ApiResponse<bool>.Error($"商品分类编码【{request.CategoryCode}】已存在,请勿重复添加");
            }
            var category = new GoodsCategory()
            {
                ParentID = request.ParentID,
                CategoryName = request.CategoryName,
                CategoryCode = request.CategoryCode,
                Sort = request.Sort,
                IsActive = true
            };
            var result = await _GoodsCategoryRepository.AddGoodsCategoryAsync(category);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "新增商品分类成功");
            }
            return ApiResponse<bool>.Error("新增商品分类失败");
        }
        #endregion
        #region 修改商品分类UpdateGoodsCategoryAsync
        public  async Task<ApiResponse<bool>> UpdateGoodsCategoryAsync(UpdateGoodsCategoryRequest request)
        {
            var existingCategory = await _GoodsCategoryRepository.GetGoodsCategoryByParentAndNameAsync(request.ParentID, request.CategoryName);
            if (existingCategory != null && existingCategory.CategoryID != request.CategoryID)
            {
                return ApiResponse<bool>.Error( $"商品分类【{existingCategory.CategoryName}】已存在，请勿重复修改");
            }
            var category = new GoodsCategory()
            {
                CategoryID = request.CategoryID,
                ParentID = request.ParentID,
                CategoryName = request.CategoryName,
                CategoryCode = request.CategoryCode,
                Sort = request.Sort
            };
            var result = await _GoodsCategoryRepository.UpdateGoodsCategoryAsync(category);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改商品分类成功");
            }

            return ApiResponse<bool>.Error("修改商品分类失败");
        }
        #endregion
        #region 修改商品分类状态UpdateGoodsCategoryStatusAsync
        public async Task<ApiResponse<bool>> UpdateGoodsCategoryStatusAsync(UpdateGoodsCategoryStatusRequest request)
        {
            var category = new GoodsCategory()
            {
                IsActive = request.IsActive,
                CategoryID = request.CategoryID
            };
            var result = await _GoodsCategoryRepository.UpdateGoodsCategoryStatusAsync(category);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改商品分类状态成功");
            }
            return ApiResponse<bool>.Error("修改商品分类状态失败");
        }
        #endregion
    }
}