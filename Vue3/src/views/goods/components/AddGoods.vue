<template>
    <el-dialog title="新增商品" width="600px" v-model="AddGoodsDialogVisible" @closed="ResetForm">
        <el-form :model="AddGoodsModel" :rules="Rules" ref="AddGoodsFormRef">
            <el-form-item label="商品条码" prop="BarCode">
                <el-input v-model="AddGoodsModel.BarCode" placeholder="请输入商品条码"/>
            </el-form-item>
            <el-form-item label="商品编码" prop="GoodsCode">
                <el-input v-model="AddGoodsModel.GoodsCode" placeholder="请输入商品编码"/>
            </el-form-item>
            <el-form-item label="商品名称" prop="GoodsTitle">
                <el-input v-model="AddGoodsModel.GoodsTitle" placeholder="请输入商品名称"/>
            </el-form-item>
            <el-form-item label="商品副标题" prop="GoodsSubTitle">
                <el-input v-model="AddGoodsModel.GoodsSubTitle" placeholder="请输入商品副标题"/>
            </el-form-item>
            <el-form-item label="商品分类" prop="CategoryID">
                <el-select v-model="AddGoodsModel.CategoryID" placeholder="请选择商品分类">
                    <el-option v-for="Category in CategoryList" :key="Category.categoryID" :label="Category.categoryName" :value="Category.categoryID"/>
                </el-select>
            </el-form-item>
            <el-form-item label="商品价格" prop="GoodsPrice">
                <el-input-number v-model="AddGoodsModel.GoodsPrice" :min="0" :precision="2" placeholder="请输入商品价格"/>
            </el-form-item>
            <el-button type="primary" @click="AddGoods">新增商品</el-button>
        </el-form>
    </el-dialog>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { GetGoodsCategoryListApi } from '@/api/goodsCategory/index.js'
import { AddGoodsApi } from '@/api/goods/index.js'
import { ElMessage } from 'element-plus'

onMounted(() => {
    GetGoodsCategoryList()
})

const AddGoodsDialogVisible = ref(false);
function OpenDialog(){
    AddGoodsDialogVisible.value = true;
}

defineExpose({
    OpenDialog
})

const AddGoodsModel = reactive({
    BarCode: '',
    GoodsCode: '',
    CategoryID: null,
    GoodsTitle: '',
    GoodsSubTitle: '',
    GoodsPrice: null
})
//获取分类
const CategoryList = ref([])
async function GetGoodsCategoryList() {
    try {
        const res = await GetGoodsCategoryListApi()
        if (res.code === 200) {
            CategoryList.value = res.data.list
        } else {
            ElMessage.error(res.message)
        }
    } catch (error) {
        ElMessage.error('获取商品分类失败')
        console.log('获取商品分类失败：', error)
    }
}
//表单校验
const AddGoodsFormRef = ref()
const Rules = {
    GoodsCode: [
        { required: true, message: '请输入商品编码', trigger: 'blur' }
    ],
    BarCode: [
        { required: true, message: '请输入商品条码', trigger: 'blur' }
    ],
    CategoryID: [
        { required: true, message: '请选择商品分类', trigger: 'change' }
    ],
    GoodsTitle: [
        { required: true, message: '请输入商品名称', trigger: 'blur' }
    ],
    GoodsPrice: [
        { required: true, message: '请输入商品价格', trigger: 'blur' }
    ]
}
const emit = defineEmits([
    'success'
])
//新增商品
async function AddGoods() {
    await AddGoodsFormRef.value.validate()
    try {
        const res = await AddGoodsApi(AddGoodsModel)
        if (res.code === 200) {
            ElMessage.success(res.message)
            AddGoodsDialogVisible.value = false
            emit('success')
        } else {
            ElMessage.error(res.message)
        }
    } catch (error) {
        ElMessage.error('新增商品失败')
        console.log('新增商品失败：', error)
    }
}
//关闭弹窗清空状态
function ResetForm() {
    AddGoodsFormRef.value.resetFields()
}
</script>

<style scoped>

</style>