
<template>
  <el-dialog v-model="EditArrivalDialogVisible" title="编辑到货单" width="95%" top="3vh" :close-on-click-modal="false">
    <div v-loading="loading">
      <h3>到货单基本信息</h3>
      <el-form :model="editArrivalModel" label-width="100px">
        <el-row :gutter="24">
          <el-col :span="8">
            <el-form-item label="供应商">
              <el-input v-model="editArrivalModel.supplierName" disabled />
            </el-form-item>
          </el-col>

          <el-col :span="8">
            <el-form-item label="入库站点">
              <el-input v-model="editArrivalModel.locationName" disabled />
            </el-form-item>
          </el-col>

          <el-col :span="8">
            <el-form-item label="到货时间">
              <el-date-picker v-model="editArrivalModel.arrivalTime" type="datetime" value-format="YYYY-MM-DDTHH:mm:ss" style="width: 100%"/>
            </el-form-item>
          </el-col>

          <el-col :span="24">
            <el-form-item label="备注">
              <el-input v-model="editArrivalModel.remark" type="textarea" :rows="2" maxlength="500"/>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <div style="display: flex; justify-content: space-between; align-items: center; margin: 20px 0 12px;">
        <h3 style="margin: 0;">到货明细</h3>

        <el-button type="primary" @click="OpenSelectGoods">
            + 添加商品
        </el-button>
        </div>

      <el-table :data="PagedArrivalDetails" border style="width: 100%">
        <el-table-column prop="goodsTitle" label="商品名称" min-width="180" />
        <el-table-column prop="goodsCode" label="商品编码" width="140" />
        <el-table-column label="单位" min-width="140">
            <template #default="{ row }">
                <el-select v-model="row.goodsUnitID" placeholder="请选择单位" style="width: 100%" @change="HandleGoodsUnitChange(row)">
                    <el-option v-for="unit in row.goodsUnitOptions" :key="unit.goodsUnitID" :label="unit.unitName" :value="unit.goodsUnitID"/>
                </el-select>
            </template>
        </el-table-column>
        <el-table-column prop="conversionRate" label="换算率" width="100" />
        <el-table-column label="数量" min-width="140">
            <template #default="{ row }">
                <el-input-number
                v-model="row.quantity"
                :min="0"
                :precision="6"
                :controls="false"
                style="width: 100%"
                @change="HandleQuantityChange(row)"
                />
            </template>
        </el-table-column>
        <el-table-column label="采购单价" min-width="150">
            <template #default="{ row }">
                <el-input-number
                v-model="row.purchaseUnitCost"
                :min="0"
                :precision="6"
                :controls="false"
                style="width: 100%"
                @change="HandlePriceChange(row)"
                />
            </template>
            </el-table-column>
        <el-table-column label="金额" min-width="150">
            <template #default="{ row }">
                <el-input-number
                v-model="row.amount"
                :min="0"
                :precision="2"
                :controls="false"
                style="width: 100%"
                @change="HandleAmountChange(row)"
                />
            </template>
            </el-table-column>
        <el-table-column label="备注" min-width="160">
            <template #default="{ row }">
                <el-input v-model="row.remark" maxlength="500" placeholder="请输入明细备注"/>
            </template>
        </el-table-column>
        <el-table-column label="操作" width="90" fixed="right">
            <template #default="scope">
                <el-button type="danger" link @click="RemoveArrivalDetail(scope.$index)">
                    删除
                </el-button>
            </template>
        </el-table-column>
      </el-table>
      <el-pagination
        v-model:current-page="DetailPage"
        v-model:page-size="DetailPageSize"
        :page-sizes="[10, 20, 50, 100]"
        :total="editArrivalModel.arrivalDetails.length"
        layout="total, sizes, prev, pager, next, jumper"
        style="margin-top: 16px; justify-content: flex-end;"
    />

      <div style="text-align: right; margin-top: 15px">
        明细合计：
        <strong>¥{{ totalAmount.toFixed(2) }}</strong>
      </div>
    </div>

    <template #footer>
        <el-button :disabled="SaveLoading" @click="EditArrivalDialogVisible = false">
            取消
        </el-button>
        <el-button type="primary" :loading="SaveLoading" :disabled="loading" @click="SaveEditArrival">
            保存修改
        </el-button>
    </template>
  </el-dialog>
  <SelectArrivalGoods ref="SelectArrivalGoodsRef" @success="HandleGoodsSelected"/>
</template>

<script setup>
import { reactive, ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { GetArrivalDetailApi, UpdateArrivalApi } from '@/api/arrival'
import { ArrivalStatus } from '@/enums/arrivalStatus'
import SelectArrivalGoods from './SelectArrivalGoods.vue'
import { GetGoodsUnitListApi } from '@/api/goodsUnit'

const EditArrivalDialogVisible = ref(false)
const loading = ref(false)
// 通知父组件刷新到货单列表
const emit = defineEmits(['success'])

// 到货明细分页
const DetailPage = ref(1)
const DetailPageSize = ref(10)

// 当前页展示的商品
const PagedArrivalDetails = computed(() => {
    const start = (DetailPage.value - 1) * DetailPageSize.value
    const end = start + DetailPageSize.value

    return editArrivalModel.arrivalDetails.slice(start, end)
})

const editArrivalModel = reactive({
  arrivalID: null,
  supplierID: null,
  supplierName: '',
  locationID: null,
  locationName: '',
  arrivalTime: null,
  remark: '',
  arrivalDetails: []
})

const totalAmount = computed(() =>
  editArrivalModel.arrivalDetails.reduce(
    (total, row) => total + (Number(row.amount) || 0),
    0
  )
)

// 打开编辑弹窗
async function OpenDialog(arrivalID) {
  if (loading.value) return

  loading.value = true
  EditArrivalDialogVisible.value = true

  DetailPage.value = 1
  DetailPageSize.value = 10

  // 清空上一次编辑的数据
  Object.assign(editArrivalModel, {
    arrivalID: null,
    supplierID: null,
    supplierName: '',
    locationID: null,
    locationName: '',
    arrivalTime: null,
    remark: '',
    arrivalDetails: []
  })

  try {
    const pageSize = 200
    let page = 1
    let allDetails = []
    let arrival = null

    // 分页加载全部到货明细
    while (true) {
      const res = await GetArrivalDetailApi({
        ArrivalID: arrivalID,
        Page: page,
        PageSize: pageSize
      })

      if (res.code !== 200 || !res.data) {
        throw new Error(res.message || '查询到货单详情失败')
      }

      arrival = res.data

      // 后端再次确认状态
      if (arrival.arrivalStatus !== ArrivalStatus.Pending) {
        ElMessage.warning('只有未审核的到货单才能编辑')
        EditArrivalDialogVisible.value = false
        return
      }

      const details = arrival.arrivalDetails
      const currentList = details?.list

      if (!Array.isArray(currentList)) {
        throw new Error('到货明细返回格式不正确')
      }

      allDetails.push(...currentList)

      const totalCount = details.totalCount

      if (!Number.isInteger(totalCount) || totalCount < 0) {
        throw new Error('到货明细总数无效')
      }

      if (allDetails.length >= totalCount) {
        break
      }

      if (currentList.length === 0) {
        throw new Error('到货明细分页数据不完整')
      }

      page++
    }
    editArrivalModel.arrivalID = arrival.arrivalID
    editArrivalModel.supplierID = arrival.supplierID
    editArrivalModel.supplierName = arrival.supplierName
    editArrivalModel.locationID = arrival.locationID
    editArrivalModel.locationName = arrival.locationName
    editArrivalModel.arrivalTime = arrival.arrivalTime
    editArrivalModel.remark = arrival.remark ?? ''
    editArrivalModel.arrivalDetails = allDetails.map(item => ({
      arrivalDetailID: item.arrivalDetailID,
      goodsID: item.goodsID,
      goodsUnitID: item.goodsUnitID,
      goodsTitle: item.goodsTitle,
      goodsCode: item.goodsCode,
      barCode: item.barCode,
      unitName: item.unitName,
      conversionRate: item.conversionRate,
      goodsUnitOptions: [],
      quantity: item.quantity,
      purchaseUnitCost: item.purchaseUnitCost,
      amount: item.amount,
      remark: item.remark ?? ''
    }))

    // 加载原有商品的单位下拉选项
    await Promise.all(
        editArrivalModel.arrivalDetails.map(row => LoadGoodsUnits(row))
    )

  } catch (error) {
    console.error('加载到货单失败：', error)
    ElMessage.error(error.message || '加载到货单失败')
    EditArrivalDialogVisible.value = false
  } finally {
    loading.value = false
  }
}

defineExpose({
  OpenDialog
})

// 金额保留两位小数
function RoundAmount(value) {
    return Number(Number(value).toFixed(2))
}

// 修改数量：保持采购单价不变，重新计算金额
function HandleQuantityChange(row) {
    row.amount = RoundAmount(
        (row.quantity ?? 0) * (row.purchaseUnitCost ?? 0)
    )
}

// 修改采购单价：重新计算金额
function HandlePriceChange(row) {
    row.amount = RoundAmount(
        (row.quantity ?? 0) * (row.purchaseUnitCost ?? 0)
    )
}

// 修改金额：保持金额不变，反算采购单价
function HandleAmountChange(row) {
    if (row.quantity > 0) {
        row.purchaseUnitCost = Number(
            ((row.amount ?? 0) / row.quantity).toFixed(6)
        )
    }
}

const SelectArrivalGoodsRef = ref(null)
// 打开商品选择弹窗
function OpenSelectGoods() {
    SelectArrivalGoodsRef.value?.OpenDialog()
}
// 商品选择完成
async function HandleGoodsSelected(goodsList) {
    for (const goods of goodsList) {
        // 防止重复添加同一商品
        const exists = editArrivalModel.arrivalDetails.some(
            item => item.goodsID === goods.goodsID
        )
        if (exists) continue
        const row = {
            goodsID: goods.goodsID,
            goodsTitle: goods.goodsTitle,
            goodsCode: goods.goodsCode,
            barCode: goods.barCode,
            goodsUnitID: null,
            unitName: '',
            conversionRate: null,
            goodsUnitOptions: [],
            quantity: 1,
            purchaseUnitCost: 0,
            amount: 0,
            remark: ''
        }
        editArrivalModel.arrivalDetails.push(row)
        // 查询当前商品可用单位
        await LoadGoodsUnits(row)
    }
}

// 查询商品单位
async function LoadGoodsUnits(row) {
    try {
        const res = await GetGoodsUnitListApi({
            GoodsID: row.goodsID,
            UnitName: '',
            IsBaseUnit: null,
            IsActive: true,
            Page: 1,
            PageSize: 100
        })

        if (res.code === 200) {
            row.goodsUnitOptions = res.data.list.filter(
                item => item.isActive
            )
        } else {
            ElMessage.error(res.message || '查询商品单位失败')
        }
    } catch (error) {
        console.error(error)
        ElMessage.error('查询商品单位失败')
    }
}

// 删除当前编辑表格中的商品
function RemoveArrivalDetail(index) {
    const actualIndex = (DetailPage.value - 1) * DetailPageSize.value + index
    editArrivalModel.arrivalDetails.splice(actualIndex, 1)
    // 删除最后一条后，如果当前页已经不存在，自动回到上一页
    const maxPage = Math.max(1, Math.ceil(editArrivalModel.arrivalDetails.length / DetailPageSize.value))
    if (DetailPage.value > maxPage) { DetailPage.value = maxPage }
}

// 切换商品单位
function HandleGoodsUnitChange(row) {
    const selectedUnit = row.goodsUnitOptions.find(
        item => item.goodsUnitID === row.goodsUnitID
    )

    if (!selectedUnit) {
        row.unitName = ''
        row.conversionRate = null
        return
    }
    row.unitName = selectedUnit.unitName
    row.conversionRate = selectedUnit.conversionRate
}

const SaveLoading = ref(false)
// 保存修改到货单
const SaveEditArrival = async () => {
    if (SaveLoading.value) return
    const details = editArrivalModel.arrivalDetails
    if (!details || details.length === 0) {
        ElMessage.warning('请至少添加一个商品')
        return
    }
    for (const row of details) {
        if (!row.goodsUnitID) {
            ElMessage.warning(`商品【${row.goodsTitle}】请选择采购单位`)
            return
        }
        if (row.quantity == null || Number(row.quantity) <= 0) {
            ElMessage.warning(`商品【${row.goodsTitle}】数量必须大于0`)
            return
        }
        if (row.purchaseUnitCost == null || Number(row.purchaseUnitCost) < 0) {
            ElMessage.warning(`商品【${row.goodsTitle}】采购单价不能小于0`)
            return
        }
        if (row.amount == null || Number(row.amount) < 0) {
            ElMessage.warning(`商品【${row.goodsTitle}】金额不能小于0`)
            return
        }
    }
    const data = {
        ArrivalID: editArrivalModel.arrivalID,
        SupplierID: editArrivalModel.supplierID,
        LocationID: editArrivalModel.locationID,
        ArrivalTime: editArrivalModel.arrivalTime,
        Remark: editArrivalModel.remark,
        ArrivalDetails: details.map(row => ({
            GoodsUnitID: row.goodsUnitID,
            Quantity: row.quantity,
            PurchaseUnitCost: row.purchaseUnitCost,
            Amount: row.amount,
            Remark: row.remark
        }))
    }
    try {
        SaveLoading.value = true
        const res = await UpdateArrivalApi(data)
        if (res.code === 200) {
            ElMessage.success(res.message || '修改到货单成功')
            EditArrivalDialogVisible.value = false
            emit('success')
        } else {
            ElMessage.error(res.message || '修改到货单失败')
        }
    } catch (error) {
        console.error('修改到货单失败：', error)
        ElMessage.error('修改到货单失败，请稍后重试')
    } finally {
        SaveLoading.value = false
    }
}
</script>
