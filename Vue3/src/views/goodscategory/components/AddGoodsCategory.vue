<template>
    <el-dialog title="新增商品分类" v-model="AddGoodsCategoryDialogVisible" @closed="ResetForm">
        <el-form :model="GoodsCategoryModel" :rules="rules" ref="AddGoodsCategoryFormRef">
            <el-form-item label="分类名称" prop="CategoryName">
                <el-input v-model="GoodsCategoryModel.CategoryName" placeholder="请输入分类名称"/>
            </el-form-item>
            <el-form-item label="分类编码" prop="CategoryCode">
                <el-input v-model="GoodsCategoryModel.CategoryCode" placeholder="请输入分类编码"/>
            </el-form-item>
            <el-form-item label="父级分类" prop="ParentID">
                <el-tree-select v-model="GoodsCategoryModel.ParentID" :data="CategoryTreeList" placeholder="请选择父级分类" check-strictly/>
            </el-form-item>
            <el-form-item label="排序" prop="Sort">
                <el-input-number v-model="GoodsCategoryModel.Sort" :min="1"/>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="AddGoodsCategoryDialogVisible=false">取消</el-button>
            <el-button type="primary" @click="AddGoodsCategory">保存</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { ElMessage } from 'element-plus'
import { AddGoodsCategoryApi, GetGoodsCategoryTreeApi } from '@/api/goodsCategory'

const AddGoodsCategoryDialogVisible = ref(false)
const GoodsCategoryModel = reactive({
    CategoryName:'',
    CategoryCode:'',
    ParentID:null,
    Sort:1
})
const rules = {
    CategoryName: [{ required: true, message: '请输入分类名称', trigger: 'blur' }],
    CategoryCode: [{ required: true, message: '请输入分类编码', trigger: 'blur' }]
}


const OpenDialog = () => {
    AddGoodsCategoryDialogVisible.value = true
    GetCategoryTree()
}
defineExpose({
    OpenDialog
})
const ResetForm = () => {
    AddGoodsCategoryFormRef.value?.resetFields()
}
const CategoryTreeList = ref([])
const GetCategoryTree = async()=>{
     try {
        const res = await GetGoodsCategoryTreeApi()
        if(res.code === 200){
            CategoryTreeList.value = TransformTreeData(res.data)
        }else{
            ElMessage.error(res.message)
        }
    }catch(error){
        console.log("获取分类树失败：",error)
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
const AddGoodsCategoryFormRef = ref(null)
const emit = defineEmits(['success'])
const AddGoodsCategory = async()=>{
    await AddGoodsCategoryFormRef.value.validate()
    try {
        const res = await AddGoodsCategoryApi(GoodsCategoryModel)
        if(res.code === 200){
            ElMessage.success('新增商品分类成功')
            AddGoodsCategoryDialogVisible.value = false
            emit('success')
        }else{
            ElMessage.error('新增商品分类失败')
            console.log('新增商品分类失败：',res.message)
        }
    }catch(error){
        console.log("新增商品分类失败：",error)
    }
}
</script>