import request from "../../utils/request";

// 单位管理页面：搜索 + 分页
export function GetUnitListApi(params){
    return request.get('/Unit/GetUnitList', { params })
}
// 业务下拉框：直接获取启用单位
export function GetUnitOptionsApi(){
    return request.get('/Unit/GetUnitOptions')
}
// 修改单位状态
export function UpdateUnitStatusApi(data){
    return request.post('/Unit/UpdateUnitStatus', data)
}
// 新增单位
export function AddUnitApi(data){
    return request.post('/Unit/AddUnit', data)
}
//修改单位
export function UpdateUnitApi(data){
    return request.post('/Unit/UpdateUnit', data)
}