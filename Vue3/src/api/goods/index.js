import request from '../../utils/request'

//获取商品资料列表
export function GetGoodsListApi(params){
    return request.get('/Goods/GetGoodsList',{
        params
    })
}
//新增商品资料
export function AddGoodsApi(data){
    return request.post('/Goods/AddGoods',data)
}
//修改商品状态
export function UpdateGoodsStatusApi(data){
    return request.post('/Goods/UpdateGoodsStatus',data)
}
//修改商品资料
export function UpdateGoodsApi(data){
    return request.post('/Goods/UpdateGoods',data)
}