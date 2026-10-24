<template>
    <!-- 查询区域 -->
    <el-form :inline="true" :model="SearchModel">
        <el-form-item label="到货单号">
            <el-input v-model="SearchModel.ArrivalCode" placeholder="请输入到货单号" clearable/>
        </el-form-item>
        <el-form-item label="站点名称">
            <el-select v-model="SearchModel.LocationID" placeholder="请选择站点" clearable>
                <el-option v-for="item in LocationList" :key="item.locationID" :label="item.locationName" :value="item.locationID"></el-option>
            </el-select>
        </el-form-item>
        <el-form-item label="到货状态">
            <el-select v-model="SearchModel.ArrivalStatus" placeholder="请选择到货状态" clearable style="width: 160px">
                <el-option v-for="item in ArrivalStatusOptions" :key="item.value" :label="item.label" :value="item.value"/>
            </el-select>
        </el-form-item>
        <el-form-item label="创建日期">
            <el-date-picker
                v-model="DateRange"
                type="daterange"
                range-separator="至"
                start-placeholder="开始日期"
                end-placeholder="结束日期"
                value-format="YYYY-MM-DD"
            />
        </el-form-item>
        <el-form-item>
            <el-button type="primary" @click="HandleSearch">查询</el-button>
        </el-form-item>
    </el-form>

    <!-- 操作区域 -->
    <el-button type="primary" @click="OpenAddArrival">新增到货单</el-button>

    <!-- 到货单列表区域 -->
    <el-table :data="ArrivalList" border style="width: 100%; margin-top: 15px;">
        <el-table-column label="到货状态">
            <template #default="scope">
                {{ GetArrivalStatusLabel(scope.row.arrivalStatus) }}
            </template>
        </el-table-column>
        <el-table-column prop="arrivalCode" label="到货单号" />
        <el-table-column prop="supplierName" label="供应商" />
        <el-table-column prop="locationName" label="站点" />
        <el-table-column prop="totalAmount" label="到货金额" />
        <el-table-column prop="arrivalTime" label="到货时间" />
        <el-table-column prop="createUserName" label="创建人" />
        <el-table-column prop="createTime" label="创建时间" />
    </el-table>
    <!-- 分页区域 -->
    <el-pagination
        v-model:current-page="SearchModel.Page"
        v-model:page-size="SearchModel.PageSize"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        :total="Total"
        @size-change="HandleSizeChange"
        @current-change="HandleCurrentChange"
    />
    <AddArrival ref="AddArrivalRef" />
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'
import { GetArrivalListApi } from '../../api/arrival';
import { ElMessage } from 'element-plus';
import { ArrivalStatus, ArrivalStatusOptions } from '../../enums/arrivalStatus';
import { GetLocationListApi } from '../../api/location';
import AddArrival from './components/AddArrival.vue'

onMounted(() => {
    GetArrivalList()
    GetLocationList()
})

const SearchModel = reactive({
    ArrivalCode: "",
    LocationID: null,
    ArrivalStatus: null,
    StartDate: null,
    EndDate: null,
    Page: 1,
    PageSize: 10
})

const GetArrivalStatusLabel = (status) => {
    const result = ArrivalStatusOptions.find(item => item.value === status)
    return result?.label ?? '未知状态'
}

const DateRange = ref([])
const ArrivalList = ref([])
const Total = ref(0)
const LocationList = ref([])
const GetArrivalList = async () => {
    try{
        if (DateRange.value && DateRange.value.length === 2) {
            SearchModel.StartDate = DateRange.value[0]
            SearchModel.EndDate = DateRange.value[1]
        } else {
            SearchModel.StartDate = null
            SearchModel.EndDate = null
        }
        const res = await GetArrivalListApi(SearchModel)
        if(res.code == 200){
            ArrivalList.value = res.data.list
            Total.value = res.data.totalCount
        }else{
            console.log(res.message)
            ElMessage.error(res.message)
        }
    }catch(e){
        console.log(e)
        ElMessage.error("查询到货单失败")
    }
}

// 每页条数改变
const HandleSizeChange = (pageSize) => {
    SearchModel.PageSize = pageSize
    SearchModel.Page = 1
    GetArrivalList()
}

// 当前页改变
const HandleCurrentChange = (page) => {
    SearchModel.Page = page
    GetArrivalList()
}

const HandleSearch = () => {
    SearchModel.Page = 1
    GetArrivalList()
}

const GetLocationList = async () => {
    try {
        const res = await GetLocationListApi({
            Page: 1,
            PageSize: 200
        })
        if (res.code == 200) {
            LocationList.value = res.data.list
        } else {
            ElMessage.error(res.message)
        }
    } catch (e) {
        console.log(e)
        ElMessage.error("查询站点失败")
    }
}

const AddArrivalRef = ref(null)
function OpenAddArrival() {
    AddArrivalRef.value.OpenDialog()
}
</script>