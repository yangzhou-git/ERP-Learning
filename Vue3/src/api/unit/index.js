import request from "../../utils/request";

//查询单位列表
export function GetUnitListApi(params){
    return request.get('/Unit/GetUnitList', {
        params
    })
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