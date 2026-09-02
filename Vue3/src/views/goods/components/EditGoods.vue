<template>
    <el-dialog v-model="EditGoodsDialogVisible" title="编辑商品" width="600px" @closed="ResetForm">
        <el-form :model="EditGoodsModel" :rules="Rules" ref="EditGoodsFormRef">
            <el-form-item label="商品条码" prop="barCode">
                <el-input v-model="EditGoodsModel.barCode" placeholder="请输入商品条码"/>
            </el-form-item>
            <el-form-item label="商品编号" prop="goodsCode">
                <el-input v-model="EditGoodsModel.goodsCode" placeholder="请输入商品编号"/>
            </el-form-item>
            <el-form-item label="商品分类" prop="categoryID">
                <el-select v-model="EditGoodsModel.categoryID">
                    <el-option v-for="Category in CategoryList" :key="Category.categoryID" :label="Category.categoryName" :value="Category.categoryID"/>
                </el-select>
            </el-form-item>
            <el-form-item label="商品名称" prop="goodsTitle">
                <el-input v-model="EditGoodsModel.goodsTitle"  placeholder="请输入商品名称"/>
            </el-form-item>
            <el-form-item label="商品副标题" prop="goodsSubTitle">
                <el-input v-model="EditGoodsModel.goodsSubTitle" placeholder="请输入商品副标题"/>
            </el-form-item>
            <el-form-item label="商品价格" prop="goodsPrice">
                <el-input-number v-model="EditGoodsModel.goodsPrice" :min="0" :precision="2"/>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="EditGoodsDialogVisible=false">取消</el-button>
            <el-button type="primary" @click="UpdateGoods">保存</el-button>
        </template>
    </el-dialog>
</template>


<script setup>
import { reactive, ref, onMounted } from 'vue'
import { UpdateGoodsApi } from '../../../api/goods'
import { ElMessage } from 'element-plus'
import { GetGoodsCategoryListApi } from '../../../api/goodsCategory'

const EditGoodsDialogVisible = ref(false)
const EditGoodsFormRef = ref()

onMounted(()=>{
    GetGoodsCategoryList()
})

const EditGoodsModel = reactive({
    goodsID:null,
    barCode:"",
    goodsCode:"",
    categoryID:null,
    goodsTitle:"",
    goodsSubTitle:"",
    goodsPrice:0
})

//通知父组件
const emit = defineEmits([
    "success"
])

const Rules = {
    goodsCode: [{ required: true, message: '请输入商品编码', trigger: 'blur' }],
    barCode:[{ required:true, message:"请输入商品条码", trigger:"blur" }],
    goodsTitle:[{ required:true, message:"请输入商品名称", trigger:"blur" }],
    categoryID:[{ required:true, message:"请选择商品分类", trigger:"change" }],
    goodsPrice:[{ required:true, message:"请输入商品价格", trigger:"blur" }]
}

//获取分类
const CategoryList = ref([])
async function GetGoodsCategoryList(){
    const res = await GetGoodsCategoryListApi()
    if(res.code===200){
        CategoryList.value = res.data.list
    } else {
        ElMessage.error(res.message)
    }
}

//清除表单验证
function ResetForm(){
    EditGoodsFormRef.value?.clearValidate()
}

//修改商品资料
const UpdateGoods = async () => {
    try {
        await EditGoodsFormRef.value.validate()
        const res = await UpdateGoodsApi(EditGoodsModel)
        if(res.code === 200){
            ElMessage.success(res.message)
            EditGoodsDialogVisible.value = false
            emit("success")
        } else {
            ElMessage.error(res.message)
        }
    } catch(error){
        console.log("修改商品失败：",error)
        ElMessage.error(res.message)
    }
}

function OpenDialog(row) {
    Object.assign(EditGoodsModel, row)
    EditGoodsDialogVisible.value = true
}

defineExpose({
    OpenDialog
})
</script>