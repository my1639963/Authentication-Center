<template>
  <div>
    <el-card>
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>权限管理</span>
          <el-select v-model="selectedGroup" placeholder="全部分组" clearable style="width: 200px" @change="loadData">
            <el-option v-for="g in groups" :key="g" :label="g || '未分组'" :value="g" />
          </el-select>
        </div>
      </template>

      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="permissionCode" label="权限编码" width="200" />
        <el-table-column prop="permissionName" label="权限名称" width="200" />
        <el-table-column prop="permissionGroup" label="分组" width="150">
          <template #default="{ row }">{{ row.permissionGroup || '未分组' }}</template>
        </el-table-column>
        <el-table-column prop="description" label="描述" />
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'success' : 'danger'">{{ row.status === 0 ? '正常' : '禁用' }}</el-tag>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getAllPermissions } from '@/api/admin'
import type { AdminPermissionDto } from '@/api/admin'

const loading = ref(false)
const allData = ref<AdminPermissionDto[]>([])
const selectedGroup = ref('')

const groups = computed(() => {
  const set = new Set<string>()
  for (const p of allData.value) {
    set.add(p.permissionGroup ?? '')
  }
  return Array.from(set)
})

const tableData = computed(() => {
  if (!selectedGroup.value) return allData.value
  return allData.value.filter(p => (p.permissionGroup ?? '') === selectedGroup.value)
})

onMounted(() => loadData())

async function loadData() {
  loading.value = true
  try {
    const { data } = await getAllPermissions()
    allData.value = data
  } finally {
    loading.value = false
  }
}
</script>
