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
           <el-form-item label="商品单位" prop="unit">
                <el-select v-model="EditGoodsModel.unit" placeholder="请选择商品单位">
                    <el-option label="本" value="本" />
                    <el-option label="册" value="册" />
                    <el-option label="套" value="套" />
                    <el-option label="个" value="个" />
                    <el-option label="件" value="件" />
                </el-select>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="EditGoodsDialogVisible=false">取消</el-button>
            <el-button type="primary" @click="UpdateGoods">保存</el-button>
        </template>
    </el-dialog>
</template>


<script setup>
import { reactive, ref, watch, onMounted } from 'vue'
import { UpdateGoodsApi } from '../../../api/goods'
import { ElMessage } from 'element-plus'
import { GetGoodsCategoryListApi } from '../../../api/goodsCategory'

const EditGoodsDialogVisible = defineModel()
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
    goodsPrice:0,
    unit:""
})

//接收父组件数据
const props = defineProps({
    Goods:{
        type:Object,
        default:()=>({})
    }
})

//通知父组件
const emit = defineEmits([
    "success"
])

//数据回显
watch(()=>props.Goods,(newValue)=>{
    Object.assign(EditGoodsModel,newValue),{ deep:true, immediate:true}
})

const Rules = {
    barCode:[{ required:true, message:"请输入商品条码", trigger:"blur" }],
    goodsTitle:[{ required:true, message:"请输入商品名称", trigger:"blur" }],
    categoryID:[{ required:true, message:"请选择商品分类", trigger:"change" }],
    goodsPrice:[{ required:true, message:"请输入商品价格", trigger:"blur" }],
    unit:[{ required:true, message:"请选择商品单位", trigger:"change" }]
}

//获取分类
const CategoryList = ref([])
async function GetGoodsCategoryList(){
    const res = await GetGoodsCategoryListApi()
    if(res.code===200){
        CategoryList.value = res.data
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
            ElMessage.success("修改成功")
            EditGoodsDialogVisible.value = false
            emit("success")
        }
    } catch(error){
        console.log("修改商品失败：",error)
        ElMessage.error("修改商品失败")
    }
}
</script>