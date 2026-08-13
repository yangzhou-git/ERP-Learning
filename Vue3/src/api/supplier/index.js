import request from "../../utils/request";

// 获取供应商列表
export function GetSupplierListApi(params){
    return request.get('/Supplier/GetSupplierList', {
        params
    })
}
//新增供应商
export function AddSupplierApi(data){
    return request.post('/Supplier/AddSupplier', data)
}
//修改供应商状态
export function UpdateSupplierStatusApi(data){
    return request.post('/Supplier/UpdateSupplierStatus',data)
}
//修改供应商
export function UpdateSupplierApi(data){
    return request.post('/Supplier/UpdateSupplier', data)
}