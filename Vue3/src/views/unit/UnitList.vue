<template>
    <div>
        <h2>单位管理</h2>
    </div>
    <div>
        <el-input v-model="SearchModel.UnitName" placeholder="请输入单位名称" style="width:200px" clearable></el-input>
        <el-input v-model="SearchModel.UnitCode" placeholder="请输入单位编码" style="width:200px" clearable></el-input>
        <el-select v-model="SearchModel.IsActive" placeholder="请选择状态" style="width:150px" clearable>
            <el-option label="启用" :value="true" />
            <el-option label="禁用" :value="false" />
        </el-select>
        <el-button @click="SearchUnit">查询</el-button>
        <el-button type="primary" @click="OpenAddUnit">新增单位</el-button>
    </div>
    <div>
        <el-table :data="UnitList">
            <el-table-column label="单位ID" prop="unitID"></el-table-column>
            <el-table-column label="单位名称" prop="unitName"></el-table-column>
            <el-table-column label="单位编码" prop="unitCode"></el-table-column>
            <el-table-column label="排序" prop="sort"></el-table-column>
            <el-table-column label="创建时间" prop="createTime"></el-table-column>
            <el-table-column label="是否启用">
                <template #default="scope">
                    <el-switch v-model="scope.row.isActive" active-text="启用" inactive-text="禁用" @change="UpdateUnitStatus(scope.row)"/>
                </template>
            </el-table-column>
            <el-table-column label="操作">
                <template #default="scope">
                    <el-button type="primary" @click="OpenEditUnit(scope.row)">编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
    <div class="page-box">
        <el-pagination
            v-model:current-page="SearchModel.Page"
            v-model:page-size="SearchModel.PageSize"
            :page-sizes="[10,20,50,100]"
            :total="TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @current-change="GetUnitList"
            @size-change="ChangePageSize"
        />
    </div>
    <AddUnit ref="AddUnitRef" @success="GetUnitList"/>
    <EditUnit ref="EditUnitRef" @success="GetUnitList"/>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { GetUnitListApi, UpdateUnitStatusApi } from '@/api/unit'
import { ElMessage } from 'element-plus'
import AddUnit from './components/AddUnit.vue'
import EditUnit from './components/EditUnit.vue'

const UnitList = ref([])
const TotalCount = ref(0)
const AddUnitRef = ref(null)
const EditUnitRef = ref(null)

const SearchModel = reactive({
    UnitName: "",
    UnitCode: "",
    IsActive: null,
    Page: 1,
    PageSize: 10
})

onMounted(() => {
    GetUnitList()
})

const GetUnitList = async () => {
    try {
        const res = await GetUnitListApi(SearchModel)
        if (res.code === 200) {
            UnitList.value = res.data.list
            TotalCount.value = res.data.totalCount
        }
        else {
            ElMessage.error(res.message)
        }
    }
    catch (error) {
        ElMessage.error("查询单位失败")
        console.log(error)
    }
}

const SearchUnit = () => {
    SearchModel.Page = 1
    GetUnitList()
}

function ChangePageSize(){
    SearchModel.Page = 1
    GetUnitList()
}

async function UpdateUnitStatus(row){
    const oldStatus = !row.isActive
    const data = {
        UnitID: row.unitID,
        IsActive: row.isActive
    }
    try{
        const res = await UpdateUnitStatusApi(data)
        if(res.code === 200){
            ElMessage.success("修改成功")
        }
        else{
            ElMessage.error(res.message)
            row.isActive = oldStatus
        }
    }
    catch(error){
        ElMessage.error("修改失败")
        row.isActive = oldStatus
        console.log(error)
    }
}

function OpenAddUnit(){
    AddUnitRef.value.OpenDialog()
}

function OpenEditUnit(row){
    EditUnitRef.value.OpenDialog(row)
}
</script>

<style></style>