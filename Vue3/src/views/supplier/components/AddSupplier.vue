<template>
    <el-dialog title="新增供应商" v-model="AddSupplierDialogVisible" @closed="ResetForm">
        <el-form :model="AddSupplierModel" :rules="rules" ref="AddSupplierFormRef" width="600px">
            <el-form-item label="供应商名称" prop="SupplierName">
                <el-input v-model="AddSupplierModel.SupplierName" placeholder="请输入供应商名称"/>
            </el-form-item>
            <el-form-item label="联系人" prop="Contact">
                <el-input v-model="AddSupplierModel.Contact" placeholder="请输入联系人"/>
            </el-form-item>
            <el-form-item label="联系电话" prop="Phone">
                <el-input v-model="AddSupplierModel.Phone" placeholder="请输入联系电话"/>
            </el-form-item>
            <el-form-item label="地址" prop="Address">
                <el-input v-model="AddSupplierModel.Address" placeholder="请输入地址"/>
            </el-form-item>
            <el-form-item label="备注" prop="Remark">
                <el-input type="textarea" v-model="AddSupplierModel.Remark" placeholder="请输入备注"/>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="CloseDialog">取消</el-button>
            <el-button type="primary" :loading="Submitting" @click="AddSupplier">确定</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { AddSupplierApi } from '@/api/supplier'
import { ElMessage } from 'element-plus'

const AddSupplierModel = reactive({
    SupplierName:"",
    Contact:"",
    Phone:"",
    Address:"",
    Remark:""
})

const Submitting = ref(false) // 是否正在提交供应商
const AddSupplierFormRef = ref(null)
const emit = defineEmits(['success'])
const AddSupplier  = async () => {
    if (Submitting.value) return
    Submitting.value = true
    try{
        const valid = await AddSupplierFormRef.value.validate().catch(() => false)
        if(!valid) return
        const res = await AddSupplierApi(AddSupplierModel)
        if(res.code === 200){
            ElMessage.success(res.message)
            emit('success')
            AddSupplierDialogVisible.value=false
        }
        else {
            ElMessage.error(res.message)
            }
    }catch(error){
        ElMessage.error("res.message")
        console.log(error)
    }
    finally{
        Submitting.value = false
    }
}

const AddSupplierDialogVisible = ref(false)
function OpenDialog(){
    AddSupplierDialogVisible.value = true
}
defineExpose({
    OpenDialog
})
function CloseDialog(){
    AddSupplierDialogVisible.value = false
}
const rules = {
    SupplierName:[{ required:true, message:"请输入供应商名称", trigger:"blur" }],
    Contact:[{ required:true, message:"请输入联系人", trigger:"blur" }],
    Phone:[{ required:true, message:"请输入联系电话", trigger:"blur"}]
}
function ResetForm(){
    AddSupplierFormRef.value.resetFields()
}
</script>

<style>
</style>