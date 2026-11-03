import request from "@/utils/request"

// 查询库存汇总分页列表
export function GetInventorySummaryListApi(params) {
    return request.get("/Inventory/GetInventorySummaryList", { params })
}
// 查询供应商库存明细分页列表
export function GetInventoryDetailListApi(params) {
    return request.get("/Inventory/GetInventoryDetailList", { params })
}
