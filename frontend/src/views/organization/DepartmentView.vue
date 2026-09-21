<template>
  <div>
    <el-card>
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>部门管理</span>
          <el-button type="primary" @click="openCreateDialog(null)">新增顶级部门</el-button>
        </div>
      </template>

      <el-form :inline="true" style="margin-bottom: 16px">
        <el-form-item label="单位ID">
          <el-input-number v-model="unitId" :min="1" @change="loadData" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadData">查询</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="treeData" v-loading="loading" row-key="id" border default-expand-all>
        <el-table-column prop="deptCode" label="部门编码" width="160" />
        <el-table-column prop="deptName" label="部门名称" width="200" />
        <el-table-column prop="sort" label="排序" width="80" />
        <el-table-column prop="description" label="描述" />
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'success' : 'danger'">{{ row.status === 0 ? '正常' : '禁用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="240">
          <template #default="{ row }">
            <el-button size="small" @click="openCreateDialog(row)">新增子部门</el-button>
            <el-button size="small" @click="openEditDialog(row)">编辑</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑部门' : '新增部门'" width="550px">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="80px">
        <el-form-item label="部门编码" prop="deptCode">
          <el-input v-model="form.deptCode" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="部门名称" prop="deptName">
          <el-input v-model="form.deptName" />
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
import { getDepartmentChildren, createDepartment, updateDepartment } from '@/api/organization'
import type { DepartmentDto } from '@/api/organization'

const loading = ref(false)
const treeData = ref<DepartmentDto[]>([])
const unitId = ref(1)
const dialogVisible = ref(false)
const isEdit = ref(false)
const editingId = ref(0)
const parentId = ref<number | null>(null)
const submitLoading = ref(false)
const formRef = ref<FormInstance>()
const form = reactive({ deptCode: '', deptName: '', sort: 0, description: '' })
const formRules: FormRules = {
  deptCode: [{ required: true, message: '请输入部门编码', trigger: 'blur' }],
  deptName: [{ required: true, message: '请输入部门名称', trigger: 'blur' }]
}

onMounted(() => loadData())

function buildTree(list: DepartmentDto[]): DepartmentDto[] {
  const map = new Map<number, DepartmentDto & { children?: DepartmentDto[] }>()
  const roots: DepartmentDto[] = []
  for (const item of list) map.set(item.id, { ...item })
  for (const item of list) {
    const node = map.get(item.id)!
    if (item.parentId && map.has(item.parentId)) {
      const parent = map.get(item.parentId)!
      if (!parent.children) parent.children = []
      parent.children.push(node)
    } else {
      roots.push(node)
    }
  }
  return roots
}

async function loadData() {
  if (!unitId.value) return
  loading.value = true
  try {
    const { data } = await getDepartmentChildren(unitId.value)
    treeData.value = buildTree(data)
  } finally {
    loading.value = false
  }
}

function openCreateDialog(parent: DepartmentDto | null) {
  isEdit.value = false
  parentId.value = parent?.id ?? null
  Object.assign(form, { deptCode: '', deptName: '', sort: 0, description: '' })
  dialogVisible.value = true
}

function openEditDialog(row: DepartmentDto) {
  isEdit.value = true
  editingId.value = row.id
  Object.assign(form, { deptCode: row.deptCode, deptName: row.deptName, sort: row.sort, description: row.description ?? '' })
  dialogVisible.value = true
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitLoading.value = true
  try {
    if (isEdit.value) {
      await updateDepartment(editingId.value, { deptName: form.deptName, sort: form.sort, description: form.description || undefined })
      ElMessage.success('更新成功')
    } else {
      await createDepartment({ unitId: unitId.value, parentId: parentId.value ?? undefined, deptCode: form.deptCode, deptName: form.deptName, sort: form.sort, description: form.description || undefined })
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadData()
  } finally {
    submitLoading.value = false
  }
}
</script>
