<template>
    <el-dialog v-model="AddLocationDialogVisible" title="新增站点" width="600px" @closed="ResetForm">
        <el-form :model="AddLocationModel" ref="AddLocationFormRef" :rules="rules">
            <el-form-item label="所属公司" prop="CompanyID">
                 <el-select v-model="AddLocationModel.CompanyID" placeholder="请选择所属公司" style="width: 100%;" @change="HandleCompanyChange">
                    <el-option v-for="company in props.CompanyList" :key="company.companyID" :label="company.companyName" :value="company.companyID"/>
                </el-select>
            </el-form-item>
            <el-form-item label="父级站点" prop="ParentID">
                <el-tree-select v-model="AddLocationModel.ParentID" :data="LocationTree" placeholder="请选择父级站点" clearable :disabled="!AddLocationModel.CompanyID"
                    :props="{
                        value: 'locationID',
                        label: 'locationName',
                        children: 'children'
                    }"
                    style="width: 100%;"
                />
            </el-form-item>
            <el-form-item label="站点编码" prop="LocationCode">
                <el-input v-model="AddLocationModel.LocationCode" placeholder="请输入站点编码"/>
            </el-form-item>
            <el-form-item label="站点名称" prop="LocationName">
                <el-input v-model="AddLocationModel.LocationName" placeholder="请输入站点名称"/>
            </el-form-item>
            <el-form-item label="所在地区">
                <el-cascader v-model="AreaValue" :props="AreaProps" placeholder="请选择省市区" clearable style="width: 100%;" @change="HandleAreaChange"/>
            </el-form-item>
            <el-form-item label="详细地址" prop="Address">
                <el-input v-model="AddLocationModel.Address" placeholder="请输入详细地址"/>
            </el-form-item>
            <el-form-item label="联系人" prop="Contact">
                <el-input v-model="AddLocationModel.Contact" placeholder="请输入联系人"/>
            </el-form-item>
            <el-form-item label="联系电话" prop="Phone">
                <el-input v-model="AddLocationModel.Phone" placeholder="请输入联系电话"/>
            </el-form-item>
            <el-form-item label="站点功能">
                <el-checkbox v-model="AddLocationModel.CanSale">
                    允许销售
                </el-checkbox>
                <el-checkbox v-model="AddLocationModel.CanStock">
                    允许库存
                </el-checkbox>
                <el-checkbox v-model="AddLocationModel.CanReceive">
                    允许收货
                </el-checkbox>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="AddLocationDialogVisible = false">取消</el-button>
            <el-button type="primary" @click="AddLocation">确定</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { ElMessage } from 'element-plus'
import { GetLocationTreeApi, AddLocationApi } from '@/api/location'
import { GetAreaTreeApi } from '@/api/area'

const AddLocationFormRef = ref(null)
const AddLocationModel = reactive({
    CompanyID: null,
    LocationCode: '',
    LocationName: '',
    ParentID: null,
    CanSale: false,
    CanStock: false,
    CanReceive: false,
    ProvinceID: null,
    CityID: null,
    DistrictID: null,
    Address: '',
    Contact: '',
    Phone: ''
})

const AddLocationDialogVisible = ref(false)
function OpenDialog (){
    AddLocationDialogVisible.value = true
}
defineExpose({
    OpenDialog
})

const props = defineProps({
    CompanyList: {
        type: Array,
        default: () => []
    }
})

const emit = defineEmits(['success'])
async function AddLocation() {
    try {
        await AddLocationFormRef.value.validate()
        const res = await AddLocationApi(AddLocationModel)
        if (res.code === 200) {
            ElMessage.success(res.message)
            AddLocationDialogVisible.value = false
            emit('success')
        } else {
            ElMessage.error(res.message)
        }
    } catch (e) {
        console.log('新增站点失败：', e)
    }
}

const LocationTree = ref([])
async function HandleCompanyChange() {
    AddLocationModel.ParentID = null
    LocationTree.value = []
    if (!AddLocationModel.CompanyID) {
        return
    }
    const res = await GetLocationTreeApi({
        CompanyID: AddLocationModel.CompanyID
    })
    if (res.code === 200) {
        LocationTree.value = res.data
    }
}

//地区选择
const AreaValue = ref([])
const AreaProps = {
    value: 'areaCode',
    label: 'areaName',
    lazy: true,
    lazyLoad: LoadArea,
    leaf: 'leaf'
}
async function LoadArea(node, resolve) {
    const ParentID = node.level === 0 ? null : node.data.areaID
    const res = await GetAreaTreeApi({
        ParentID: ParentID
    })
    if (res.code === 200) {
        const nodes = res.data.map(item => ({
            ...item,
            leaf: node.level >= 2
        }))
        resolve(nodes)
    } else {
        resolve([])
    }
}
function HandleAreaChange(value) {
    AddLocationModel.ProvinceID = value?.[0] ?? null
    AddLocationModel.CityID = value?.[1] ?? null
    AddLocationModel.DistrictID = value?.[2] ?? null
}

const rules = {
    CompanyID: [{ required: true, message: '请选择所属公司', trigger: 'change' }],
    LocationCode: [{ required: true, message: '请输入站点编码', trigger: 'blur' }],
    LocationName: [{ required: true, message: '请输入站点名称', trigger: 'blur' }]
}

function ResetForm() {
    AddLocationFormRef.value?.resetFields()
    AddLocationModel.ProvinceID = null
    AddLocationModel.CityID = null
    AddLocationModel.DistrictID = null
    AddLocationModel.CanSale = false
    AddLocationModel.CanStock = false
    AddLocationModel.CanReceive = false
    AreaValue.value = []
    LocationTree.value = []
}
</script>