<template>
    <el-dialog v-model="EditGoodsUnitDialogVisible" :title="`编辑商品单位 - ${CurrentUnitName}`" width="500px" @closed="ResetForm">
        <el-form :model="EditGoodsUnitModel" ref="EditGoodsUnitFormRef" :rules="Rules">
            <el-form-item label="单位">
                <el-input v-model="EditGoodsUnitModel.UnitName" disabled />
            </el-form-item>
            <el-form-item label="换算率" prop="ConversionRate">
                <el-input-number v-model="EditGoodsUnitModel.ConversionRate" :min="1" :precision="2" :disabled="EditGoodsUnitModel.IsBaseUnit" style="width: 100%;"/>
            </el-form-item>
            <el-form-item label="基础单位">
                <el-switch v-model="EditGoodsUnitModel.IsBaseUnit" active-text="是" inactive-text="否" disabled/>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="EditGoodsUnitDialogVisible = false">取消</el-button>
            <el-button type="primary" @click="UpdateGoodsUnit">保存</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { UpdateGoodsUnitApi } from '../../../api/goodsUnit'
import { ElMessage } from 'element-plus'

const EditGoodsUnitDialogVisible = ref(false)

const CurrentUnitName = ref('')

const EditGoodsUnitModel = reactive({
    GoodsUnitID: 0,
    GoodsID: 0,
    UnitID: 0,
    UnitName: '',
    ConversionRate: 1,
    IsBaseUnit: false
})

const EditGoodsUnitFormRef = ref()

function OpenDialog(row) {
    EditGoodsUnitModel.GoodsUnitID = row.goodsUnitID
    EditGoodsUnitModel.GoodsID = row.goodsID
    EditGoodsUnitModel.UnitID = row.unitID
    EditGoodsUnitModel.UnitName = row.unitName
    EditGoodsUnitModel.ConversionRate = row.conversionRate
    EditGoodsUnitModel.IsBaseUnit = row.isBaseUnit
    CurrentUnitName.value = row.unitName

    EditGoodsUnitDialogVisible.value = true
}

defineExpose({
    OpenDialog
})

const emit = defineEmits([
    "success"
])
async function UpdateGoodsUnit() {
    try {
        await EditGoodsUnitFormRef.value.validate()
        const res = await UpdateGoodsUnitApi(EditGoodsUnitModel)
        if (res.code === 200) {
            ElMessage.success(res.message || '保存成功')
            EditGoodsUnitDialogVisible.value = false
            emit('success')
        } else {
            ElMessage.error(res.message || '保存失败')
        }
    } catch (error) {
        console.log('修改商品单位关系失败：', error)
    }
}

const Rules = {
    ConversionRate: [{ required: true, message: '请输入换算率', trigger: 'blur' }]
}

function ResetForm() {
    EditGoodsUnitFormRef.value?.clearValidate()
}
</script>