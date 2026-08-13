<template>
    <el-dialog v-model="EditLocationDialogVisible" title="编辑站点" width="600px" @closed="ResetForm">
        <el-form :model="EditLocationModel" ref="EditLocationFormRef" :rules="rules">
            <el-form-item label="所属公司">
                <el-select v-model="EditLocationModel.CompanyID" placeholder="请选择所属公司" style="width: 100%;" disabled>
                    <el-option v-for="company in props.CompanyList" :key="company.companyID" :label="company.companyName" :value="company.companyID"/>
                </el-select>
            </el-form-item>
            <el-form-item label="父级站点">
                <el-tree-select v-model="EditLocationModel.ParentID" :data="LocationTree" placeholder="请选择父级站点" clearable
                    :props="{
                        value: 'locationID',
                        label: 'locationName',
                        children: 'children'
                    }"
                    style="width: 100%;"
                />
            </el-form-item>
            <el-form-item label="站点编码">
                <el-input v-model="EditLocationModel.LocationCode" placeholder="请输入站点编码" disabled/>
            </el-form-item>
            <el-form-item label="站点名称" prop="LocationName">
                <el-input v-model="EditLocationModel.LocationName" placeholder="请输入站点名称"/>
            </el-form-item>
            <el-form-item label="所在地区">
                <el-cascader v-model="AreaValue" :props="AreaProps" placeholder="请选择省市区" clearable style="width: 100%;" @change="HandleAreaChange"/>
            </el-form-item>
            <el-form-item label="详细地址">
                <el-input v-model="EditLocationModel.Address" placeholder="请输入详细地址"/>
            </el-form-item>
            <el-form-item label="联系人">
                <el-input v-model="EditLocationModel.Contact" placeholder="请输入联系人"/>
            </el-form-item>
            <el-form-item label="联系电话">
                <el-input v-model="EditLocationModel.Phone" placeholder="请输入联系电话"/>
            </el-form-item>
            <el-form-item label="站点功能">
                <el-checkbox v-model="EditLocationModel.CanSale">
                    允许销售
                </el-checkbox>
                <el-checkbox v-model="EditLocationModel.CanStock">
                    允许库存
                </el-checkbox>
                <el-checkbox v-model="EditLocationModel.CanReceive">
                    允许收货
                </el-checkbox>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="EditLocationDialogVisible = false">取消</el-button>
            <el-button type="primary" @click="UpdateLocation">确定</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { GetLocationTreeApi,UpdateLocationApi } from '@/api/location'
import { GetAreaTreeApi } from '@/api/area'
import { ElMessage } from 'element-plus'

const EditLocationDialogVisible = ref(false)
const EditLocationFormRef = ref(null)

const EditLocationModel = reactive({
    LocationID: null,
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
    EditLocationModel.ProvinceID = value?.[0] ?? null
    EditLocationModel.CityID = value?.[1] ?? null
    EditLocationModel.DistrictID = value?.[2] ?? null
}

// 打开编辑弹窗并回显数据
async function OpenDialog(row) {
    EditLocationModel.LocationID = row.locationID
    EditLocationModel.CompanyID = row.companyID
    EditLocationModel.LocationCode = row.locationCode
    EditLocationModel.LocationName = row.locationName
    EditLocationModel.ParentID = row.parentID
    EditLocationModel.CanSale = row.canSale
    EditLocationModel.CanStock = row.canStock
    EditLocationModel.CanReceive = row.canReceive
    EditLocationModel.ProvinceID = row.provinceID
    EditLocationModel.CityID = row.cityID
    EditLocationModel.DistrictID = row.districtID
    EditLocationModel.Address = row.address
    EditLocationModel.Contact = row.contact
    EditLocationModel.Phone = row.phone

    AreaValue.value = [
        row.provinceID,
        row.cityID,
        row.districtID
    ]

    await GetLocationTree()

    EditLocationDialogVisible.value = true
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

const LocationTree = ref([])
async function GetLocationTree() {
    if (!EditLocationModel.CompanyID) {
        LocationTree.value = []
        return
    }
    const res = await GetLocationTreeApi({
        CompanyID: EditLocationModel.CompanyID
    })
    if (res.code === 200) {
        LocationTree.value = res.data
    }
}

const emit = defineEmits(['success'])
const UpdateLocation = async () =>{
    try {
        await EditLocationFormRef.value.validate()
        const res = await UpdateLocationApi(EditLocationModel)
        if (res.code === 200) {
            ElMessage.success(res.message)
            EditLocationDialogVisible.value = false
            emit('success')
        } else {
            ElMessage.error(res.message)
        }
    } catch (e) {
        ElMessage.error('修改站点失败')
        console.log('修改站点失败：', e)
    }
}

const rules = {
    LocationName: [{ required: true, message: '请输入站点名称', trigger: 'blur' }]
}

function ResetForm() {
    EditLocationFormRef.value?.resetFields()

    EditLocationModel.ProvinceID = null
    EditLocationModel.CityID = null
    EditLocationModel.DistrictID = null

    EditLocationModel.CanSale = false
    EditLocationModel.CanStock = false
    EditLocationModel.CanReceive = false

    AreaValue.value = []
    LocationTree.value = []
}
</script>