import request from "@/utils/request"

//查询到货单
export function GetArrivalListApi(params) {
    return request.get("/Arrival/GetArrivalList", {params})
}
// 新增到货单
export function AddArrivalApi(data) {
    return request.post("/Arrival/AddArrival", data)
}
//查询到货单详情
export function GetArrivalDetailApi(params) {
    return request.get("/Arrival/GetArrivalDetail", {params})
}
//审核到货单
export function UpdateArrivalAuditApi(data) {
    return request.post('/Arrival/UpdateArrivalAudit', data)
}
// 到货单入库
export function StockInArrivalApi(data) {
    return request.post('/Arrival/StockInArrival', data)
}
// 撤销到货单入库
export function CancelStockInArrivalApi(data) {
    return request.post('/Arrival/CancelStockInArrival', data)
}
// 反审核到货单
export function CancelArrivalAuditApi(data) {
    return request.post('/Arrival/CancelArrivalAudit', data)
}
// 修改到货单
export function UpdateArrivalApi(data) {
    return request.post('/Arrival/UpdateArrival', data)
}