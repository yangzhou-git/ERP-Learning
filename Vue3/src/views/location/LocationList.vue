<template>
    <div class="location-page">
        <h2>站点管理</h2>
        <div class="search-box">
            <el-input v-model="SearcherModel.LocationName" placeholder="请输入站点名称" style="width: 200px;" clearable/>
            <el-select v-model="SearcherModel.CompanyID" placeholder="请选择公司" clearable style="width: 220px;">
                <el-option v-for="company in CompanyList" :key="company.companyID" :label="company.companyName" :value="company.companyID"/>
            </el-select>
            <el-select v-model="SearcherModel.IsActive" placeholder="请选择状态" clearable style="width: 140px;">
                <el-option label="启用" :value="true"></el-option>
                <el-option label="禁用" :value="false"></el-option>
            </el-select>
            <el-button type="primary" @click="HandleSearch">查询</el-button>
            <el-button type="success" @click="OpenAddLocation">新增站点</el-button>
        </div>

        <div class="table-box">
            <el-table :data="LocationList">
                <el-table-column label="站点ID" prop="locationID"></el-table-column>
                <el-table-column label="站点编码" prop="locationCode"></el-table-column>
                <el-table-column label="站点名称" prop="locationName"></el-table-column>
                <el-table-column label="所属公司" prop="companyName"></el-table-column>
                <el-table-column label="联系人" prop="contact"></el-table-column>
                <el-table-column label="联系电话" prop="phone"></el-table-column>
                <el-table-column label="详细地址" prop="address"></el-table-column>
                <el-table-column label="允许销售">
                    <template #default="scope">
                        {{ scope.row.canSale ? '允许' : '不允许' }}
                    </template>
                </el-table-column>
                <el-table-column label="允许库存">
                    <template #default="scope">
                        {{ scope.row.canStock ? '允许' : '不允许' }}
                    </template>
                </el-table-column>
                <el-table-column label="允许收货">
                    <template #default="scope">
                        {{ scope.row.canReceive ? '允许' : '不允许' }}
                    </template>
                </el-table-column>
                <el-table-column label="状态">
                    <template #default="scope">
                        <el-switch v-model="scope.row.isActive" active-text="启用" inactive-text="禁用" @change="UpdateLocationStatus(scope.row)"></el-switch>
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="120">
                    <template #default="scope">
                        <el-button type="primary" @click="OpenEditLocation(scope.row)">编辑</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <div class="pagination-box">
            <el-pagination
                v-model:current-page="SearcherModel.Page"
                v-model:page-size="SearcherModel.PageSize"
                :total="TotalCount"
                :page-sizes="[10, 20, 50, 100]"
                layout="total, sizes, prev, pager, next, jumper"
                @current-change="GetLocationList"
                @size-change="HandleSizeChange"
            />
        </div>
        <AddLocation ref="AddLocationRef" :CompanyList="CompanyList" @success="GetLocationList"/>
        <EditLocation ref="EditLocationRef" :CompanyList="CompanyList" @success="GetLocationList"/>
    </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { GetLocationListApi, UpdateLocationStatusApi } from '../../api/location'
import { GetActiveCompanyListApi } from '../../api/company'
import AddLocation from './components/AddLocation.vue'
import EditLocation from './components/EditLocation.vue'

onMounted(() => {
    GetLocationList()
    GetActiveCompanyList()
})

const SearcherModel = reactive({
    LocationName: '',
    CompanyID: null,
    IsActive: null,
    Page: 1,
    PageSize: 10
})

const LocationList = ref([])
const TotalCount = ref(0)
async function GetLocationList() {
    try {
        const res = await GetLocationListApi(SearcherModel)
        if (res.code === 200) {
            LocationList.value = res.data.list
            TotalCount.value = res.data.totalCount
        } else {
            ElMessage.error(res.message)
        }
    } catch (error) {
        ElMessage.error('查询站点列表失败')
        console.log('查询站点列表失败：', error)
    }
}

function HandleSearch() {
    SearcherModel.Page = 1
    GetLocationList()
}

function HandleSizeChange() {
    SearcherModel.Page = 1
    GetLocationList()
}

async function UpdateLocationStatus(row) {
    const data = {
        LocationID: row.locationID,
        IsActive: row.isActive
    }
    try {
        const res = await UpdateLocationStatusApi(data)
        if (res.code === 200) {
            ElMessage.success(res.message)
        } else {
            row.isActive = !row.isActive
            ElMessage.error(res.message)
        }
    } catch (error) {
        row.isActive = !row.isActive
        ElMessage.error('修改站点状态失败')
        console.log('修改站点状态失败：', error)
    }
}

const CompanyList = ref([])
async function GetActiveCompanyList() {
    try {
        const res = await GetActiveCompanyListApi()
        if (res.code === 200) {
            CompanyList.value = res.data
        } else {
            ElMessage.error(res.message)
        }
    } catch (error) {
        ElMessage.error('查询公司列表失败')
        console.log('查询公司列表失败：', error)
    }
}
const AddLocationRef = ref(null)
const OpenAddLocation = () => {
     AddLocationRef.value.OpenDialog()
}

const EditLocationRef = ref(null)
function OpenEditLocation(row) {
    EditLocationRef.value.OpenDialog({ ...row })
}
</script>

<style scoped>

</style>