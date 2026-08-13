<template>

    <div class="user-page">
        <h2>员工管理</h2>
        <div class="search-box">
            <el-input placeholder="请输入用户名" style="width: 200px;" v-model="SearchModel.UserName" clearable/>
            <el-button @click="HandleSearch">查询</el-button>
            <el-button @click="OpenAddUser" type="primary">新增员工</el-button>
        </div>

        <!-- 员工表格 -->
        <el-table :data="UserList">
            <el-table-column prop="userName" label="用户名"></el-table-column>
            <el-table-column prop="phone" label="手机号"></el-table-column>
            <el-table-column prop="companyID" label="公司"></el-table-column>
            <el-table-column prop="isActive" label="状态">
                <template #default="scope">
                    <el-switch v-model="scope.row.isActive" active-text="启用" inactive-text="禁用" @change="ChangeUserStatus(scope.row)"/>
                </template>
            </el-table-column>
            <el-table-column label="操作">
                <template #default="scope">
                    <el-button type="primary" @click="OpenEditUser(scope.row)">编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination
            v-model:current-page="SearchModel.Page"
            v-model:page-size="SearchModel.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @current-change="HandleCurrentChange"
            @size-change="HandleSizeChange"
        />
        <!-- 新增员工弹窗 -->
        <AddUser v-model="AddUserDialogVisible" @success="GetUserList"/>
        <!-- 编辑员工弹窗 -->
        <EditUser v-model="EditUserDialogVisible" :User="CurrentUser" @success="GetUserList"/>
    </div>

</template>


<script setup>
import { ref,onMounted } from 'vue'
import { GetUserListApi } from '@/api/user/index.js'
import AddUser from './components/AddUser.vue'
import EditUser from './components/EditUser.vue'
import { UpdateUserStatusApi } from '@/api/User'
import { ElMessage } from 'element-plus'

const AddUserDialogVisible = ref(false)
const UserList = ref([]);
const EditUserDialogVisible = ref(false)
const CurrentUser = ref({})

onMounted(() => {
    GetUserList();
});

const SearchModel = ref({
    UserName: "",
    Page: 1,
    PageSize: 10
})

//修改用户状态
const ChangeUserStatus = async (row) => {
    const data = {
        UserID: row.userID,
        IsActive: row.isActive
    }
    try {
        const res = await UpdateUserStatusApi(data)
        if (res.code === 200) {
            ElMessage.success(res.message)
            GetUserList()
        }
        else {
            row.isActive = !row.isActive
            ElMessage.error(res.message)
        }
    } catch {
        row.isActive = !row.isActive
        ElMessage.error("修改失败")
    }
}

function HandleSearch() {
    SearchModel.value.Page = 1
    GetUserList()
}

//员工总数
const TotalCount = ref(0)
async function GetUserList() {
    try {
        const res = await GetUserListApi(SearchModel.value)
        if (res.code === 200) {
            UserList.value = res.data.list
            TotalCount.value = res.data.totalCount
        } else {
            console.error(res.message)
        }
    } catch (error) {
        console.error('获取员工列表失败：', error)
    }
}

function HandleCurrentChange(Page) {
    SearchModel.value.Page = Page
    GetUserList()
}

function HandleSizeChange(PageSize) {
    SearchModel.value.PageSize = PageSize
    SearchModel.value.Page = 1
    GetUserList()
}

function OpenAddUser(){
    AddUserDialogVisible.value = true
}
function OpenEditUser(row){
    CurrentUser.value = row
    EditUserDialogVisible.value = true
}
</script>


<style scoped>
.seach-box{
    display: flex;
    gap: 10px;
    margin-bottom: 20px;
}
</style>