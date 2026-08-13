import request from '../../utils/request'

//获取用户列表
export function GetUserListApi(params){
    return request.get('/User/GetUserList', {
    params
})
}
//新增员工资料
export function AddUserApi(data){
    return request.post('/User/AddUser', data)
}
//修改员工资料
export function UpdateUserApi(data){
    return request.post('/User/UpdateUser', data)
}
// 修改员工状态
export function UpdateUserStatusApi(data) {
    return request.post('/User/UpdateUserStatus', data)
}