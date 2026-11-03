
<template>
    <el-dialog v-model="DetailVisible" title="供应商库存明细" width="850px" destroy-on-close>
        <!-- 当前商品信息 -->
        <div class="detail-info">
            <span>商品名称：{{ SelectedGoods.goodsTitle }}</span>
            <span>所属站点：{{ SelectedGoods.locationName }}</span>
        </div>
        <!-- 供应商库存明细列表 -->
        <el-table :data="DetailList" v-loading="DetailLoading" border stripe style="width: 100%">
            <el-table-column prop="supplierName" label="供应商名称" min-width="160"/>
            <el-table-column label="库存数量" width="140">
                <template #default="{ row }">
                    {{ row.quantity }} {{ SelectedGoods.unitName }}
                </template>
            </el-table-column>
            <el-table-column prop="inventoryAmount" label="库存金额（元）" width="150"/>
            <el-table-column prop="averageCost" label="平均成本（元/基础单位）" width="190"/>
        </el-table>
        <!-- 分页 -->
        <div class="pagination">
            <el-pagination
                v-model:current-page="DetailPage"
                v-model:page-size="DetailPageSize"
                :total="DetailTotalCount"
                :page-sizes="[10, 20, 50, 100]"
                layout="total, sizes, prev, pager, next"
                @current-change="GetInventoryDetailList"
                @size-change="ChangePageSize"
            />
        </div>
    </el-dialog>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
import { GetInventoryDetailListApi } from '@/api/inventory/inventory'

// 弹窗显示状态
const DetailVisible = ref(false)

// 数据加载状态
const DetailLoading = ref(false)

// 库存明细列表
const DetailList = ref([])
const DetailTotalCount = ref(0)

// 分页
const DetailPage = ref(1)
const DetailPageSize = ref(10)

// 当前选中的商品和站点
const SelectedGoods = ref({
    goodsID: null,
    locationID: null,
    goodsTitle: '',
    locationName: '',
    unitName: ''
})

// 打开库存明细弹窗
function open(row) {
    SelectedGoods.value = {
        goodsID: row.goodsID,
        locationID: row.locationID,
        goodsTitle: row.goodsTitle,
        locationName: row.locationName,
        unitName: row.unitName
    }
    // 每次打开重新加载第一页
    DetailPage.value = 1
    DetailPageSize.value = 10
    DetailList.value = []
    DetailTotalCount.value = 0
    DetailVisible.value = true
    GetInventoryDetailList()
}

// 查询供应商库存明细
async function GetInventoryDetailList() {
    DetailLoading.value = true
    try {
        const params = {
            GoodsID: SelectedGoods.value.goodsID,
            LocationID: SelectedGoods.value.locationID,
            Page: DetailPage.value,
            PageSize: DetailPageSize.value
        }
        const response = await GetInventoryDetailListApi(params)
        // 兼容已解包和未解包的 Axios 响应
        const result = response?.data?.code !== undefined ? response.data : response
        if (result?.code !== 200) {
            ElMessage.error(result?.message || '查询库存明细失败')
            DetailList.value = []
            DetailTotalCount.value = 0
            return
        }
        DetailList.value = result.data?.list ?? []
        DetailTotalCount.value = result.data?.totalCount ?? 0
    } catch (error) {
        DetailList.value = []
        DetailTotalCount.value = 0
        ElMessage.error('查询库存明细失败')
        console.error(error)
    } finally {
        DetailLoading.value = false
    }
}

// 修改每页显示数量
function ChangePageSize() {
    DetailPage.value = 1
    GetInventoryDetailList()
}

// 暴露方法给父组件
defineExpose({
    open
})
</script>

<style scoped>
.detail-info {
    display: flex;
    gap: 30px;
    margin-bottom: 20px;
    color: #606266;
}

.pagination {
    display: flex;
    justify-content: flex-end;
    margin-top: 20px;
}
</style>
