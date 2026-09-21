<template>
  <div>
    <el-card>
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>OAuth 客户端管理</span>
          <el-button type="primary" @click="openCreateDialog">新增客户端</el-button>
        </div>
      </template>

      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="clientId" label="客户端 ID" width="180" />
        <el-table-column prop="displayName" label="显示名称" width="180" />
        <el-table-column prop="clientType" label="类型" width="120" />
        <el-table-column prop="redirectUris" label="回调地址">
          <template #default="{ row }">
            <el-tag v-for="uri in row.redirectUris" :key="uri" size="small" style="margin-right: 4px">{{ uri }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="openEditDialog(row)">编辑</el-button>
            <el-popconfirm title="确认删除？" @confirm="handleDelete(row.clientId)">
              <template #reference>
                <el-button size="small" type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑客户端' : '新增客户端'" width="550px">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="100px">
        <el-form-item label="客户端 ID" prop="clientId">
          <el-input v-model="form.clientId" :disabled="isEdit" />
        </el-form-item>
        <el-form-item v-if="!isEdit" label="密钥">
          <el-input v-model="form.clientSecret" type="password" show-password />
        </el-form-item>
        <el-form-item label="显示名称">
          <el-input v-model="form.displayName" />
        </el-form-item>
        <el-form-item label="类型">
          <el-input v-model="form.clientType" />
        </el-form-item>
        <el-form-item label="回调地址">
          <el-input v-model="form.redirectUrisStr" type="textarea" placeholder="每行一个 URI" />
        </el-form-item>
        <el-form-item label="权限">
          <el-input v-model="form.permissionsStr" type="textarea" placeholder="每行一个权限" />
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
import { getAllOAuthClients, createOAuthClient, updateOAuthClient, deleteOAuthClient } from '@/api/oauthClient'
import type { OAuthClientDto } from '@/api/oauthClient'

const loading = ref(false)
const tableData = ref<OAuthClientDto[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const editingClientId = ref('')
const submitLoading = ref(false)
const formRef = ref<FormInstance>()
const form = reactive({ clientId: '', clientSecret: '', displayName: '', clientType: '', redirectUrisStr: '', permissionsStr: '' })
const formRules: FormRules = {
  clientId: [{ required: true, message: '请输入客户端 ID', trigger: 'blur' }]
}

onMounted(() => loadData())

async function loadData() {
  loading.value = true
  try {
    const { data } = await getAllOAuthClients()
    tableData.value = data
  } finally {
    loading.value = false
  }
}

function openCreateDialog() {
  isEdit.value = false
  Object.assign(form, { clientId: '', clientSecret: '', displayName: '', clientType: '', redirectUrisStr: '', permissionsStr: '' })
  dialogVisible.value = true
}

function openEditDialog(row: OAuthClientDto) {
  isEdit.value = true
  editingClientId.value = row.clientId
  Object.assign(form, {
    clientId: row.clientId, clientSecret: '', displayName: row.displayName ?? '', clientType: row.clientType ?? '',
    redirectUrisStr: row.redirectUris.join('\n'), permissionsStr: row.permissions.join('\n')
  })
  dialogVisible.value = true
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitLoading.value = true
  try {
    const redirectUris = form.redirectUrisStr.split('\n').map(s => s.trim()).filter(Boolean)
    const permissions = form.permissionsStr.split('\n').map(s => s.trim()).filter(Boolean)
    if (isEdit.value) {
      await updateOAuthClient(editingClientId.value, { displayName: form.displayName || undefined, redirectUris, permissions })
      ElMessage.success('更新成功')
    } else {
      await createOAuthClient({ clientId: form.clientId, clientSecret: form.clientSecret || undefined, displayName: form.displayName || undefined, clientType: form.clientType || undefined, redirectUris, permissions })
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadData()
  } finally {
    submitLoading.value = false
  }
}

async function handleDelete(clientId: string) {
  await deleteOAuthClient(clientId)
  ElMessage.success('已删除')
  loadData()
}
</script>
