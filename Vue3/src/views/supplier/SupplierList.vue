<template>
    <div class="supplier-page">
        <!-- 页面标题 -->
        <h2>供应商资料</h2>
        <!-- 搜索区域 -->
        <div class="search-box">
            <el-input placeholder="请输入供应商名称" v-model="SearchModel.SupplierName" style="width:200px" clearable/>
            <el-button @click="SearchSupplier">查询</el-button>
            <el-button type="primary" @click="OpenAddSupplier">新增供应商</el-button>
        </div>
        <!-- 表格区域 -->
        <div class="table-box">
            <el-table :data="SupplierList">
                <el-table-column label="ID" prop="supplierID"></el-table-column>
                <el-table-column label="供应商名称" prop="supplierName"></el-table-column>
                <el-table-column label="联系人" prop="contact"></el-table-column>
                <el-table-column label="电话" prop="phone"></el-table-column>
                <el-table-column label="地址" prop="address"></el-table-column>
                <el-table-column label="备注" prop="remark"></el-table-column>
                <el-table-column label="建档时间" prop="createTime"></el-table-column>
                <el-table-column label="是否启用">
                    <template #default="scope">
                        <el-switch v-model="scope.row.isActive" active-text="启用" inactive-text="禁用" @change="UpdateSupplierStatus(scope.row)"/>
                    </template>
                </el-table-column>
                <el-table-column label="操作">
                    <template #default="scope">
                        <el-button type="primary" @click="OpenEditSupplier(scope.row)">编辑</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <!-- 分页区域 -->
        <div class="page-box">
            <el-pagination
                v-model:current-page="SearchModel.Page"
                v-model:page-size="SearchModel.PageSize"
                :page-sizes="[10,20,50,100]"
                :total="TotalCount"
                layout="total, sizes, prev, pager, next, jumper"
                @current-change="GetSupplierList"
                @size-change="ChangePageSize"
            />
        </div>
        <AddSupplier ref="AddSupplierRef" @success="GetSupplierList"/>
        <EditSupplier ref="EditSupplierRef" :Supplier="CurrentSupplier" @success="GetSupplierList"/>
    </div>
</template>


<script setup>
import { ref, reactive, onMounted } from 'vue'
import { GetSupplierListApi, UpdateSupplierStatusApi } from '@/api/supplier'
import { ElMessage } from 'element-plus'
import AddSupplier from './components/AddSupplier.vue'
import EditSupplier from './components/EditSupplier.vue'

const AddSupplierRef = ref(null)
const EditSupplierRef = ref(null)
const CurrentSupplier = ref({})

const SupplierList = ref([])
const SearchModel = reactive({
    SupplierName: "",
    Page: 1,
    PageSize: 10
})

onMounted(()=>{
    GetSupplierList()
})

const TotalCount = ref(0)
const GetSupplierList = async()=>{
    try{
        const res = await GetSupplierListApi(SearchModel)
        if(res.code === 200){
            SupplierList.value = res.data.list
            TotalCount.value = res.data.totalCount
        }
    }catch(error){
        ElMessage.error("查询失败")
        console.log(error)
    }
}
function ChangePageSize(){
    SearchModel.Page = 1
    GetSupplierList()
}
const SearchSupplier = ()=>{
    SearchModel.Page = 1
    GetSupplierList()
}
function OpenAddSupplier(){
    AddSupplierRef.value.OpenDialog()
}
async function UpdateSupplierStatus(row){
    const oldStatus = !row.isActive
    const data = {
        SupplierID: row.supplierID,
        IsActive: row.isActive
    }
    try{
        const res = await UpdateSupplierStatusApi(data)
        if(res.code === 200){
            ElMessage.success("修改成功")
        }
        else{
            ElMessage.error(res.message)
            row.isActive = oldStatus
        }
    }catch(error){
        ElMessage.error("修改失败")
        row.isActive = oldStatus
        console.log(error)
    }
}
function OpenEditSupplier(row){
    CurrentSupplier.value = {...row}
    EditSupplierRef.value.OpenDialog()
}
</script>

<style scoped>
.supplier-page{
    width:100%;
}
.search-box{
    display:flex;
    gap:10px;
    margin-bottom:20px;
}
</style>