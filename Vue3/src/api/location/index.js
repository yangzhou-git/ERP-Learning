import request from "../../utils/request";

// 获取站点列表
export function GetLocationListApi(params){
    return request.get('/Location/GetLocationList', {params})
}
// 新增站点
export function AddLocationApi(data){
    return request.post('/Location/AddLocation', data)
}
// 修改站点
export function UpdateLocationApi(data){
    return request.post('/Location/UpdateLocation', data)
}
// 修改站点状态
export function UpdateLocationStatusApi(data){
    return request.post('/Location/UpdateLocationStatus', data)
}
// 查询站点树
export function GetLocationTreeApi(params){
    return request.get('/Location/GetLocationTree', {params})
}