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
        <el-table-column label="到货单号" min-width="160">
            <template #default="scope">
                <el-button type="primary" link @click="OpenArrivalDetail(scope.row)">
                    {{ scope.row.arrivalCode }}
                </el-button>
            </template>
        </el-table-column>
        <el-table-column label="到货状态">
            <template #default="scope">
                {{ GetArrivalStatusLabel(scope.row.arrivalStatus) }}
            </template>
        </el-table-column>
        <el-table-column prop="supplierName" label="供应商" />
        <el-table-column prop="locationName" label="站点" />
        <el-table-column prop="totalAmount" label="到货金额" />
        <el-table-column prop="arrivalTime" label="到货时间" />
        <el-table-column prop="createUserName" label="创建人" />
        <el-table-column prop="createTime" label="创建时间" />
        <el-table-column label="操作" width="220" fixed="right" align="center">
            <template #default="{ row }">
                <!-- 第一组：审核 / 撤销审核 -->
                <el-button
                    v-if="row.arrivalStatus === ArrivalStatus.Pending"
                    type="success"
                    link
                    :loading="OperatingArrivalID === row.arrivalID"
                    :disabled="OperatingArrivalID !== null"
                    @click="UpdateArrivalAudit(row)"
                >
                    审核
                </el-button>

                <el-button
                    v-else
                    type="warning"
                    link
                    :loading="OperatingArrivalID === row.arrivalID"
                    :disabled="OperatingArrivalID !== null || row.arrivalStatus !== ArrivalStatus.Audited"
                    @click="CancelArrivalAudit(row)"
                >
                    撤销审核
                </el-button>

                <!-- 第二组：入库 / 撤销入库 -->
                <el-button
                    v-if="row.arrivalStatus === ArrivalStatus.Stocked"
                    type="danger"
                    link
                    :loading="OperatingArrivalID === row.arrivalID"
                    :disabled="OperatingArrivalID !== null"
                    @click="CancelStockInArrival(row)"
                >
                    撤销入库
                </el-button>

                <el-button
                    v-else
                    type="primary"
                    link
                    :loading="OperatingArrivalID === row.arrivalID"
                    :disabled="OperatingArrivalID !== null || row.arrivalStatus !== ArrivalStatus.Audited"
                    @click="StockInArrival(row)"
                >
                    入库
                </el-button>
                <el-button
                    type="primary"
                    link
                    :disabled="row.arrivalStatus !== ArrivalStatus.Pending || OperatingArrivalID !== null"
                    @click="OpenEditArrival(row)"
                >
                    编辑
                </el-button>

            </template>
        </el-table-column>
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
    <AddArrival ref="AddArrivalRef" @success="HandleArrivalAdded"/>
    <ArrivalDetail ref="ArrivalDetailDialogRef"/>
    <EditArrival ref ="EditArrivalRef" @success="GetArrivalList"/>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { GetArrivalListApi, UpdateArrivalAuditApi, StockInArrivalApi, CancelStockInArrivalApi, CancelArrivalAuditApi } from '@/api/arrival'
import { GetLocationListApi } from '@/api/location'
import { ArrivalStatus, ArrivalStatusOptions } from '@/enums/arrivalStatus'
import AddArrival from './components/AddArrival.vue'
import ArrivalDetail from './components/ArrivalDetail.vue'
import EditArrival from './components/EditArrival.vue'

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

// 新增到货单成功后刷新列表
async function HandleArrivalAdded() {
    // 返回第一页，方便看到新增加的到货单
    SearchModel.Page = 1
    // 重新查询到货单列表
    await GetArrivalList()
}

const ArrivalDetailDialogRef = ref(null)
// 打开到货单详情弹窗
function OpenArrivalDetail(row) {
    ArrivalDetailDialogRef.value?.OpenArrivalDetailDialog(row.arrivalID)
}

// 当前正在操作的到货单ID
const OperatingArrivalID = ref(null)

async function UpdateArrivalAudit(row) {
    if (OperatingArrivalID.value !== null) {
        return
    }

    if (row.arrivalStatus !== ArrivalStatus.Pending) {
        ElMessage.warning('只有未审核的到货单才能审核')
        return
    }

    // 先占用操作锁，避免确认弹窗期间重复点击
    OperatingArrivalID.value = row.arrivalID

    try {
        await ElMessageBox.confirm(
            `确定审核到货单【${row.arrivalCode}】吗？`,
            '审核到货单',
            {
                confirmButtonText: '确定审核',
                cancelButtonText: '取消',
                type: 'warning'
            }
        )

        const res = await UpdateArrivalAuditApi({
            ArrivalID: row.arrivalID
        })

        if (res.code === 200) {
            ElMessage.success(res.message || '审核到货单成功')
            await GetArrivalList()
        } else {
            ElMessage.error(res.message || '审核到货单失败')
        }
    } catch (error) {
        if (error !== 'cancel' && error !== 'close') {
            console.error('审核到货单失败：', error)
            ElMessage.error('审核到货单失败')
        }
    } finally {
        OperatingArrivalID.value = null
    }
}


 // 撤销审核到货单
async function CancelArrivalAudit(row) {
    if (OperatingArrivalID.value !== null) return

    if (row.arrivalStatus !== ArrivalStatus.Audited) {
        ElMessage.warning('只有已审核且未入库的到货单才能撤销审核')
        return
    }

    OperatingArrivalID.value = row.arrivalID

    try {
        await ElMessageBox.confirm(
            `确定撤销审核到货单【${row.arrivalCode}】吗？`,
            '撤销审核',
            {
                confirmButtonText: '确定撤销',
                cancelButtonText: '取消',
                type: 'warning'
            }
        )

        const res = await CancelArrivalAuditApi({
            ArrivalID: row.arrivalID
        })

        if (res.code === 200) {
            ElMessage.success(res.message || '撤销审核成功')
            await GetArrivalList()
        } else {
            ElMessage.error(res.message || '撤销审核失败')
        }
    } catch (error) {
        if (error !== 'cancel' && error !== 'close') {
            console.error('撤销审核失败：', error)
            ElMessage.error('撤销审核失败，请稍后重试')
        }
    } finally {
        OperatingArrivalID.value = null
    }
}

// 到货单入库
async function StockInArrival(row) {
    if (OperatingArrivalID.value !== null) return

    if (row.arrivalStatus !== ArrivalStatus.Audited) {
        ElMessage.warning('只有已审核的到货单才能入库')
        return
    }

    OperatingArrivalID.value = row.arrivalID

    try {
        await ElMessageBox.confirm(
            `确定将到货单【${row.arrivalCode}】入库吗？入库后将更新实际库存。`,
            '到货单入库',
            {
                confirmButtonText: '确定入库',
                cancelButtonText: '取消',
                type: 'warning'
            }
        )

        const res = await StockInArrivalApi({
            ArrivalID: row.arrivalID
        })

        if (res.code === 200) {
            ElMessage.success(res.message || '到货单入库成功')
            await GetArrivalList()
        } else {
            ElMessage.error(res.message || '到货单入库失败')
        }
    } catch (error) {
        if (error !== 'cancel' && error !== 'close') {
            console.error('到货单入库失败：', error)
            ElMessage.error('到货单入库失败，请稍后重试')
        }
    } finally {
        OperatingArrivalID.value = null
    }
}

// 撤销到货单入库
async function CancelStockInArrival(row) {
    if (OperatingArrivalID.value !== null) return

    if (row.arrivalStatus !== ArrivalStatus.Stocked) {
        ElMessage.warning('只有已入库的到货单才能撤销入库')
        return
    }

    OperatingArrivalID.value = row.arrivalID

    try {
        await ElMessageBox.confirm(
            `确定撤销到货单【${row.arrivalCode}】的入库吗？此操作将调整实际库存及库存流水。`,
            '撤销到货入库',
            {
                confirmButtonText: '确定撤销',
                cancelButtonText: '取消',
                type: 'warning'
            }
        )

        const res = await CancelStockInArrivalApi({
            ArrivalID: row.arrivalID
        })

        if (res.code === 200) {
            ElMessage.success(res.message || '撤销入库成功')
            await GetArrivalList()
        } else {
            ElMessage.error(res.message || '撤销入库失败')
        }
    } catch (error) {
        if (error !== 'cancel' && error !== 'close') {
            console.error('撤销入库失败：', error)
            ElMessage.error('撤销入库失败，请稍后重试')
        }
    } finally {
        OperatingArrivalID.value = null
    }
}

// 编辑到货单组件引用
const EditArrivalRef = ref(null)
// 打开编辑到货单弹窗
function OpenEditArrival(row) {
    // 只有未审核的到货单允许编辑
    if (row.arrivalStatus !== ArrivalStatus.Pending) {
        ElMessage.warning('只有未审核的到货单才能编辑')
        return
    }
    // 打开编辑弹窗，传递到货单ID
    EditArrivalRef.value?.OpenDialog(row.arrivalID)
}
</script>