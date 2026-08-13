<template>
    <el-dialog title="新增公司" width="600px" v-model="AddCompanyDialogVisible" @closed="ResetForm">
        <el-form :model="CompanyModel" :rules="Rules" ref="AddCompanyFormRef">
            <el-form-item label="公司编码" prop="CompanyCode">
                <el-input v-model="CompanyModel.CompanyCode" placeholder="请输入公司编码"/>
            </el-form-item>
            <el-form-item label="公司名称" prop="CompanyName">
                <el-input v-model="CompanyModel.CompanyName" placeholder="请输入公司名称"/>
            </el-form-item>
            <el-form-item label="联系人" prop="Contact">
                <el-input v-model="CompanyModel.Contact" placeholder="请输入联系人"/>
            </el-form-item>
            <el-form-item label="电话" prop="Phone">
                <el-input v-model="CompanyModel.Phone" placeholder="请输入电话"/>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="AddCompanyDialogVisible=false">取消</el-button>
            <el-button type="primary" @click="AddCompany">保存</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { AddCompanyApi } from '@/api/company'
import { ElMessage } from 'element-plus'

const CompanyModel = reactive({
    CompanyCode: '',
    CompanyName: '',
    Contact:"",
    Phone:""
})
const AddCompanyDialogVisible = ref(false)
const OpenDialog = () => {
    AddCompanyDialogVisible.value = true
}
defineExpose({
    OpenDialog
})
const emit = defineEmits(["success"])
const AddCompany = async () => {
    if(!AddCompanyFormRef.value){
        return
    }
    await AddCompanyFormRef.value.validate()
    try {
        const res = await AddCompanyApi(CompanyModel)
        if(res.code === 200){
            ElMessage.success('新增公司成功')
            AddCompanyDialogVisible.value = false
            emit('success')
        }
    } catch(error){
        ElMessage.error('新增公司失败')
        console.error(error)
    }
}

const AddCompanyFormRef = ref(null)
const Rules = {
    CompanyCode:[{ required:true, message:'请输入公司编码', trigger:'blur' }],
    CompanyName:[{ required:true, message:'请输入公司名称', trigger:'blur' }]
}
const ResetForm = () => {
    if(AddCompanyFormRef.value){
        AddCompanyFormRef.value.resetFields()
    }
}
</script>

<style>

</style>