import request from "../../utils/request";

//查询商品单位列表
export function GetGoodsUnitListApi(params) {
    return request.get('/GoodsUnit/GetGoodsUnitList', { params })
}
//新增商品单位关系
export function AddGoodsUnitApi(data) {
    return request.post('/GoodsUnit/AddGoodsUnit', data)
}
//修改商品单位关系状态
export function UpdateGoodsUnitStatusApi(data) {
    return request.post('/GoodsUnit/UpdateGoodsUnitStatus', data)
}
//修改商品单位关系
export function UpdateGoodsUnitApi(data) {
    return request.post('/GoodsUnit/UpdateGoodsUnit', data)
}