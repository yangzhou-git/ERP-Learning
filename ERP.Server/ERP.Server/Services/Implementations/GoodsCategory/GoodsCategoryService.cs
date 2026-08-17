using ERP.Server.Common;
using ERP.Server.Models.Entity;
using ERP.Server.Models.Request;
using ERP.Server.Models.Response;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Services.Implementations
{
    public class GoodsCategoryService : IGoodsCategoryService
    {
        private readonly IGoodsCategoryRepository _goodsCategoryRepository;

        public GoodsCategoryService(IGoodsCategoryRepository goodsCategoryRepository)
        {
            _goodsCategoryRepository = goodsCategoryRepository;
        }

        #region 获取商品分类列表GetGoodsCategoryListAsync
        public async Task<ApiResponse<PageResponse<GoodsCategoryResponse>>> GetGoodsCategoryListAsync(GetGoodsCategoryListRequest request)
        {
            var categoryList = await _goodsCategoryRepository.GetGoodsCategoryListAsync(request);
            var totalCount = await _goodsCategoryRepository.GetGoodsCategoryCountAsync(request);
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
            var categoryList = await _goodsCategoryRepository.GetGoodsCategoryTreeAsync();
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
            var existingCategory = await _goodsCategoryRepository.GetGoodsCategoryByParentAndNameAsync(request.ParentID, request.CategoryName);
            if (existingCategory != null)
            {
                return ApiResponse<bool>.Error($"商品分类【{existingCategory.CategoryName}】已存在,请勿重复添加");
            }
            var existingCode = await _goodsCategoryRepository.GetGoodsCategoryByCodeAsync(request.CategoryCode);
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
            var result = await _goodsCategoryRepository.AddGoodsCategoryAsync(category);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "新增商品分类成功");
            }
            return ApiResponse<bool>.Error("新增商品分类失败");
        }
        #endregion
        #region 修改商品分类状态UpdateGoodsCategoryStatusAsync
        public async Task<ApiResponse<bool>> UpdateGoodsCategoryStatusAsync(UpdateGoodsCategoryStatusRequest request)
        {
            if (!request.IsActive)
            {
                var activeChildList = await _goodsCategoryRepository.GetActiveChildCategoryListAsync(request.CategoryID);

                if (activeChildList.Any())
                {
                    var childNames = string.Join("、", activeChildList.Select(x => x.CategoryName));

                    return ApiResponse<bool>.Error($"当前分类下仍存在启用的子分类【{childNames}】，请先停用这些子分类后再操作");
                }
            }
            if (request.IsActive)
            {
                var currentCategory = await _goodsCategoryRepository.GetGoodsCategoryByIdAsync(request.CategoryID);

                if (currentCategory == null)
                {
                    return ApiResponse<bool>.Error("当前商品分类不存在");
                }
                if (currentCategory.ParentID != null)
                {
                    var parentCategory = await _goodsCategoryRepository.GetGoodsCategoryByIdAsync(currentCategory.ParentID.Value);

                    if (parentCategory == null)
                    {
                        return ApiResponse<bool>.Error("当前分类的父级分类不存在，请检查分类数据");
                    }
                    if (!parentCategory.IsActive)
                    {
                        return ApiResponse<bool>.Error($"父级分类【{parentCategory.CategoryName}】当前处于停用状态，请先启用父级分类后再操作");
                    }
                }
            }
            var category = new GoodsCategory
            {
                CategoryID = request.CategoryID,
                IsActive = request.IsActive
            };
            var result = await _goodsCategoryRepository.UpdateGoodsCategoryStatusAsync(category);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改商品分类状态成功");
            }
            return ApiResponse<bool>.Error("修改商品分类状态失败");
        }
        #endregion
        #region 修改商品分类UpdateGoodsCategoryAsync
        public async Task<ApiResponse<bool>> UpdateGoodsCategoryAsync(UpdateGoodsCategoryRequest request)
        {
            if (request.ParentID == request.CategoryID)
            {
                return ApiResponse<bool>.Error("不能将当前分类设置为自己的父级分类，请重新选择父级分类");
            }
            if (request.ParentID != null)
            {
                var categoryList = await _goodsCategoryRepository.GetAllGoodsCategoryAsync();
                var isDescendant = IsDescendant(request.CategoryID, request.ParentID.Value, categoryList);
                if (isDescendant)
                {
                    return ApiResponse<bool>.Error("不能将当前分类移动到自己的子孙分类下，请重新选择父级分类");
                }
            }
            var existingCategory = await _goodsCategoryRepository.GetGoodsCategoryByParentAndNameAsync(request.ParentID, request.CategoryName);
            if (existingCategory != null && existingCategory.CategoryID != request.CategoryID)
            {
                return ApiResponse<bool>.Error($"商品分类【{request.CategoryName}】已存在，请勿重复修改");
            }
            var existingCode = await _goodsCategoryRepository.GetGoodsCategoryByCodeAsync(request.CategoryCode);
            if (existingCode != null && existingCode.CategoryID != request.CategoryID)
            {
                return ApiResponse<bool>.Error($"商品分类编码【{request.CategoryCode}】已存在");
            }
            var category = new GoodsCategory
            {
                CategoryID = request.CategoryID,
                ParentID = request.ParentID,
                CategoryName = request.CategoryName,
                CategoryCode = request.CategoryCode,
                Sort = request.Sort
            };
            var result = await _goodsCategoryRepository.UpdateGoodsCategoryAsync(category);
            if (result)
            {
                return ApiResponse<bool>.Success(true, "修改商品分类成功");
            }
            return ApiResponse<bool>.Error("修改商品分类失败");
        }
        #endregion
        #region 判断目标节点是否为当前分类的子孙节点IsDescendant
        private bool IsDescendant(int categoryID, int targetParentID, List<GoodsCategory> categoryList)
        {
            var children = categoryList.Where(x => x.ParentID == categoryID).ToList();
            foreach (var child in children)
            {
                if (child.CategoryID == targetParentID)
                {
                    return true;
                }
                if (IsDescendant(child.CategoryID, targetParentID, categoryList))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}