<template>
    <el-dialog title="编辑供应商" width="600px" v-model="EditSupplierDialogVisible" @closed="ResetForm">
        <el-form :model="EditSupplierModel" :rules="Rules" ref="EditSupplierFormRef">
            <el-form-item label="供应商名称">
                <el-input v-model="EditSupplierModel.SupplierName"/>
            </el-form-item>
            <el-form-item label="联系人">
                <el-input v-model="EditSupplierModel.Contact"/>
            </el-form-item>
            <el-form-item label="电话">
                <el-input v-model="EditSupplierModel.Phone"/>
            </el-form-item>
            <el-form-item label="地址">
                <el-input v-model="EditSupplierModel.Address"/>
            </el-form-item>
            <el-form-item label="备注">
                <el-input v-model="EditSupplierModel.Remark"/>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="EditSupplierDialogVisible=false">取消</el-button>
            <el-button type="primary" @click="UpdateSupplier">保存</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { reactive, ref, watch } from 'vue';
import { UpdateSupplierApi } from '@/api/supplier'
import { ElMessage } from 'element-plus'

const EditSupplierModel = reactive({
    SupplierID:null,
    SupplierName:"",
    Contact:"",
    Phone:"",
    Address:"",
    Remark:""
})

const props = defineProps({
    Supplier:{
        type:Object,
        default:()=>({})
    }
})
defineExpose({
    OpenDialog
})
const EditSupplierDialogVisible = ref(false)
const EditSupplierFormRef = ref(null)
function OpenDialog(){
    EditSupplierDialogVisible.value = true
}
watch(() => props.Supplier,
    (newValue) => {
        if(newValue){
            EditSupplierModel.SupplierID = newValue.supplierID
            EditSupplierModel.SupplierName = newValue.supplierName
            EditSupplierModel.Contact = newValue.contact
            EditSupplierModel.Phone = newValue.phone
            EditSupplierModel.Address = newValue.address
            EditSupplierModel.Remark = newValue.remark
        }},
    {
        deep:true,
        immediate:true
    }
)
const Rules = {
    SupplierName:[{ required:true, message:"请输入供应商名称", trigger:"blur" }]
}
const emit = defineEmits(["success"])
async function UpdateSupplier(){
    await EditSupplierFormRef.value.validate()
    try{
        const res = await UpdateSupplierApi(EditSupplierModel)
        if(res.code === 200){
            ElMessage.success("修改成功")
            EditSupplierDialogVisible.value = false
            emit("success")
        }
        else{
            ElMessage.error(res.message)
        }
    }catch(error){
        ElMessage.error("修改失败")
        console.log(error)
    }
}
function ResetForm(){
    if(EditSupplierFormRef.value){
        EditSupplierFormRef.value.clearValidate()
    }
    EditSupplierModel.SupplierID = null
    EditSupplierModel.SupplierName = ""
    EditSupplierModel.Contact = ""
    EditSupplierModel.Phone = ""
    EditSupplierModel.Address = ""
    EditSupplierModel.Remark = ""
}
</script>

<style>

</style>