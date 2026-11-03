
<template>
    <el-dialog v-model="SelectGoodsDialogVisible" title="选择到货商品" width="850px" :close-on-click-modal="false">
        <!-- 查询条件 -->
        <div style="display: flex; gap: 10px; margin-bottom: 15px;">
            <el-input v-model="SearchModel.goodsTitle" placeholder="请输入商品名称" clearable style="width: 250px;" @keyup.enter="SearchGoods"/>
            <el-button type="primary" @click="SearchGoods">
                查询
            </el-button>
        </div>

        <!-- 商品列表 -->
        <el-table ref="GoodsTableRef" :data="GoodsList" row-key="goodsID" border v-loading="GoodsLoading" @select="HandleSelectionChange" @select-all="HandleSelectionChange">
            <el-table-column type="selection" width="55" :selectable="row => row.isActive" reserve-selection/>
            <el-table-column prop="barCode" label="商品条码" min-width="150"/>
            <el-table-column prop="goodsTitle" label="商品名称" min-width="180"/>
            <el-table-column prop="categoryName" label="商品分类" min-width="120"/>
            <el-table-column label="状态" width="90">
                <template #default="scope">
                    <el-tag :type="scope.row.isActive ? 'success' : 'info'">
                        {{ scope.row.isActive ? '启用' : '禁用' }}
                    </el-tag>
                </template>
            </el-table-column>
        </el-table>
        <!-- 分页 -->
        <el-pagination 
            v-model:current-page="SearchModel.Page"
            v-model:page-size="SearchModel.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="TotalCount"
            layout="total, sizes, prev, pager, next"
            style="margin-top: 15px;"
            @current-change="GetGoodsList"
            @size-change="ChangePageSize"
        />
        <template #footer>
            <el-button @click="SelectGoodsDialogVisible = false">取消</el-button>
            <el-button type="primary" @click="ConfirmSelection">
                确认添加（{{ SelectedGoods.length }}）
            </el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive, nextTick } from 'vue'
import { ElMessage } from 'element-plus'
import { GetGoodsListApi } from '@/api/goods'

// 弹窗状态
const SelectGoodsDialogVisible = ref(false)

// 商品查询参数
const SearchModel = reactive({
    goodsTitle: '',
    Page: 1,
    PageSize: 10
})

// 商品列表
const GoodsList = ref([])
const TotalCount = ref(0)
const GoodsLoading = ref(false)

// 已选商品
const SelectedGoods = ref([])
const GoodsTableRef = ref(null)

// 打开弹窗
async function OpenDialog() {
    SearchModel.goodsTitle = ''
    SearchModel.Page = 1
    SearchModel.PageSize = 10

    SelectedGoods.value = []
    GoodsList.value = []
    TotalCount.value = 0

    SelectGoodsDialogVisible.value = true

    await nextTick()
    GoodsTableRef.value?.clearSelection()

    await GetGoodsList()
}
defineExpose({ OpenDialog })

// 查询真实商品列表
async function GetGoodsList() {
    GoodsLoading.value = true

    try {
        const res = await GetGoodsListApi(SearchModel)

        if (res.code === 200) {
            GoodsList.value = res.data.list
            TotalCount.value = res.data.totalCount
        } else {
            ElMessage.error(res.message)
        }
    } catch (error) {
        console.error('查询商品失败：', error)
        ElMessage.error('查询商品失败')
    } finally {
        GoodsLoading.value = false
    }
}

// 查询商品
function SearchGoods() {
    SearchModel.Page = 1
    GetGoodsList()
}

// 修改分页大小
function ChangePageSize() {
    SearchModel.Page = 1
    GetGoodsList()
}

// 多选商品（支持跨页保留）
function HandleSelectionChange(selection) {
    SelectedGoods.value = selection
}

// 通知父组件
const emit = defineEmits(['success'])

function ConfirmSelection() {
    if (SelectedGoods.value.length === 0) {
        ElMessage.warning('请先选择商品')
        return
    }

    emit('success', [...SelectedGoods.value])

    SelectGoodsDialogVisible.value = false
}
</script>
