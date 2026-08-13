<template>
    <div class="edit-category-dialog">
        <el-dialog title="编辑商品分类" v-model="EditGoodsCategoryDialogVisible" @closed="ResetForm">
            <el-form  :model="GoodsCategoryModel" :rules="rules" ref="EditGoodsCategoryFormRef">
                <el-form-item label="分类名称" prop="CategoryName">
                    <el-input v-model="GoodsCategoryModel.CategoryName"/>
                </el-form-item>
                <el-form-item label="分类编码" prop="CategoryCode">
                    <el-input v-model="GoodsCategoryModel.CategoryCode"/>
                </el-form-item>
                <el-form-item label="父级分类" prop="ParentID">
                    <el-tree-select v-model="GoodsCategoryModel.ParentID" :data="CategoryTreeList" check-strictly/>
                </el-form-item>
                <el-form-item label="排序" prop="Sort">
                    <el-input-number v-model="GoodsCategoryModel.Sort" :min="1"/>
                </el-form-item>
            </el-form>
            <template #footer>
                <el-button @click="EditGoodsCategoryDialogVisible=false">取消</el-button>
                <el-button type="primary" @click="UpdateGoodsCategory">保存</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { GetGoodsCategoryTreeApi, UpdateGoodsCategoryApi } from '@/api/goodsCategory'
import { ElMessage } from 'element-plus'

const props = defineProps({
    GoodsCategory:{
        type:Object,
        default:()=>({})
    }
})
const GoodsCategoryModel = reactive({
    CategoryID:null,
    CategoryName:'',
    CategoryCode:'',
    ParentID:null,
    Sort:1
})
watch(
    ()=>props.GoodsCategory,
    (newValue)=>{
        if(newValue){
            GoodsCategoryModel.CategoryID = newValue.categoryID
            GoodsCategoryModel.CategoryName = newValue.categoryName
            GoodsCategoryModel.CategoryCode = newValue.categoryCode
            GoodsCategoryModel.ParentID = newValue.parentID
            GoodsCategoryModel.Sort = newValue.sort
        }
    },
    {
        immediate:true
    }
)

const EditGoodsCategoryDialogVisible = ref(false)
const OpenDialog = async () => {
    await GetCategoryTree()
    EditGoodsCategoryDialogVisible.value = true
}
defineExpose({
    OpenDialog
})

const CategoryTreeList = ref([])
const GetCategoryTree = async()=>{
    try{
        const res = await GetGoodsCategoryTreeApi()
        if(res.code === 200){
            CategoryTreeList.value = TransformTreeData(res.data)
        }else{
            ElMessage.error(res.message)
        }
    }catch(error){
        console.log("获取分类树失败",error)
    }
}
const TransformTreeData = (list)=>{
    return list.map(item=>{
        return {
            label:item.categoryName,
            value:item.categoryID,
            children:item.children
                ? TransformTreeData(item.children)
                : []
        }
    })
}
const EditGoodsCategoryFormRef = ref(null)
const rules = { 
    CategoryName: [{ required: true, message: '请输入分类名称', trigger: 'blur' }],
    CategoryCode: [{ required: true, message: '请输入分类编码', trigger: 'blur' }]
}
const ResetForm = () => {
    EditGoodsCategoryFormRef.value?.resetFields()
}
const emit = defineEmits(['success'])
const UpdateGoodsCategory = async()=>{
    await EditGoodsCategoryFormRef.value.validate()
    try{
        const res = await UpdateGoodsCategoryApi(GoodsCategoryModel)
        if(res.code === 200){
            ElMessage.success('编辑商品分类成功')
            EditGoodsCategoryDialogVisible.value = false
            emit('success')
        }else{
            ElMessage.error('编辑商品分类失败')
            console.log('编辑商品分类失败：',res.message)
        }
    }catch(error){
        console.log("编辑商品分类失败：",error)
    }
}
</script>