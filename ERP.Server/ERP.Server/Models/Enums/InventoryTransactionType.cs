namespace ERP.Server.Models.Enums
{
    public enum InventoryTransactionType
    {
        // 入库类型 1 ~ 100
        ArrivalStockIn = 1, // 到货入库
        OtherStockIn = 2, // 其他入库

        // 出库类型 101 ~ 200
        SaleStockOut = 101, // 销售出库
        OtherStockOut = 102, // 其他出库

        // 冲销类型 201 ~ 300
        ArrivalStockInCancel = 201   // 撤销到货入库
    }
}
