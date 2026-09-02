<template>
    <div class="goods-page">
        <h2>商品管理</h2>

        <div class="search-box">
            <el-input placeholder="请输入商品名称" style="width: 200px" v-model="SearchModel.goodsTitle"></el-input>
            <el-button @click="SearchGoods">查询</el-button>
            <el-button type="primary" @click="OpenAddGoods">新增商品</el-button>
        </div>
        <el-table :data="GoodsList">
            <el-table-column prop="goodsID" label="商品ID"></el-table-column>
            <el-table-column prop="barCode" label="商品条码"></el-table-column>
            <el-table-column prop="goodsTitle" label="商品名称"></el-table-column>
            <el-table-column prop="categoryName" label="商品分类"></el-table-column>
            <el-table-column prop="goodsPrice" label="商品价格"></el-table-column>
            <el-table-column label="是否启用">
                <template #default="scope">
                    <el-switch v-model="scope.row.isActive" active-text="启用" inactive-text="禁用" @change="ChangeGoodsStatus(scope.row)"/>
                </template>
            </el-table-column>
            <el-table-column label="操作">
                <template #default="scope">
                    <el-button type="primary" @click="OpenEditGoods(scope.row)">编辑</el-button>
                    <el-button type="success" @click="OpenGoodsUnit(scope.row)">单位维护</el-button>
                </template>
            </el-table-column>
        </el-table>
        <div class="page-box">
            <el-pagination
                v-model:current-page="SearchModel.Page"
                v-model:page-size="SearchModel.PageSize"
                :page-sizes="[10,20,50,100]"
                :total="TotalCount"
                layout="total, sizes, prev, pager, next, jumper"
                @current-change="GetGoodsList"
                @size-change="ChangePageSize"
            />
        </div>
    <AddGoods ref="AddGoodsRef"  @success="GetGoodsList"></AddGoods>
    <EditGoods ref="EditGoodsRef" @success="GetGoodsList"></EditGoods>
    <GoodsUnit ref="GoodsUnitRef"></GoodsUnit>
    </div>
</template>

<script setup>
import { ref,onMounted } from 'vue'
import { GetGoodsListApi, UpdateGoodsStatusApi } from '../../api/goods'
import AddGoods from './components/AddGoods.vue'
import EditGoods from './components/EditGoods.vue'
import { ElMessage } from 'element-plus';
import GoodsUnit from './components/GoodsUnit.vue'

onMounted(() => {
    SearchGoods();
});

const SearchModel = ref({
    goodsTitle : '',
    Page : 1,
    PageSize : 10
})
//获取商品列表
const TotalCount = ref(0);
const GoodsList = ref([]);
async function GetGoodsList() {
   try {
        const res = await GetGoodsListApi(SearchModel.value)
        if (res.code === 200) {
            GoodsList.value = res.data.list
            TotalCount.value = res.data.totalCount
        } else {
            console.error(res.message)
        }
    } catch (error) {
        console.error('获取商品列表失败：', error)
    }
}

//修改商品资料
const EditGoodsRef = ref(null)
function OpenEditGoods(row){
    EditGoodsRef.value.OpenDialog(row)
}

//打开新增商品
const AddGoodsRef = ref(null);
function OpenAddGoods(){
    AddGoodsRef.value.OpenDialog();
}
//查询重置页码
function SearchGoods(){
    SearchModel.value.Page = 1
    GetGoodsList()
}
//修改每页显示数量
function ChangePageSize(){
    SearchModel.value.Page = 1
    GetGoodsList()
}
//修改商品状态
async function ChangeGoodsStatus(row){
    const oldStatus = !row.isActive
    const data = {
        GoodsID: row.goodsID,
        IsActive: row.isActive
    }
    try{
        const res = await UpdateGoodsStatusApi(data)
        if(res.code === 200){
            ElMessage.success(res.message)
            GetGoodsList()
        }
        else{
            //恢复原状态
            row.isActive = oldStatus
            ElMessage.error(res.message)
        }
    }
    catch(error){
        console.log("修改商品状态失败：",error)
        //恢复原状态
        row.isActive = oldStatus
        ElMessage.error("系统异常，请稍后重试")
    }
}

const GoodsUnitRef = ref(null)
function OpenGoodsUnit(row){
    GoodsUnitRef.value.OpenDialog(row)
}
</script>

<style scoped>
.search-box{
    display: flex;
    gap: 10px;
    margin-bottom: 20px;
}
.page-box{
    margin-top:20px;
    display:flex;
}
</style>