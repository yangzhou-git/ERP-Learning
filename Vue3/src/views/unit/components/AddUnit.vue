<template>
    <el-dialog v-model="AddUnitDialogVisible" title="新增单位" width="500px" @closed="ResetForm">

        <el-form :model="AddUnitModel" label-width="100px" ref="AddUnitFormRef" :rules="rules">
            <el-form-item label="单位名称" prop="UnitName">
                <el-input v-model="AddUnitModel.UnitName" placeholder="请输入单位名称"/>
            </el-form-item>
            <el-form-item label="单位编码" prop="UnitCode">
                <el-input v-model="AddUnitModel.UnitCode" placeholder="请输入单位编码"/>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="AddUnitDialogVisible = false">取消</el-button>
            <el-button type="primary" @click="SubmitAddUnit">保存</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { AddUnitApi } from '../../../api/unit'
import { ElMessage } from 'element-plus'

const AddUnitDialogVisible = ref(false)
const AddUnitFormRef = ref(null)

const AddUnitModel = reactive({
    UnitName: "",
    UnitCode: ""
})

function OpenDialog(){
    AddUnitDialogVisible.value = true
}

defineExpose({
    OpenDialog
})

const rules = {
    UnitName: [{ required: true, message: "请输入单位名称", trigger: "blur" }],
    UnitCode: [{ required: true, message: "请输入单位编码", trigger: "blur" }]
}

const emit = defineEmits(['success'])
async function SubmitAddUnit(){
    AddUnitFormRef.value.validate(async (valid) => {
        if(!valid){
            return
        }
        try{
            const res = await AddUnitApi(AddUnitModel)

            if(res.code === 200){
                ElMessage.success("新增单位成功")
                AddUnitDialogVisible.value = false
                emit('success')
            }
            else{
                ElMessage.error(res.message)
            }
        }
        catch(error){
            ElMessage.error("新增单位失败")
            console.log(error)
        }
    })
}

function ResetForm(){
    AddUnitFormRef.value?.resetFields()
}
</script>