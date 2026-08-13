import request from "../../utils/request";

// 查询省市区树
export function GetAreaTreeApi(params){
    return request.get('/Area/GetAreaTree', {params})
}