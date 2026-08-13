<template>
    <el-dialog title="编辑公司" width="600px" v-model="EditCompanyDialogVisible" @closed="ResetForm">
        <el-form :model="EditCompanyModel" :rules="Rules" ref="EditCompanyFormRef">
            <el-form-item label="公司编码" prop="CompanyCode">
                <el-input v-model="EditCompanyModel.CompanyCode"/>
            </el-form-item>
            <el-form-item label="公司名称" prop="CompanyName">
                <el-input v-model="EditCompanyModel.CompanyName" />
            </el-form-item>
            <el-form-item label="联系人" prop="Contact">
                <el-input v-model="EditCompanyModel.Contact"/>
            </el-form-item>
            <el-form-item label="电话" prop="Phone">
                <el-input v-model="EditCompanyModel.Phone"/>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="EditCompanyDialogVisible=false">取消</el-button>
            <el-button type="primary" @click="SaveCompany">保存</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { UpdateCompanyApi } from '@/api/company'
import { ElMessage } from 'element-plus'

const EditCompanyModel = reactive({
    CompanyID: null,
    CompanyCode: '',
    CompanyName: '',
    Contact: '',
    Phone: ''
})

const props = defineProps({
    Company: {
        type: Object,
        default: () => ({})
    }
})

watch(() => props.Company, (newCompany) => {
    if(!newCompany.companyID){
        return
    }
    EditCompanyModel.CompanyID = newCompany.companyID
    EditCompanyModel.CompanyCode = newCompany.companyCode
    EditCompanyModel.CompanyName = newCompany.companyName
    EditCompanyModel.Contact = newCompany.contact
    EditCompanyModel.Phone = newCompany.phone

},{immediate:true})

const EditCompanyDialogVisible = ref(false)
const EditCompanyFormRef = ref(null)
const ResetForm = () => {
    EditCompanyFormRef.value?.resetFields()
}
const emit = defineEmits(["success"])
const SaveCompany = async()=>{
    if(!EditCompanyFormRef.value){
        return
    }
    await EditCompanyFormRef.value.validate()
    try{
        const res = await UpdateCompanyApi(EditCompanyModel)
        if(res.code === 200){
            ElMessage.success('修改公司成功')
            EditCompanyDialogVisible.value = false
            emit('success')
        }
    }catch(error){
        ElMessage.error('修改公司失败')
        console.error(error)
    }
}
const Rules = {
    CompanyCode: [{ required: true, message: '请输入公司编码', trigger: 'blur' }],
    CompanyName: [{ required: true, message: '请输入公司名称', trigger: 'blur' }]
}
const OpenDialog = () => {
    EditCompanyDialogVisible.value = true
}
defineExpose({
    OpenDialog
})
</script>