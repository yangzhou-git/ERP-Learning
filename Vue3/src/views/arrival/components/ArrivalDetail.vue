
<template>
    <el-dialog v-model="ArrivalDetailDialogVisible" title="到货单详情" width="90%" destroy-on-close>
        <div v-loading="Loading">
            <!-- 到货单主表信息 -->
            <el-descriptions title="基本信息" :column="3" border>
                <el-descriptions-item label="到货单号">{{ Detail.arrivalCode }}</el-descriptions-item>
                <el-descriptions-item label="到货状态">{{ GetArrivalStatusLabel(Detail.arrivalStatus) }}</el-descriptions-item>
                <el-descriptions-item label="供应商">{{ Detail.supplierName }}</el-descriptions-item>
                <el-descriptions-item label="所属公司">{{ Detail.companyName }}</el-descriptions-item>
                <el-descriptions-item label="到货站点">{{ Detail.locationName }}</el-descriptions-item>
                <el-descriptions-item label="到货时间">{{ Detail.arrivalTime }}</el-descriptions-item>
                <el-descriptions-item label="商品种类数">{{ Detail.goodsCount }}</el-descriptions-item>
                <el-descriptions-item label="总数量">{{ Detail.totalQuantity }}</el-descriptions-item>
                <el-descriptions-item label="到货总金额">{{ Detail.totalAmount }}</el-descriptions-item>
                <el-descriptions-item label="创建人">{{ Detail.createUserName }}</el-descriptions-item>
                <el-descriptions-item label="创建时间">{{ Detail.createTime }}</el-descriptions-item>
                <el-descriptions-item label="审核人">{{ Detail.auditUserName || '-' }}</el-descriptions-item>
                <el-descriptions-item label="审核时间">{{ Detail.auditTime || '-' }}</el-descriptions-item>
                <el-descriptions-item label="入库人">{{ Detail.stockInUserName || '-' }}</el-descriptions-item>
                <el-descriptions-item label="入库时间">{{ Detail.stockInTime || '-' }}</el-descriptions-item>
                <el-descriptions-item label="作废人">{{ Detail.voidUserName || '-' }} </el-descriptions-item>
                <el-descriptions-item label="作废时间">{{ Detail.voidTime || '-' }}</el-descriptions-item>
                <el-descriptions-item label="备注" :span="3">{{ Detail.remark || '-' }}</el-descriptions-item>
            </el-descriptions>
            <!-- 商品明细 -->
            <div style="margin-top: 24px;">
                <h3>到货商品明细</h3>
                <el-table :data="DetailItems" border style="width: 100%" max-height="400">
                    <el-table-column prop="goodsCode" label="商品编码" min-width="130"/>
                    <el-table-column prop="barCode" label="条码" min-width="130"/>
                    <el-table-column prop="goodsTitle" label="商品名称" min-width="160"/>
                    <el-table-column prop="goodsSubTitle" label="商品副标题" min-width="150"/>
                    <el-table-column prop="unitName" label="到货单位" width="100"/>
                    <el-table-column prop="conversionRate" label="换算率" width="100"/>
                    <el-table-column prop="quantity" label="到货数量" width="110"/>
                    <el-table-column prop="baseQuantity" label="基础单位数量" width="130"/>
                    <el-table-column prop="purchaseUnitCost" label="采购单位单价" width="130"/>
                    <el-table-column prop="unitCost" label="基础单位成本" width="130"/>
                    <el-table-column prop="amount" label="明细金额" width="120"/>
                    <el-table-column prop="goodsPrice" label="当前商品售价" width="130"/>
                    <el-table-column prop="currentInventory" label="当前库存" width="110"/>
                    <el-table-column prop="remark" label="备注" min-width="150"/>
                </el-table>
                <!-- 明细独立分页 -->
                <el-pagination
                    v-model:current-page="DetailPage.Page"
                    v-model:page-size="DetailPage.PageSize"
                    :page-sizes="[10, 20, 50, 100]"
                    layout="total, sizes, prev, pager, next"
                    :total="DetailTotal"
                    style="margin-top: 15px; justify-content: flex-end;"
                    @size-change="HandleSizeChange"
                    @current-change="HandleCurrentChange"
                />
            </div>
        </div>
        <template #footer>
            <el-button @click="ArrivalDetailDialogVisible = false">
                关闭
            </el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { GetArrivalDetailApi } from '../../../api/arrival'
import { ArrivalStatusOptions } from '../../../enums/arrivalStatus'

// 到货单详情弹窗显示状态
const ArrivalDetailDialogVisible = ref(false)
const Loading = ref(false)

// 当前到货单ID
const ArrivalID = ref(null)

// 主表详情
const Detail = ref({})

// 商品明细
const DetailItems = ref([])
const DetailTotal = ref(0)

// 商品明细分页
const DetailPage = reactive({
    Page: 1,
    PageSize: 10
})

// 获取到货单状态文字
const GetArrivalStatusLabel = (status) => {
    const result = ArrivalStatusOptions.find(
        item => item.value === status
    )
    return result?.label ?? '未知状态'
}

async function OpenArrivalDetailDialog(arrivalID) {
    ArrivalID.value = arrivalID

    DetailPage.Page = 1
    DetailPage.PageSize = 10

    Detail.value = {}
    DetailItems.value = []
    DetailTotal.value = 0

    ArrivalDetailDialogVisible.value = true

    await GetArrivalDetail()
}

// 查询到货单详情
async function GetArrivalDetail() {
    if (!ArrivalID.value) return

    Loading.value = true

    try {
        const res = await GetArrivalDetailApi({
            ArrivalID: ArrivalID.value,
            Page: DetailPage.Page,
            PageSize: DetailPage.PageSize
        })

        if (res.code === 200) {
            Detail.value = res.data
            DetailItems.value = res.data.arrivalDetails?.list ?? []
            DetailTotal.value = res.data.arrivalDetails?.totalCount ?? 0
        } else {
            ElMessage.error(res.message || '查询到货单详情失败')
        }
    } catch (e) {
        console.error('查询到货单详情失败：', e)
        ElMessage.error('查询到货单详情失败')
    } finally {
        Loading.value = false
    }
}

// 每页条数变化
function HandleSizeChange(pageSize) {
    DetailPage.PageSize = pageSize
    DetailPage.Page = 1
    GetArrivalDetail()
}

// 当前页变化
function HandleCurrentChange(page) {
    DetailPage.Page = page
    GetArrivalDetail()
}

// 暴露方法给父组件
defineExpose({
    OpenArrivalDetailDialog
})
</script>
