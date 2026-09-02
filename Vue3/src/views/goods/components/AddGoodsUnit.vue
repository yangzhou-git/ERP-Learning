<template>
    <el-dialog v-model="AddGoodsUnitDialogVisible" :title="`新增商品单位 - ${CurrentGoodsTitle}`" width="500px" @closed="ResetForm">
        <el-form :model="AddGoodsUnitModel" :rules="Rules" ref="AddGoodsUnitFormRef">
            <el-form-item label="单位" prop="UnitID">
                <el-select v-model="AddGoodsUnitModel.UnitID" placeholder="请选择单位" style="width: 100%;">
                    <el-option v-for="unit in UnitList" :key="unit.unitID" :label="unit.unitName" :value="unit.unitID"/>
                </el-select>
            </el-form-item>
            <el-form-item label="换算率" prop="ConversionRate">
                <el-input-number v-model="AddGoodsUnitModel.ConversionRate" :min="1" :precision="2" :disabled="AddGoodsUnitModel.IsBaseUnit" style="width: 100%;" />
            </el-form-item>
            <el-form-item label="基础单位" prop="IsBaseUnit">
                <el-switch v-model="AddGoodsUnitModel.IsBaseUnit" active-text="是" inactive-text="否" @change="ChangeBaseUnit"/>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="AddGoodsUnitDialogVisible = false">取消</el-button>
            <el-button type="primary" @click="AddGoodsUnit">确定</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { GetUnitOptionsApi } from '../../../api/unit'
import { AddGoodsUnitApi } from '../../../api/goodsUnit'
import { ElMessage } from 'element-plus'


const AddGoodsUnitDialogVisible = ref(false)

const CurrentGoodsTitle = ref('')

const AddGoodsUnitModel = reactive({
    GoodsID: 0,
    UnitID: null,
    ConversionRate: 1,
    IsBaseUnit: false
})

const AddGoodsUnitFormRef = ref()

function OpenDialog(goodsID, goodsTitle) {
    AddGoodsUnitModel.GoodsID = goodsID
    CurrentGoodsTitle.value = goodsTitle
    GetUnitOptions()
    AddGoodsUnitDialogVisible.value = true
}

defineExpose({
    OpenDialog
})

const UnitList = ref([])
async function GetUnitOptions() {
    try {
        const res = await GetUnitOptionsApi()
        if (res.code === 200) {
            UnitList.value = res.data
        }
    } catch (error) {
        console.log('获取单位列表失败：', error)
    }
}

function ChangeBaseUnit(value) {
    if (value) {
        AddGoodsUnitModel.ConversionRate = 1
    }
}

const Rules = {
    UnitID: [{ required: true, message: '请选择单位', trigger: 'change' }],
    ConversionRate: [{ required: true, message: '请输入换算率', trigger: 'change' }]
}

const emit = defineEmits(['success'])
async function AddGoodsUnit() {
    try{
        await AddGoodsUnitFormRef.value.validate()
        const res = await AddGoodsUnitApi(AddGoodsUnitModel)
        if (res.code === 200) {
            ElMessage.success(res.message)
            AddGoodsUnitDialogVisible.value = false
            emit('success')
        } else {
            ElMessage.error(res.message)
        }
    } catch (error) {
        console.log('新增商品单位失败：', error)
    }
}

function ResetForm() {
    AddGoodsUnitFormRef.value?.resetFields()
}
</script>