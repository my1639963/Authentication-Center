<template>
  <div>
    <el-card>
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>单位管理</span>
          <el-button type="primary" @click="openCreateDialog">新增单位</el-button>
        </div>
      </template>

      <el-form :inline="true" :model="query" style="margin-bottom: 16px">
        <el-form-item label="关键词">
          <el-input v-model="query.keyword" placeholder="单位名称/编码" clearable @keyup.enter="loadData" />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="query.status" placeholder="全部" clearable>
            <el-option label="正常" :value="0" />
            <el-option label="禁用" :value="1" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadData">查询</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="unitCode" label="单位编码" width="140" />
        <el-table-column prop="unitName" label="单位名称" width="200" />
        <el-table-column prop="unitType" label="类型" width="100" />
        <el-table-column prop="description" label="描述" />
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'success' : 'danger'">{{ row.status === 0 ? '正常' : '禁用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="240" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="openEditDialog(row)">编辑</el-button>
            <el-button v-if="row.status === 0" size="small" type="warning" @click="handleDisable(row)">禁用</el-button>
            <el-button v-else size="small" type="success" @click="handleEnable(row)">启用</el-button>
          </template>
        </el-table-column>
      </el-table>

      <el-pagination
        style="margin-top: 16px; justify-content: flex-end"
        v-model:current-page="query.pageIndex"
        v-model:page-size="query.pageSize"
        :total="total"
        :page-sizes="[10, 20, 50]"
        layout="total, sizes, prev, pager, next"
        @size-change="loadData"
        @current-change="loadData"
      />
    </el-card>

    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑单位' : '新增单位'" width="550px">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="80px">
        <el-form-item label="单位编码" prop="unitCode">
          <el-input v-model="form.unitCode" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="单位名称" prop="unitName">
          <el-input v-model="form.unitName" />
        </el-form-item>
        <el-form-item label="类型">
          <el-input v-model="form.unitType" />
        </el-form-item>
        <el-form-item label="排序">
          <el-input-number v-model="form.sort" :min="0" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="form.description" type="textarea" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { searchUnits, createUnit, updateUnit, enableUnit, disableUnit } from '@/api/organization'
import type { UnitDto } from '@/api/organization'

const loading = ref(false)
const tableData = ref<UnitDto[]>([])
const total = ref(0)
const query = reactive({ keyword: '', status: undefined as number | undefined, pageIndex: 1, pageSize: 20 })

const dialogVisible = ref(false)
const isEdit = ref(false)
const editingId = ref(0)
const submitLoading = ref(false)
const formRef = ref<FormInstance>()
const form = reactive({ unitCode: '', unitName: '', unitType: '', sort: 0, description: '' })
const formRules: FormRules = {
  unitCode: [{ required: true, message: '请输入单位编码', trigger: 'blur' }],
  unitName: [{ required: true, message: '请输入单位名称', trigger: 'blur' }]
}

onMounted(() => loadData())

async function loadData() {
  loading.value = true
  try {
    const { data } = await searchUnits(query)
    tableData.value = data.items
    total.value = data.total
  } finally {
    loading.value = false
  }
}

function openCreateDialog() {
  isEdit.value = false
  Object.assign(form, { unitCode: '', unitName: '', unitType: '', sort: 0, description: '' })
  dialogVisible.value = true
}

function openEditDialog(row: UnitDto) {
  isEdit.value = true
  editingId.value = row.id
  Object.assign(form, { unitCode: row.unitCode, unitName: row.unitName, unitType: row.unitType ?? '', sort: row.sort, description: row.description ?? '' })
  dialogVisible.value = true
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitLoading.value = true
  try {
    if (isEdit.value) {
      await updateUnit(editingId.value, { unitName: form.unitName, unitType: form.unitType || undefined, sort: form.sort, description: form.description || undefined })
      ElMessage.success('更新成功')
    } else {
      await createUnit({ unitCode: form.unitCode, unitName: form.unitName, unitType: form.unitType || undefined, sort: form.sort, description: form.description || undefined })
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadData()
  } finally {
    submitLoading.value = false
  }
}

async function handleEnable(row: UnitDto) {
  await enableUnit(row.id)
  ElMessage.success('已启用')
  loadData()
}

async function handleDisable(row: UnitDto) {
  await disableUnit(row.id)
  ElMessage.success('已禁用')
  loadData()
}
</script>
