<template>
    <el-dialog title="新增到货单" v-model="AddArrivalDialogVisible" width="95%" top="3vh" :close-on-click-modal="false" @closed="ResetAddArrivalForm">
        <h3>到货单基本信息</h3>
        <el-form ref="AddArrivalFormRef" :model="AddArrivalModel" :rules="AddArrivalRules" label-width="100px" style="width: 100%">
            <el-row :gutter="24">
                <!-- 供应商 -->
                <el-col :xs="24" :sm="12" :lg="8">
                    <el-form-item label="供应商" prop="SupplierID">
                        <el-select v-model="AddArrivalModel.SupplierID" placeholder="请选择或搜索供应商" filterable clearable :loading="SupplierLoading" style="width: 100%">
                            <el-option v-for="item in SupplierList" :key="item.supplierID"
                                :label="`${item.supplierCode} - ${item.supplierName}`"
                                :value="item.supplierID"
                            />
                            <template #footer>
                                <el-button type="primary" link @click.stop="OpenAddSupplier">
                                    + 录入供应商
                                </el-button>
                            </template>
                        </el-select>
                    </el-form-item>
                </el-col>
                <!-- 入库站点 -->
                <el-col :xs="24" :sm="12" :lg="8">
                    <el-form-item label="入库站点" prop="LocationID">
                        <el-select v-model="AddArrivalModel.LocationID" placeholder="请选择入库站点" filterable clearable :loading="LocationLoading" style="width: 100%">
                            <el-option v-for="item in LocationList" :key="item.locationID" :label="item.locationName" :value="item.locationID"/>
                            <template #footer>
                                <el-button type="primary" link @click.stop="OpenAddLocation">
                                    + 录入站点
                                </el-button>
                            </template>
                        </el-select>
                    </el-form-item>
                </el-col>
                <!-- 到货时间 -->
                <el-col :xs="24" :sm="12" :lg="8">
                    <el-form-item label="到货时间" prop="ArrivalTime">
                        <el-date-picker v-model="AddArrivalModel.ArrivalTime" type="datetime" placeholder="请选择到货时间" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DDTHH:mm:ss" style="width: 100%"/>
                    </el-form-item>
                </el-col>
                <!-- 备注：占据整行 -->
                <el-col :span="24">
                    <el-form-item label="备注" prop="Remark">
                        <el-input v-model="AddArrivalModel.Remark" type="textarea" :rows="2" maxlength="500" show-word-limit placeholder="请输入到货单备注"/>
                    </el-form-item>
                </el-col>
            </el-row>
        </el-form>
        <!-- 到货明细 -->
        <div style="display: flex; justify-content: space-between; align-items: center; margin: 20px 0 12px;">
            <h3 style="margin: 0;">到货明细</h3>
            <!-- 下一步接商品选择弹窗 -->
            <el-button type="primary" @click="OpenSelectGoods">
                + 添加商品
            </el-button>
        </div>

        <el-table :data="AddArrivalModel.ArrivalDetails" border empty-text="请先添加到货商品" style="width: 100%">
            <el-table-column type="index" label="序号" width="60" align="center" />

            <!-- 商品名称与编号 -->
            <el-table-column label="商品信息" min-width="220">
                <template #default="scope">
                    <div>{{ scope.row.GoodsTitle }}</div>
                    <el-text type="info" size="small">
                        {{ scope.row.GoodsCode || scope.row.BarCode || '无编码' }}
                    </el-text>
                </template>
            </el-table-column>

            <el-table-column label="单位" min-width="140">
                <template #default="scope">
                    <el-select v-model="scope.row.GoodsUnitID" placeholder="请选择单位" style="width: 100%" @change="HandleGoodsUnitChange(scope.row)">
                        <el-option v-for="unit in scope.row.GoodsUnitOptions" :key="unit.goodsUnitID" :label="unit.unitName" :value="unit.goodsUnitID" />
                    </el-select>
                </template>
            </el-table-column>
            <el-table-column prop="ConversionRate" label="换算率" width="90" align="center" />

            <!-- 到货数量：按当前采购单位 -->
            <el-table-column label="到货数量" min-width="140">
                <template #default="scope">
                    <el-input-number
                        v-model="scope.row.Quantity"
                        :min="0"
                        :precision="6"
                        :controls="false"
                        style="width: 100%"
                        @change="HandleQuantityChange(scope.row)"
                    />
                </template>
            </el-table-column>

            <!-- 当前采购单位单价 -->
            <el-table-column label="采购单价" min-width="150">
                <template #default="scope">
                    <el-input-number
                        v-model="scope.row.PurchaseUnitCost"
                        :min="0"
                        :precision="6"
                        :controls="false"
                        style="width: 100%"
                        @change="HandlePriceChange(scope.row)"
                    />
                </template>
            </el-table-column>

            <!-- 明细总金额 -->
            <el-table-column label="明细金额" min-width="150">
                <template #default="scope">
                    <el-input-number
                        v-model="scope.row.Amount"
                        :min="0"
                        :precision="2"
                        :controls="false"
                        style="width: 100%"
                        @change="HandleAmountChange(scope.row)"
                    />
                </template>
            </el-table-column>

            <el-table-column label="备注" min-width="150">
                <template #default="scope">
                    <el-input
                        v-model="scope.row.Remark"
                        maxlength="500"
                        placeholder="明细备注"
                    />
                </template>
            </el-table-column>

            <el-table-column label="操作" width="90" fixed="right" align="center">
                <template #default="scope">
                    <el-button type="primary" link @click="OpenGoodsUnitManage(scope.row)">
                        单位维护
                    </el-button>
                    <el-button type="danger" link @click="RemoveArrivalDetail(scope.$index)">
                        删除
                    </el-button>
                </template>
            </el-table-column>
        </el-table>

        <div style="text-align: right; margin-top: 15px;">
            明细合计：
            <strong>¥{{ ArrivalTotalAmount.toFixed(2) }}</strong>
        </div>
        <template #footer>
            <el-button @click="CloseDialog">取消</el-button>
            <el-button type="primary" :loading="Submitting" :disabled="Submitting" @click="SubmitArrival">
                确定新增
            </el-button>
        </template>
    </el-dialog>
    <AddSupplier ref="AddSupplierRef" @success="HandleSupplierAdded"/>
    <AddLocation ref="AddLocationRef" @success="HandleLocationAdded"/>
    <GoodsUnit ref="GoodsUnitRef" @success="HandleGoodsUnitSaved"/>
    <SelectArrivalGoods ref="SelectArrivalGoodsRef" @success="HandleGoodsSelected"/>
</template>

<script setup>
import { reactive, ref, computed, nextTick } from 'vue'
import { GetSupplierListApi } from '@/api/supplier'
import { GetLocationListApi } from '@/api/location'
import SelectArrivalGoods from './SelectArrivalGoods.vue'
import { ElMessage } from 'element-plus'
import AddSupplier from '@/views/supplier/components/AddSupplier.vue'
import AddLocation from '@/views/location/components/AddLocation.vue'
import GoodsUnit from '@/views/goods/components/GoodsUnit.vue'
import { GetGoodsUnitListApi } from '@/api/goodsUnit'
import { AddArrivalApi } from '@/api/arrival'

const emit = defineEmits(['success'])

// 是否正在提交，防止重复点击
const Submitting = ref(false)
// 新增到货单提交

async function SubmitArrival() {
    // 1. 防止重复提交
    if (Submitting.value) {
        return
    }
    // 2. 校验到货单基本信息
    try {
        await AddArrivalFormRef.value.validate()
    } catch {
        return
    }
    // 3. 校验到货明细不能为空
    if (AddArrivalModel.ArrivalDetails.length === 0) {
        ElMessage.warning('请至少添加一条到货明细')
        return
    }
    // 4. 逐条校验商品单位、数量和金额
    for (const row of AddArrivalModel.ArrivalDetails) {
        if (!row.GoodsUnitID) {
            ElMessage.warning(`商品【${row.GoodsTitle}】请选择单位`)
            return
        }
        if (!Number.isFinite(row.Quantity) || row.Quantity <= 0) {
            ElMessage.warning(`商品【${row.GoodsTitle}】到货数量必须大于0`)
            return
        }
        if (!Number.isFinite(row.PurchaseUnitCost) || row.PurchaseUnitCost < 0) {
            ElMessage.warning(`商品【${row.GoodsTitle}】采购单价不能小于0`)
            return
        }
        if (!Number.isFinite(row.Amount) || row.Amount < 0) {
            ElMessage.warning(`商品【${row.GoodsTitle}】明细金额不能小于0`)
            return
        }
    }
    // 5. 组装后端 AddArrivalRequest
    const requestData = {
        SupplierID: AddArrivalModel.SupplierID,
        LocationID: AddArrivalModel.LocationID,
        ArrivalTime: AddArrivalModel.ArrivalTime,
        Remark: AddArrivalModel.Remark,
        ArrivalDetails: AddArrivalModel.ArrivalDetails.map(row => ({
            GoodsUnitID: row.GoodsUnitID,
            Quantity: row.Quantity,
            PurchaseUnitCost: row.PurchaseUnitCost,
            Amount: row.Amount,
            Remark: row.Remark
        }))
    }
    // 6. 提交后端
    Submitting.value = true
    try {
        const res = await AddArrivalApi(requestData)
        if (res.code === 200) {
            ElMessage.success(res.message)
            // 通知到货单列表刷新
            emit('success')
            // 关闭新增弹窗
            CloseDialog()
        } else {
            ElMessage.error(res.message || '新增到货单失败')
        }
    } catch (error) {
        console.error('新增到货单失败：', error)
        ElMessage.error('新增到货单失败，请稍后重试')
    } finally {
        Submitting.value = false
    }
}

// 新增到货单表单
const AddArrivalModel = reactive({
    SupplierID: null,
    LocationID: null,
    ArrivalTime: null,
    Remark: '',
    ArrivalDetails: []
})

// 新增到货单表单引用
const AddArrivalFormRef = ref(null)
// 新增到货单主表校验规则
const AddArrivalRules = {
    SupplierID: [{ required: true, message: '请选择供应商', trigger: 'change'}],
    LocationID: [{ required: true, message: '请选择入库站点', trigger: 'change'}],
    ArrivalTime: [{ required: true, message: '请选择到货时间', trigger: 'change'},
        {
            validator: (rule, value, callback) => {
                if (!value) {
                    callback()
                    return
                }
                const arrivalTime = new Date(value.replace('T', ' '))
                if (Number.isNaN(arrivalTime.getTime())) {
                    callback(new Error('到货时间格式不正确'))
                } else if (arrivalTime.getTime() > Date.now()) {
                    callback(new Error('到货时间不能晚于当前时间'))
                } else {
                    callback()
                }
            },
            trigger: 'change'
        }
    ]
}
// 重置新增到货单表单
async function ResetAddArrivalForm() {
    // 重置表单
    AddArrivalFormRef.value?.resetFields()
    // 清空商品明细
    AddArrivalModel.ArrivalDetails = []
    // 等待 Vue 完成更新
    await nextTick()
    // 清除校验提示
    AddArrivalFormRef.value?.clearValidate()
}

// 供应商数据
const SupplierList = ref([])
const SupplierLoading = ref(false)
const AddSupplierRef = ref(null)

//打开新增供应商弹窗
async function OpenAddSupplier() {
     AddSupplierRef.value?.OpenDialog()
}

// 新增供应商成功后刷新列表
async function HandleSupplierAdded() {
    await GetSupplierList()
}

// 查询已有供应商
async function GetSupplierList() {
    SupplierLoading.value = true
    try {
        const res = await GetSupplierListApi({
            Page: 1,
            PageSize: 200
        })
        if (res.code === 200) {
            SupplierList.value = res.data.list.filter(
                item => item.isActive
            )
        } else {
            ElMessage.error(res.message)
        }
    } catch (error) {
        console.log(error)
        ElMessage.error('获取供应商失败')
    } finally {
        SupplierLoading.value = false
    }
}

// 入库站点数据
const LocationList = ref([])
const LocationLoading = ref(false)
const AddLocationRef = ref(null)

//打开新增站点
async function OpenAddLocation() {
    AddLocationRef.value?.OpenDialog()
}

async function HandleLocationAdded() {
    await GetLocationList()
}

//查询可入库站点
async function GetLocationList() {
    LocationLoading.value = true
    try {
        const res = await GetLocationListApi({
            Page: 1,
            PageSize: 200
        })
        if (res.code === 200) {
            LocationList.value = res.data.list.filter(
                item => item.isActive && item.canStock
            )
        } else {
            ElMessage.error(res.message)
        }
    } catch (error) {
        console.error("查询入库站点失败：", error)
        ElMessage.error("获取入库站点失败")
    } finally {
        LocationLoading.value = false
    }
}


// 是否显示新增到货单弹窗
const AddArrivalDialogVisible = ref(false)

// 打开弹窗
async function OpenDialog() {
    AddArrivalDialogVisible.value = true
    await GetSupplierList()
    await GetLocationList()
}

// 关闭弹窗
async function CloseDialog() {
    AddArrivalDialogVisible.value = false
}

// 允许父组件调用打开方法
defineExpose({
    OpenDialog
})

//商品单位关系
const GoodsUnitRef  = ref(null)
// 打开商品单位维护
function OpenGoodsUnitManage(row) {
    GoodsUnitRef.value?.OpenDialog({
        goodsID: row.GoodsID,
        goodsTitle: row.GoodsTitle
    })
}

const SelectArrivalGoodsRef = ref(null)
// 打开商品选择弹窗
function OpenSelectGoods() {
    SelectArrivalGoodsRef.value?.OpenDialog()
}

// 商品选择成功
async function HandleGoodsSelected(goodsList) {
    for (const goods of goodsList) {
        // 同一商品暂时只添加一次
        const exists = AddArrivalModel.ArrivalDetails.some(item => item.GoodsID === goods.goodsID)
        if (exists) { continue}
        // 先组装到货明细对象
        const row = {
            GoodsID: goods.goodsID,
            GoodsTitle: goods.goodsTitle,
            GoodsCode: goods.goodsCode,
            BarCode: goods.barCode,
            GoodsUnitID: null,
            UnitName: '',
            ConversionRate: null,
            GoodsUnitOptions: [],
            Quantity: 1,
            PurchaseUnitCost: 0,
            Amount: 0,
            Remark: ''
        }
        // 加入到货明细
        AddArrivalModel.ArrivalDetails.push(row)
        // 根据商品ID查询真实单位关系
        await LoadGoodsUnits(row)
    }
}

// 根据商品ID查询启用的商品单位
async function LoadGoodsUnits(row) {
    try {
        const res = await GetGoodsUnitListApi({
            GoodsID: row.GoodsID,
            UnitName: '',
            IsBaseUnit: null,
            IsActive: true,
            Page: 1,
            PageSize: 100
        })
        if (res.code === 200) {
            row.GoodsUnitOptions = res.data.list.filter(
                item => item.isActive
            )
        } else {
            ElMessage.error(res.message)
        }
    } catch (e) {
        console.error('查询商品单位失败：', e)
        ElMessage.error('查询商品单位失败')
    }
}

function HandleGoodsUnitChange(row) {
    const selectedUnit = row.GoodsUnitOptions.find(item => item.goodsUnitID === row.GoodsUnitID)
    if (!selectedUnit) {
        row.UnitName = ''
        row.ConversionRate = null
        return
    }
    row.UnitName = selectedUnit.unitName
    row.ConversionRate = selectedUnit.conversionRate
}

 // 商品单位新增、修改、启停成功后刷新到货明细单位列表
async function HandleGoodsUnitSaved(goodsID) {

    // 1. 根据商品ID找到到货明细
    const row = AddArrivalModel.ArrivalDetails.find(item => item.GoodsID === goodsID)
    if (!row) { return }
    // 2. 重新查询这个商品最新的单位关系
    await LoadGoodsUnits(row)
    // 3. 如果原来选择过单位，检查该单位是否仍然有效
    if (row.GoodsUnitID !== null) {
        const selectedUnit = row.GoodsUnitOptions.find(item => item.goodsUnitID === row.GoodsUnitID)
        if (!selectedUnit) {
            // 原单位已停用或不存在
            row.GoodsUnitID = null
            row.UnitName = ''
            row.ConversionRate = null
        } else {
            // 原单位仍然有效，同步最新单位名称和换算率
            row.UnitName = selectedUnit.unitName
            row.ConversionRate = selectedUnit.conversionRate
        }
    }
}

// 前端暂时用于显示金额的两位小数处理
function RoundAmount(value) {
    return Number(Number(value).toFixed(2))
}

// 修改数量：保留单价，重新计算金额
function HandleQuantityChange(row) {
    row.Amount = RoundAmount(
        (row.Quantity ?? 0) * (row.PurchaseUnitCost ?? 0)
    )
}

// 修改采购单价：重新计算金额
function HandlePriceChange(row) {
    row.Amount = RoundAmount(
        (row.Quantity ?? 0) * (row.PurchaseUnitCost ?? 0)
    )
}

// 修改明细金额：反算当前采购单位单价
function HandleAmountChange(row) {
    if (row.Quantity > 0) {
        row.PurchaseUnitCost = Number(
            ((row.Amount ?? 0) / row.Quantity).toFixed(6)
        )
    }
}

// 删除一条到货明细
function RemoveArrivalDetail(index) {
    AddArrivalModel.ArrivalDetails.splice(index, 1)
}

// 计算整张到货单的明细合计金额
const ArrivalTotalAmount = computed(() => {
    return AddArrivalModel.ArrivalDetails.reduce(
        (total, item) => total + (Number(item.Amount) || 0),
        0
    )
})
</script>