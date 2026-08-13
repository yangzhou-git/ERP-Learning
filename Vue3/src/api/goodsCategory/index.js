import request from "../../utils/request";

//查询商品分类列表
export function GetGoodsCategoryListApi(params) {
    return request.get('/GoodsCategory/GetGoodsCategoryList', { params })
}
//查询商品分类树形列表
export function GetGoodsCategoryTreeApi(params) {
    return request.get('/GoodsCategory/GetGoodsCategoryTree', { params })
}
//新增商品分类
export function AddGoodsCategoryApi(data) {
    return request.post('/GoodsCategory/AddGoodsCategory', data)
}
//修改商品分类
export function UpdateGoodsCategoryApi(data) {
    return request.post('/GoodsCategory/UpdateGoodsCategory', data)
}
//修改商品分类状态
export function UpdateGoodsCategoryStatusApi(data) {
    return request.post('/GoodsCategory/UpdateGoodsCategoryStatus', data)
}