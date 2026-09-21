<template>
  <div>
    <el-card>
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>角色管理</span>
          <el-button type="primary" @click="openCreateDialog">新增角色</el-button>
        </div>
      </template>

      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="roleCode" label="角色编码" width="160" />
        <el-table-column prop="roleName" label="角色名称" width="160" />
        <el-table-column prop="description" label="描述" />
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'success' : 'danger'">{{ row.status === 0 ? '正常' : '禁用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="openEditDialog(row)">编辑</el-button>
            <el-button size="small" type="primary" @click="openPermDialog(row)">分配权限</el-button>
            <el-button size="small" @click="openUserRoleDialog(row)">用户角色</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 创建/编辑角色 -->
    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑角色' : '新增角色'" width="500px">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="80px">
        <el-form-item label="角色编码" prop="roleCode">
          <el-input v-model="form.roleCode" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="角色名称" prop="roleName">
          <el-input v-model="form.roleName" />
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

    <!-- 分配权限 -->
    <el-dialog v-model="permDialogVisible" title="分配权限" width="600px">
      <el-checkbox-group v-model="selectedPermIds">
        <div v-for="group in permissionGroups" :key="group.name" style="margin-bottom: 16px">
          <h4>{{ group.name || '未分组' }}</h4>
          <el-checkbox v-for="p in group.permissions" :key="p.id" :value="p.id" :label="p.permissionName" />
        </div>
      </el-checkbox-group>
      <template #footer>
        <el-button @click="permDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleAssignPerms">确定</el-button>
      </template>
    </el-dialog>

    <!-- 用户角色分配 -->
    <el-dialog v-model="userRoleDialogVisible" title="用户角色分配" width="500px">
      <el-form :inline="true">
        <el-form-item label="用户ID">
          <el-input-number v-model="targetUserId" :min="1" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadUserRoles">查询</el-button>
        </el-form-item>
      </el-form>
      <el-checkbox-group v-model="selectedUserRoleIds">
        <el-checkbox v-for="r in tableData" :key="r.id" :value="r.id" :label="r.roleName" />
      </el-checkbox-group>
      <template #footer>
        <el-button @click="userRoleDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleAssignUserRoles">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { getAllRoles, createRole, updateRole, getAllPermissions, getRolePermissions, assignPermissions, assignRoles } from '@/api/admin'
import type { AdminRoleDto, AdminPermissionDto } from '@/api/admin'

const loading = ref(false)
const tableData = ref<AdminRoleDto[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const editingId = ref(0)
const submitLoading = ref(false)
const formRef = ref<FormInstance>()
const form = reactive({ roleCode: '', roleName: '', description: '' })
const formRules: FormRules = {
  roleCode: [{ required: true, message: '请输入角色编码', trigger: 'blur' }],
  roleName: [{ required: true, message: '请输入角色名称', trigger: 'blur' }]
}

// 权限分配
const permDialogVisible = ref(false)
const permRoleId = ref(0)
const allPermissions = ref<AdminPermissionDto[]>([])
const selectedPermIds = ref<number[]>([])

const permissionGroups = computed(() => {
  const map = new Map<string, AdminPermissionDto[]>()
  for (const p of allPermissions.value) {
    const key = p.permissionGroup ?? ''
    if (!map.has(key)) map.set(key, [])
    map.get(key)!.push(p)
  }
  return Array.from(map.entries()).map(([name, permissions]) => ({ name, permissions }))
})

// 用户角色分配
const userRoleDialogVisible = ref(false)
const targetUserId = ref(0)
const selectedUserRoleIds = ref<number[]>([])

onMounted(() => loadData())

async function loadData() {
  loading.value = true
  try {
    const { data } = await getAllRoles()
    tableData.value = data
  } finally {
    loading.value = false
  }
}

function openCreateDialog() {
  isEdit.value = false
  Object.assign(form, { roleCode: '', roleName: '', description: '' })
  dialogVisible.value = true
}

function openEditDialog(row: AdminRoleDto) {
  isEdit.value = true
  editingId.value = row.id
  Object.assign(form, { roleCode: row.roleCode, roleName: row.roleName, description: row.description ?? '' })
  dialogVisible.value = true
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitLoading.value = true
  try {
    if (isEdit.value) {
      await updateRole(editingId.value, { roleName: form.roleName, description: form.description || undefined })
      ElMessage.success('更新成功')
    } else {
      await createRole({ roleCode: form.roleCode, roleName: form.roleName, description: form.description || undefined })
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadData()
  } finally {
    submitLoading.value = false
  }
}

async function openPermDialog(row: AdminRoleDto) {
  permRoleId.value = row.id
  const [permsResp, rolePermsResp] = await Promise.all([getAllPermissions(), getRolePermissions(row.id)])
  allPermissions.value = permsResp.data
  selectedPermIds.value = rolePermsResp.data.map(p => p.id)
  permDialogVisible.value = true
}

async function handleAssignPerms() {
  submitLoading.value = true
  try {
    await assignPermissions(permRoleId.value, selectedPermIds.value)
    ElMessage.success('权限分配成功')
    permDialogVisible.value = false
  } finally {
    submitLoading.value = false
  }
}

function openUserRoleDialog(_row: AdminRoleDto) {
  targetUserId.value = 0
  selectedUserRoleIds.value = []
  userRoleDialogVisible.value = true
}

async function loadUserRoles() {
  if (!targetUserId.value) return
  const { getUserRoles } = await import('@/api/admin')
  const { data } = await getUserRoles(targetUserId.value)
  selectedUserRoleIds.value = data.map(r => r.id)
}

async function handleAssignUserRoles() {
  if (!targetUserId.value) return
  submitLoading.value = true
  try {
    await assignRoles(targetUserId.value, selectedUserRoleIds.value)
    ElMessage.success('角色分配成功')
    userRoleDialogVisible.value = false
  } finally {
    submitLoading.value = false
  }
}
</script>
