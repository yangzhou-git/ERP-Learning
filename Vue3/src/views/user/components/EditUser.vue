<template>
    <el-dialog v-model="EditUserDialogVisible" title="修改员工资料" width="600px" @closed="ClearValidate">
        <el-form :model="EditUserModel" :rules="Rules" ref="EditUserFormRef">
            <el-form-item label="员工名称" prop="userName">
                <el-input v-model="EditUserModel.userName"/>
            </el-form-item>
            <el-form-item label="邮箱" prop="email">
                <el-input v-model="EditUserModel.email"/>
            </el-form-item>
            <el-form-item label="手机号" prop="phone">
                <el-input v-model="EditUserModel.phone"/>
            </el-form-item>
            <el-form-item label="所属公司" prop="companyID">
                <el-select v-model="EditUserModel.companyID">
                    <el-option v-for="Company in CompanyList" :key="Company.companyID" :label="Company.companyName" :value="Company.companyID"/>
                </el-select>
            </el-form-item>
        </el-form>

        <template #footer>
            <el-button @click="EditUserDialogVisible = false">取消</el-button>
            <el-button type="primary"  @click="UpdateUser">保存</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { reactive, ref, onMounted, watch } from 'vue'
import { GetCompanyListApi } from '../../../api/company'
import { UpdateUserApi } from '../../../api/user'

const EditUserDialogVisible = defineModel()
const EditUserFormRef = ref()
const Rules = {
    userName: [{ required: true, message: "请输入员工名称", trigger: "blur" }],
    email: [{ required: true, message: "请输入邮箱", trigger: "blur" }],
    phone: [{ required: true, message: "请输入手机号", trigger: "blur" }],
    companyID: [{ required: true, message: "请选择公司", trigger: "change" }]
}

const props = defineProps({
    User:{
        type:Object,
        default:()=>({})
    }
})

const emit = defineEmits([
    "success"
])

const CompanyList = ref([])

const EditUserModel = reactive({
    userID:null,
    userName:'',
    email:'',
    phone:'',
    companyID:null
})

onMounted(()=>
    GetCompanyList()
)

function ClearValidate(){
    EditUserFormRef.value?.clearValidate()
}

async function GetCompanyList(){
    const res = await GetCompanyListApi()
    CompanyList.value = res.data
}

watch(()=>props.User,(newValue)=>{
        Object.assign(EditUserModel,newValue)
    },
    {
        deep:true
    }
)

async function UpdateUser(){
    await EditUserFormRef.value.validate()
     try{
        const res = await UpdateUserApi(EditUserModel)
        if(res.code === 200){
            EditUserDialogVisible.value=false
            emit("success")
        }
    }catch(error){
        console.log(error)
    }
}

</script>

<style>

</style>