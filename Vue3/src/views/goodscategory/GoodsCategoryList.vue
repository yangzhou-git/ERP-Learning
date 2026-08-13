<template>
    <div>
        <h2>商品分类列表</h2>
        <div class="search-box">
            <el-input placeholder="请输入分类名称" style="width:200px" v-model="SearchModel.CategoryName" clearable/>
            <el-button @click="SearchCategory">查询</el-button>
            <el-button type="primary" @click="OpenAddGoodsCategory">新增分类</el-button>
        </div>
        <div class="table-box">
            <el-table :data="GoodsCategoryList">
                <el-table-column prop="categoryID" label="分类ID"></el-table-column>
                <el-table-column  prop="categoryName" label="分类名称"></el-table-column>
                <el-table-column prop="categoryCode" label="分类编码"></el-table-column>
                <el-table-column prop="parentID" label="父级ID"></el-table-column>
                <el-table-column prop="sort" label="排序"></el-table-column>
                <el-table-column label="状态">
                    <template #default="scope">
                        <el-switch :model-value="scope.row.isActive" active-text="启用" inactive-text="禁用" @change="ChangeCategoryStatus(scope.row,$event)"></el-switch>
                    </template>
                </el-table-column>
                <el-table-column label="操作">
                    <template #default="scope">
                        <el-button type="primary" @click="OpenEditGoodsCategory(scope.row)">编辑</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <div class="page-box">
            <el-pagination
                v-model:current-page="SearchModel.Page"
                v-model:page-size="SearchModel.PageSize"
                :page-sizes="[10, 20, 50, 100]"
                :total="TotalCount"
                layout="total, sizes, prev, pager, next, jumper"
                @current-change="HandleCurrentChange"
                @size-change="HandleSizeChange"
            />
        </div>
        <AddGoodsCategory ref="AddGoodsCategoryRef" @success="GetGoodsCategoryList"/>
        <EditGoodsCategory ref="EditGoodsCategoryRef" @success="GetGoodsCategoryList" :GoodsCategory="EditGoodsCategoryModel"/>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { GetGoodsCategoryListApi, UpdateGoodsCategoryStatusApi } from '@/api/goodsCategory'
import { ElMessage } from 'element-plus'
import AddGoodsCategory from './components/AddGoodsCategory.vue'
import EditGoodsCategory from './components/EditGoodsCategory.vue'

onMounted(() => {
    GetGoodsCategoryList()
})

const GoodsCategoryList = ref([])
const GetGoodsCategoryList = async () => {
    try {
        const res = await GetGoodsCategoryListApi(SearchModel.value)
        if(res.code === 200){
            GoodsCategoryList.value = res.data.list
            TotalCount.value = res.data.totalCount
        }
    } catch(error) {
        console.error('获取商品分类列表失败', error)
    }
}

const ChangeCategoryStatus = async (row, event) => {
    const data = {
        CategoryID: row.categoryID,
        IsActive: event
    }
    try {
        const res = await UpdateGoodsCategoryStatusApi(data)
        if(res.code === 200){
            ElMessage.success('商品分类状态更新成功')
            GetGoodsCategoryList()
        }
    } catch(error) {
        ElMessage.error('更新商品分类状态失败')
        console.error('更新商品分类状态失败', error)
    }
}

const AddGoodsCategoryRef = ref(null)
const OpenAddGoodsCategory = () => {
    AddGoodsCategoryRef.value.OpenDialog()
}
const EditGoodsCategoryRef = ref(null)
const EditGoodsCategoryModel = ref({})
const OpenEditGoodsCategory = (row) => {
    EditGoodsCategoryModel.value = {...row}
    EditGoodsCategoryRef.value.OpenDialog()
}

const SearchModel = ref({
    CategoryName:'',
    Page:1,
    PageSize:10
})
function SearchCategory(){
    SearchModel.value.Page = 1
    GetGoodsCategoryList()
}
const TotalCount = ref(0)
function HandleCurrentChange(Page){
    SearchModel.value.Page = Page
    GetGoodsCategoryList()
}
function HandleSizeChange(PageSize){
    SearchModel.value.PageSize = PageSize
    SearchModel.value.Page = 1
    GetGoodsCategoryList()
}
</script>

<style>
</style>