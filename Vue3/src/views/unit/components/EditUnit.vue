<template>
    <el-dialog v-model="EditUnitDialogVisible" title="编辑单位" width="500px" @closed="ResetForm">
        <el-form :model="EditUnitModel" label-width="100px" ref="EditUnitFormRef" :rules="rules">
            <el-form-item label="单位名称" prop="UnitName">
                <el-input v-model="EditUnitModel.UnitName" />
            </el-form-item>
            <el-form-item label="单位编码" prop="UnitCode">
                <el-input v-model="EditUnitModel.UnitCode" />
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="EditUnitDialogVisible = false">取消</el-button>
            <el-button type="primary" @click="UpdateUnit">保存</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { UpdateUnitApi } from '../../../api/unit'
import { ElMessage } from 'element-plus'

const EditUnitDialogVisible = ref(false)

const EditUnitModel = reactive({
    UnitID: 0,
    UnitName: "",
    UnitCode: ""
})

function OpenDialog(row){
    EditUnitModel.UnitID = row.unitID
    EditUnitModel.UnitName = row.unitName
    EditUnitModel.UnitCode = row.unitCode

    EditUnitDialogVisible.value = true
}

defineExpose({
    OpenDialog
})

const rules = {
    UnitName: [{ required: true, message: "请输入单位名称", trigger: "blur" }],
    UnitCode: [{ required: true, message: "请输入单位编码", trigger: "blur" }]
}

const emit = defineEmits(["success"])
const EditUnitFormRef = ref(null)
async function UpdateUnit(){ 
    try{ 
        await EditUnitFormRef.value.validate() 
        const res = await UpdateUnitApi(EditUnitModel)
        if(res.code == 200){
            ElMessage.success(res.message)
            EditUnitDialogVisible.value = false
            emit("success")
        }else{
            ElMessage.error(res.message)
        }
    } 
    catch(error){ 
        console.log(error) 
    } 
}

function ResetForm(){
    EditUnitFormRef.value?.clearValidate()
}
</script>