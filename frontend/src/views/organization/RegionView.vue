<template>
  <div>
    <el-card>
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>区域管理</span>
          <el-button type="primary" @click="openCreateDialog(null)">新增顶级区域</el-button>
        </div>
      </template>

      <el-table :data="treeData" v-loading="loading" row-key="id" border default-expand-all>
        <el-table-column prop="regionCode" label="区域编码" width="160" />
        <el-table-column prop="regionName" label="区域名称" width="200" />
        <el-table-column prop="regionLevel" label="层级" width="80" />
        <el-table-column prop="sort" label="排序" width="80" />
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'success' : 'danger'">{{ row.status === 0 ? '正常' : '禁用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="240">
          <template #default="{ row }">
            <el-button size="small" @click="openCreateDialog(row)">新增子区域</el-button>
            <el-button size="small" @click="openEditDialog(row)">编辑</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑区域' : '新增区域'" width="500px">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="80px">
        <el-form-item label="区域编码" prop="regionCode">
          <el-input v-model="form.regionCode" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="区域名称" prop="regionName">
          <el-input v-model="form.regionName" />
        </el-form-item>
        <el-form-item label="层级" prop="regionLevel">
          <el-input-number v-model="form.regionLevel" :min="1" :max="10" />
        </el-form-item>
        <el-form-item label="排序">
          <el-input-number v-model="form.sort" :min="0" />
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
import { getAllRegions, createRegion, updateRegion } from '@/api/organization'
import type { RegionDto } from '@/api/organization'

const loading = ref(false)
const treeData = ref<RegionDto[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const editingId = ref(0)
const parentId = ref<number | null>(null)
const submitLoading = ref(false)
const formRef = ref<FormInstance>()
const form = reactive({ regionCode: '', regionName: '', regionLevel: 1, sort: 0 })
const formRules: FormRules = {
  regionCode: [{ required: true, message: '请输入区域编码', trigger: 'blur' }],
  regionName: [{ required: true, message: '请输入区域名称', trigger: 'blur' }],
  regionLevel: [{ required: true, message: '请输入层级', trigger: 'blur' }]
}

onMounted(() => loadData())

function buildTree(list: RegionDto[]): RegionDto[] {
  const map = new Map<number, RegionDto & { children?: RegionDto[] }>()
  const roots: RegionDto[] = []
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
  loading.value = true
  try {
    const { data } = await getAllRegions()
    treeData.value = buildTree(data)
  } finally {
    loading.value = false
  }
}

function openCreateDialog(parent: RegionDto | null) {
  isEdit.value = false
  parentId.value = parent?.id ?? null
  Object.assign(form, { regionCode: '', regionName: '', regionLevel: (parent?.regionLevel ?? 0) + 1, sort: 0 })
  dialogVisible.value = true
}

function openEditDialog(row: RegionDto) {
  isEdit.value = true
  editingId.value = row.id
  Object.assign(form, { regionCode: row.regionCode, regionName: row.regionName, regionLevel: row.regionLevel, sort: row.sort })
  dialogVisible.value = true
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitLoading.value = true
  try {
    if (isEdit.value) {
      await updateRegion(editingId.value, { regionName: form.regionName, sort: form.sort })
      ElMessage.success('更新成功')
    } else {
      await createRegion({ parentId: parentId.value ?? undefined, regionCode: form.regionCode, regionName: form.regionName, regionLevel: form.regionLevel, sort: form.sort })
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadData()
  } finally {
    submitLoading.value = false
  }
}
</script>
