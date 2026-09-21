<template>
  <div>
    <el-card>
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>用户管理</span>
          <el-button type="primary" @click="openCreateDialog">新增用户</el-button>
        </div>
      </template>

      <el-form :inline="true" :model="query" style="margin-bottom: 16px">
        <el-form-item label="关键词">
          <el-input v-model="query.keyword" placeholder="用户名/姓名" clearable @keyup.enter="loadData" />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="query.status" placeholder="全部" clearable>
            <el-option label="正常" :value="0" />
            <el-option label="禁用" :value="1" />
            <el-option label="锁定" :value="2" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadData">查询</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="loginName" label="登录名" width="140" />
        <el-table-column prop="realName" label="姓名" width="120" />
        <el-table-column prop="mobile" label="手机号" width="140" />
        <el-table-column prop="email" label="邮箱" />
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="statusTagType(row.status)">{{ statusLabel(row.status) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="lastLoginTime" label="最后登录" width="170">
          <template #default="{ row }">{{ formatDateTime(row.lastLoginTime) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="openEditDialog(row)">编辑</el-button>
            <el-button size="small" @click="openResetPwdDialog(row)">重置密码</el-button>
            <el-button v-if="row.status !== 1" size="small" type="warning" @click="handleDisable(row)">禁用</el-button>
            <el-button v-if="row.status === 1" size="small" type="success" @click="handleEnable(row)">启用</el-button>
            <el-popconfirm title="确认删除？" @confirm="handleDelete(row.id)">
              <template #reference>
                <el-button size="small" type="danger">删除</el-button>
              </template>
            </el-popconfirm>
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

    <!-- 创建/编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑用户' : '新增用户'" width="500px">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="80px">
        <template v-if="!isEdit">
          <el-form-item label="登录名" prop="loginName">
            <el-input v-model="form.loginName" />
          </el-form-item>
          <el-form-item label="密码" prop="password">
            <el-input v-model="form.password" type="password" show-password />
          </el-form-item>
        </template>
        <el-form-item label="姓名" prop="realName">
          <el-input v-model="form.realName" />
        </el-form-item>
        <el-form-item label="手机号">
          <el-input v-model="form.mobile" />
        </el-form-item>
        <el-form-item label="邮箱">
          <el-input v-model="form.email" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>

    <!-- 重置密码对话框 -->
    <el-dialog v-model="resetPwdVisible" title="重置密码" width="400px">
      <el-form ref="resetFormRef" :model="resetForm" :rules="resetRules" label-width="80px">
        <el-form-item label="新密码" prop="newPassword">
          <el-input v-model="resetForm.newPassword" type="password" show-password />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="resetPwdVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleResetPwd">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { searchUsers, createUser, updateUser, enableUser, disableUser, deleteUser, resetPassword } from '@/api/user'
import type { UserDto } from '@/api/user'
import { formatDateTime, statusLabel, statusTagType } from '@/utils'

const loading = ref(false)
const tableData = ref<UserDto[]>([])
const total = ref(0)
const query = reactive({ keyword: '', status: undefined as number | undefined, pageIndex: 1, pageSize: 20 })

const dialogVisible = ref(false)
const isEdit = ref(false)
const editingId = ref(0)
const submitLoading = ref(false)
const formRef = ref<FormInstance>()

const form = reactive({ loginName: '', password: '', realName: '', mobile: '', email: '' })
const formRules: FormRules = {
  loginName: [{ required: true, message: '请输入登录名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
  realName: [{ required: true, message: '请输入姓名', trigger: 'blur' }]
}

const resetPwdVisible = ref(false)
const resetUserId = ref(0)
const resetFormRef = ref<FormInstance>()
const resetForm = reactive({ newPassword: '' })
const resetRules: FormRules = {
  newPassword: [{ required: true, message: '请输入新密码', trigger: 'blur' }, { min: 6, message: '密码至少6位', trigger: 'blur' }]
}

onMounted(() => loadData())

async function loadData() {
  loading.value = true
  try {
    const { data } = await searchUsers(query)
    tableData.value = data.items
    total.value = data.total
  } finally {
    loading.value = false
  }
}

function openCreateDialog() {
  isEdit.value = false
  Object.assign(form, { loginName: '', password: '', realName: '', mobile: '', email: '' })
  dialogVisible.value = true
}

function openEditDialog(row: UserDto) {
  isEdit.value = true
  editingId.value = row.id
  Object.assign(form, { loginName: row.loginName, password: '', realName: row.realName, mobile: row.mobile ?? '', email: row.email ?? '' })
  dialogVisible.value = true
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitLoading.value = true
  try {
    if (isEdit.value) {
      await updateUser(editingId.value, { realName: form.realName, mobile: form.mobile || undefined, email: form.email || undefined })
      ElMessage.success('更新成功')
    } else {
      await createUser({ loginName: form.loginName, password: form.password, realName: form.realName, mobile: form.mobile || undefined, email: form.email || undefined, unitId: 0 })
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadData()
  } finally {
    submitLoading.value = false
  }
}

async function handleEnable(row: UserDto) {
  await enableUser(row.id)
  ElMessage.success('已启用')
  loadData()
}

async function handleDisable(row: UserDto) {
  await disableUser(row.id)
  ElMessage.success('已禁用')
  loadData()
}

async function handleDelete(id: number) {
  await deleteUser(id)
  ElMessage.success('已删除')
  loadData()
}

function openResetPwdDialog(row: UserDto) {
  resetUserId.value = row.id
  resetForm.newPassword = ''
  resetPwdVisible.value = true
}

async function handleResetPwd() {
  const valid = await resetFormRef.value?.validate().catch(() => false)
  if (!valid) return
  submitLoading.value = true
  try {
    await resetPassword(resetUserId.value, resetForm.newPassword)
    ElMessage.success('密码已重置')
    resetPwdVisible.value = false
  } finally {
    submitLoading.value = false
  }
}
</script>
