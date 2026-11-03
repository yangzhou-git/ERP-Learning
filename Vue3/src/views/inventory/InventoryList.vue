
<template>
    <div class="inventory-page">
        <h2>库存查询</h2>

        <!-- 查询区域 -->
        <el-form :inline="true" :model="SearchModel">
            <el-form-item label="商品名称">
                <el-input v-model="SearchModel.GoodsTitle" placeholder="请输入商品名称" clearable @keyup.enter="Search"/>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" @click="Search">查询</el-button>
                <el-button @click="Reset">重置</el-button>
            </el-form-item>
        </el-form>
        <!-- 库存汇总列表 -->
        <el-table :data="InventoryList" v-loading="loading" border stripe style="width: 100%">
            <el-table-column prop="goodsTitle" label="商品名称" min-width="180"/>
            <el-table-column prop="locationName" label="站点名称" min-width="180"/>
            <el-table-column label="库存数量" width="150">
                <template #default="{ row }">
                    {{ row.quantity }} {{ row.unitName }}
                </template>
            </el-table-column>
            <el-table-column prop="totalAmount" label="库存金额（元）" width="160"/>
            <!-- 操作 -->
            <el-table-column label="操作" width="130">
                <template #default="{ row }">
                    <el-button type="primary" link @click="OpenInventoryDetail(row)">
                        查看明细
                    </el-button>
                </template>
            </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination">
            <el-pagination
                v-model:current-page="SearchModel.Page"
                v-model:page-size="SearchModel.PageSize"
                :total="totalCount"
                :page-sizes="[10, 20, 50, 100]"
                layout="total, sizes, prev, pager, next"
                @current-change="GetInventorySummaryList"
                @size-change="ChangePageSize"
            />
        </div>
        <InventoryDetail ref="DetailRef" />
    </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { GetInventorySummaryListApi } from '@/api/inventory/inventory'
import InventoryDetail from './components/InventoryDetail.vue'

// 查询条件
const SearchModel = reactive({
    GoodsTitle: '',
    LocationID: null,
    Page: 1,
    PageSize: 10
})

// 库存汇总列表
const InventoryList = ref([])
const totalCount = ref(0)
const loading = ref(false)

// 查询库存汇总分页列表
async function GetInventorySummaryList() {
    loading.value = true

    try {
        const response = await GetInventorySummaryListApi(SearchModel)
        // 兼容 request 返回业务数据或完整 Axios 响应
        const result = response?.data?.code !== undefined ? response.data : response
        if (result.code !== 200) {
            ElMessage.error(result.message || '查询库存汇总失败')
            InventoryList.value = []
            totalCount.value = 0
            return
        }
        InventoryList.value = result.data?.list ?? []
        totalCount.value = result.data?.totalCount ?? 0
    } catch (error) {
        ElMessage.error('查询库存汇总失败')
        InventoryList.value = []
        totalCount.value = 0
        console.error(error)
    } finally {
        loading.value = false
    }
}

// 查询按钮
function Search() {
    SearchModel.Page = 1
    GetInventorySummaryList()
}

// 重置按钮
function Reset() {
    SearchModel.GoodsTitle = ''
    SearchModel.LocationID = null
    SearchModel.Page = 1
    GetInventorySummaryList()
}

// 修改每页显示数量
function ChangePageSize() {
    SearchModel.Page = 1
    GetInventorySummaryList()
}

// 页面加载后自动查询
onMounted(() => {
    GetInventorySummaryList()
})

const DetailRef = ref(null)
// 打开库存明细弹窗
function OpenInventoryDetail(row) {
    DetailRef.value?.open(row)
}
</script>

<style scoped>
.inventory-page {
    width: 100%;
}

.pagination {
    display: flex;
    justify-content: flex-end;
    margin-top: 20px;
}
</style>
