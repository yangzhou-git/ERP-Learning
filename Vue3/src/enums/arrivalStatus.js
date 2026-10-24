export const ArrivalStatus = {
    Pending: 0,   // 未审核
    Audited: 1,   // 已审核
    Stocked: 2,   // 已入库
    Voided: 3     // 已作废
}

export const ArrivalStatusOptions = [
    { label: "未审核", value: ArrivalStatus.Pending },
    { label: "已审核", value: ArrivalStatus.Audited },
    { label: "已入库", value: ArrivalStatus.Stocked },
    { label: "已作废", value: ArrivalStatus.Voided }
]