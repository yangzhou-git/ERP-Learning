<template>
    <el-dialog v-model="AddUserDialogVisible" title="新增员工" width="600px" @closed="ResetForm">
        <el-form :model="AddUserModel" :rules="Rules" ref="AddUserFormRef">
            <el-form-item label="员工名称" prop="UserName">
                <el-input placeholder="请输入员工名称" v-model="AddUserModel.UserName"></el-input>
            </el-form-item>
            <el-form-item label="密码" prop="Password">
                <el-input type="password" placeholder="请输入密码" v-model="AddUserModel.Password"></el-input>
            </el-form-item>
            <el-form-item label="邮箱" prop="Email">
                <el-input placeholder="请输入邮箱" v-model="AddUserModel.Email"></el-input>
            </el-form-item>
            <el-form-item label="手机号" prop="Phone">
                <el-input placeholder="请输入手机号" v-model="AddUserModel.Phone"></el-input>
            </el-form-item>
            <el-form-item label="所属公司" prop="CompanyID">
                <el-select v-model="AddUserModel.CompanyID" placeholder="请选择所属公司">
                    <el-option v-for="Company in CompanyList" :key="Company.companyID" :label="Company.companyName" :value="Company.companyID"/>
                </el-select>
            </el-form-item>
            <el-button @click="AddUser" type="primary">新增员工</el-button>
        </el-form>
    </el-dialog>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'
import { GetCompanyListApi } from '../../../api/company';
import { AddUserApi } from '../../../api/user';
import { ElMessage } from 'element-plus'

const AddUserDialogVisible = defineModel()
const AddUserFormRef = ref()
const CompanyList = ref([]);
const AddUserModel = reactive({
    UserName : '',
    Password : '',
    Email : '',
    Phone : '',
    CompanyID : null
});

onMounted(()=>
    GetCompanyList()
)

const Rules = {
    UserName: [{ required: true, message: "请输入员工名称", trigger: "blur" }],
    Password: [{ required: true, message: "请输入密码", trigger: "blur" }],
    Email: [{ required: true, message: "请输入邮箱", trigger: "blur" }],
    Phone: [{ required: true, message: "请输入手机号", trigger: "blur" }],
    CompanyID: [{ required: true, message: "请选择公司", trigger: "change" }]
}

const emit = defineEmits([
    "success"
])

function ResetForm(){
    AddUserFormRef.value.resetFields()
}

async function GetCompanyList() {
    const res = await GetCompanyListApi()
    CompanyList.value = res.data
}
async function AddUser(){
    await AddUserFormRef.value.validate()
    try{
        const res = await AddUserApi(AddUserModel)
        if(res.code === 200){
            ElMessage.success(res.message)
            AddUserDialogVisible.value = false
            emit("success")
        } else {
            ElMessage.error(res.message)
        }
    } catch(error) {
        ElMessage.error("新增员工失败")
        console.log(error)
    }
}
</script>

<style>

</style>