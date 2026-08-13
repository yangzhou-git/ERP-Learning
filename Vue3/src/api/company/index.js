import request from '../../utils/request'

//查询公司列表
export function GetCompanyListApi(params){
    return request.get('/Company/GetCompanyList',{params})
}
// 查询启用公司列表(下拉框)
export function GetActiveCompanyListApi() {
    return request.get('/Company/GetActiveCompanyList')
}
//新增公司
export function AddCompanyApi(data){
    return request.post('/Company/AddCompany',data)
}
//修改公司
export function UpdateCompanyApi(data){
    return request.post('/Company/UpdateCompany',data)
}
//修改公司状态
export function UpdateCompanyStatusApi(data){
    return request.post('/Company/UpdateCompanyStatus',data)
}