<template>
    <div class="company-page">
        <h2>公司管理</h2>
        <div class="search-box">
            <el-input placeholder="请输入公司名称" v-model="SearchModel.CompanyName" style="width:200px" clearable/>
            <el-select v-model="SearchModel.IsActive" placeholder="请选择状态" style="width:120px" clearable>
                <el-option label="启用" :value="true"/>
                <el-option label="禁用" :value="false"/>
            </el-select>
            <el-button @click="SearchCompany">查询</el-button>
            <el-button type="primary" @click="OpenAddCompany">新增公司</el-button>
        </div>
        <div class="table-box">
            <el-table :data="CompanyList">
                <el-table-column label="ID" prop="companyID"></el-table-column>
                <el-table-column label="公司名称" prop="companyName"></el-table-column>
                <el-table-column label="联系人" prop="contact"></el-table-column>
                <el-table-column label="电话" prop="phone"></el-table-column>
                <el-table-column label="状态">
                    <template #default="scope">
                        <el-switch v-model="scope.row.isActive" active-text="启用" inactive-text="禁用" @change="ChangeCompanyStatus(scope.row)"/>
                    </template>
                </el-table-column>
                <el-table-column label="操作">
                    <template #default="scope">
                        <el-button type="primary" @click="OpenEditCompany(scope.row)">编辑</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <div class="page-box">
            <el-pagination
                v-model:current-page="SearchModel.Page"
                v-model:page-size="SearchModel.PageSize"
                :page-sizes="[10,20,50,100]"
                :total="TotalCount"
                layout="total, sizes, prev, pager, next, jumper"
                @current-change="HandleCurrentChange"
                @size-change="ChangePageSize"
            />
        </div>
        <AddCompany ref="AddCompanyRef" @success="GetCompanyList"/>
        <EditCompany ref="EditCompanyRef" @success="GetCompanyList" :Company="CurrentCompany"/>
    </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { GetCompanyListApi,UpdateCompanyStatusApi } from '../../api/company'
import { ElMessage } from 'element-plus'
import AddCompany from './components/AddCompany.vue'
import EditCompany from './components/EditCompany.vue'

onMounted(() => {
    GetCompanyList();
})

const SearchModel = reactive({
    CompanyName: '',
    IsActive: null,
    Page: 1,
    PageSize: 10
})

const TotalCount = ref(0)
const CompanyList = ref([])
const GetCompanyList = async () => {
    try {
        const res = await GetCompanyListApi(SearchModel)
        if(res.code === 200){
            CompanyList.value = res.data.list
            TotalCount.value = res.data.totalCount
        }
    } catch(error) {
        ElMessage.error('获取公司列表失败')
        console.error(error)
    }
}

const SearchCompany = () => {
    SearchModel.Page = 1
    GetCompanyList()
}

const AddCompanyRef = ref(null)
const OpenAddCompany = () => {
    if(AddCompanyRef.value){
        AddCompanyRef.value.OpenDialog()
    }
}

const EditCompanyRef = ref(null)
const CurrentCompany = ref({})
const OpenEditCompany = (row)=>{
    CurrentCompany.value = {...row}
    if(EditCompanyRef.value){
        EditCompanyRef.value.OpenDialog()
    }
}

const ChangeCompanyStatus = async(row)=>{
    if(!row){
        return
    }
    //记录修改前状态
    const oldStatus = !row.isActive
    const data = {
        CompanyID: row.companyID,
        IsActive: row.isActive
    }
    try {
        const res = await UpdateCompanyStatusApi(data)
        if(res.code === 200){
            ElMessage.success("修改状态成功")
        }else{
            row.isActive = oldStatus
            ElMessage.error(res.message || "修改状态失败")
        }
    }catch(error){
        row.isActive = oldStatus
        ElMessage.error("修改状态失败")
        console.error(error)
    }
}
const ChangePageSize = (PageSize)=>{
    SearchModel.PageSize = PageSize
    SearchModel.Page = 1
    GetCompanyList()
}
const HandleCurrentChange = (Page)=>{
    SearchModel.Page = Page
    GetCompanyList()
}
</script>

<style>

</style>