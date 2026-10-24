import request from "@/utils/request"

//查询到货单
export function GetArrivalListApi(params) {
    return request.get("/Arrival/GetArrivalList", {params})
}
// 新增到货单
export function AddArrivalApi(data) {
    return request.post("/Arrival/AddArrival", data)
}