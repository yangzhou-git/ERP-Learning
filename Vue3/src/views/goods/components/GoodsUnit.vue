<template>
    <el-dialog v-model="GoodsUnitDialogVisible" :title="`商品单位维护 - ${CurrentGoodsTitle}`" width="800px">
        <div class="search-bar">
            <el-input v-model="SearchModel.UnitName" placeholder="请输入单位名称" clearable style="width: 180px;"/>
            <el-select v-model="SearchModel.IsBaseUnit" placeholder="基础单位" clearable style="width: 130px;">
                <el-option label="全部" :value="null" />
                <el-option label="是" :value="true" />
                <el-option label="否" :value="false" />
            </el-select>
            <el-select v-model="SearchModel.IsActive" placeholder="状态" clearable style="width: 130px;">
                <el-option label="全部" :value="null" />
                <el-option label="启用" :value="true" />
                <el-option label="禁用" :value="false" />
            </el-select>
            <el-button type="primary" @click="SearchGoodsUnit">
                查询
            </el-button>
            <el-button type="success" @click="OpenAddGoodsUnit" style="margin-bottom: 15px;">
                新增单位关系
            </el-button>
        </div>
        <el-table :data="GoodsUnitList">
            <el-table-column prop="unitName" label="单位名称"></el-table-column>
            <el-table-column prop="unitCode" label="单位编码"></el-table-column>
            <el-table-column prop="conversionRate" label="换算率"></el-table-column>
            <el-table-column label="基础单位">
                <template #default="scope">
                    <el-tag v-if="scope.row.isBaseUnit" type="success">是</el-tag>
                    <el-tag v-else type="info">否</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="状态">
                <template #default="scope">
                    <el-switch v-model="scope.row.isActive" active-text="是" inactive-text="否" @change="ChangeGoodsUnitStatus(scope.row)"/>
                </template>
            </el-table-column>
            <el-table-column prop="sort" label="排序"></el-table-column>
            <el-table-column label="操作">
                <template #default="scope">
                    <el-button type="primary" @click="OpenEditGoodsUnit(scope.row)">编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination
            v-model:current-page="SearchModel.Page"
            v-model:page-size="SearchModel.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            :total="TotalCount"
            style="margin-top: 15px;"
            @size-change="HandleSizeChange"
            @current-change="HandleCurrentChange"
        />
    </el-dialog>
    <AddGoodsUnit ref="AddGoodsUnitRef" @success="GetGoodsUnitList"></AddGoodsUnit>
    <EditGoodsUnit ref="EditGoodsUnitRef" @success="GetGoodsUnitList"></EditGoodsUnit>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { GetGoodsUnitListApi,  UpdateGoodsUnitStatusApi } from '../../../api/goodsUnit'
import AddGoodsUnit from './AddGoodsUnit.vue'
import EditGoodsUnit from './EditGoodsUnit.vue'
import { ElMessage } from 'element-plus'

const GoodsUnitDialogVisible = ref(false)

const SearchModel = reactive({
    GoodsID: 0,
    UnitName: '',
    IsBaseUnit: null,
    IsActive: null,
    Page: 1,
    PageSize: 10
})

const CurrentGoodsTitle = ref('')
const OpenDialog = (row) => {
    SearchModel.GoodsID = row.goodsID
    SearchModel.UnitName = ''
    SearchModel.IsBaseUnit = null
    SearchModel.IsActive = null
    SearchModel.Page = 1
    SearchModel.PageSize = 10

    CurrentGoodsTitle.value = row.goodsTitle
    GoodsUnitDialogVisible.value = true

    GetGoodsUnitList()
}

defineExpose({
    OpenDialog
})

const GoodsUnitList = ref([])
const TotalCount = ref(0)
async function GetGoodsUnitList() {
    try {
        const res = await GetGoodsUnitListApi(SearchModel)
        if (res.code === 200) {
            GoodsUnitList.value = res.data.list
            TotalCount.value = res.data.totalCount
        } else {
            console.error(res.message)
        }
    } catch (error) {
        console.error('获取商品单位列表失败：', error)
    }
}

const AddGoodsUnitRef = ref(null)
function OpenAddGoodsUnit() {
    AddGoodsUnitRef.value.OpenDialog(
        SearchModel.GoodsID,
        CurrentGoodsTitle.value
    )
}

async function ChangeGoodsUnitStatus(row) {
    try {
        const data = {
            GoodsUnitID: row.goodsUnitID,
            IsActive: row.isActive
        }
       const res = await UpdateGoodsUnitStatusApi(data)
       if (res.code === 200) {
            ElMessage.success(res.message)
            GetGoodsUnitList()
        } else {
            row.isActive = !row.isActive
            ElMessage.error(res.message)
        }
    } catch (error) {
        row.isActive = !row.isActive
        console.error('修改商品单位状态失败：', error)
        ElMessage.error('修改商品单位状态失败')
    }
}

const EditGoodsUnitRef = ref(null)
function OpenEditGoodsUnit(row) {
    EditGoodsUnitRef.value.OpenDialog(row)
}

function SearchGoodsUnit() {
    SearchModel.Page = 1
    GetGoodsUnitList()
}

function HandleSizeChange(pageSize) {
    SearchModel.PageSize = pageSize
    SearchModel.Page = 1
    GetGoodsUnitList()
}

function HandleCurrentChange(page) {
    SearchModel.Page = page
    GetGoodsUnitList()
}
</script>

<style scoped>
.search-bar {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 15px;
}
</style>