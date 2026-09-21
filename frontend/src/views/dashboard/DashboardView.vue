<template>
  <div>
    <h2 style="margin-bottom: 24px">欢迎使用认证中心管理系统</h2>

    <!-- 统计卡片 -->
    <el-row :gutter="20" style="margin-bottom: 24px">
      <el-col :span="4" v-for="item in statCards" :key="item.label">
        <el-card shadow="hover" :body-style="{ padding: '20px' }">
          <div style="text-align: center">
            <el-icon :size="32" :color="item.color" style="margin-bottom: 8px">
              <component :is="item.icon" />
            </el-icon>
            <div style="font-size: 28px; font-weight: bold; color: #303133">{{ item.value }}</div>
            <div style="font-size: 13px; color: #909399; margin-top: 4px">{{ item.label }}</div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 今日登录统计 -->
    <el-row :gutter="20" style="margin-bottom: 24px">
      <el-col :span="12">
        <el-card shadow="hover" style="min-height: 260px">
          <template #header>
            <span style="font-weight: 600">今日登录统计</span>
          </template>
          <el-row :gutter="20">
            <el-col :span="12">
              <div style="text-align: center; padding: 16px 0">
                <el-icon :size="40" color="#67c23a"><SuccessFilled /></el-icon>
                <div style="font-size: 32px; font-weight: bold; color: #67c23a; margin-top: 8px">
                  {{ stats?.todayLoginSuccess ?? 0 }}
                </div>
                <div style="color: #909399; margin-top: 4px">登录成功</div>
              </div>
            </el-col>
            <el-col :span="12">
              <div style="text-align: center; padding: 16px 0">
                <el-icon :size="40" color="#f56c6c"><CircleCloseFilled /></el-icon>
                <div style="font-size: 32px; font-weight: bold; color: #f56c6c; margin-top: 8px">
                  {{ stats?.todayLoginFail ?? 0 }}
                </div>
                <div style="color: #909399; margin-top: 4px">登录失败</div>
              </div>
            </el-col>
          </el-row>
        </el-card>
      </el-col>

      <!-- 客户端清单 -->
      <el-col :span="12">
        <el-card shadow="hover">
          <template #header>
            <div style="display: flex; justify-content: space-between; align-items: center">
              <span style="font-weight: 600">客户端清单</span>
              <el-button text type="primary" size="small" @click="$router.push('/oauth-clients')">
                管理客户端
              </el-button>
            </div>
          </template>
          <el-table :data="oauthClients" size="small" stripe style="min-height: 260px">
            <el-table-column prop="clientId" label="Client ID" />
            <el-table-column prop="displayName" label="名称" />
            <el-table-column prop="clientType" label="类型" width="100" />
            <el-table-column label="权限" width="120">
              <template #default="{ row }">
                <el-tag v-for="p in row.permissions" :key="p" size="small" style="margin-right: 4px">
                  {{ p }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
    </el-row>

    <!-- 最近日志 -->
    <el-row :gutter="20">
      <el-col :span="12">
        <el-card shadow="hover">
          <template #header>
            <div style="display: flex; justify-content: space-between; align-items: center">
              <span style="font-weight: 600">最近登录日志</span>
              <el-button text type="primary" size="small" @click="$router.push('/logs/login')">
                查看全部
              </el-button>
            </div>
          </template>
          <el-table :data="recentLogins" size="small" stripe style="min-height: 260px">
            <el-table-column prop="loginName" label="账号" width="120" />
            <el-table-column prop="loginType" label="类型" width="100" />
            <el-table-column label="结果" width="80">
              <template #default="{ row }">
                <el-tag :type="row.loginResult === 0 ? 'success' : 'danger'" size="small">
                  {{ row.loginResult === 0 ? '成功' : '失败' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="ipAddress" label="IP" />
            <el-table-column prop="createTime" label="时间" width="170">
              <template #default="{ row }">
                {{ formatTime(row.createTime) }}
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>

      <el-col :span="12">
        <el-card shadow="hover">
          <template #header>
            <div style="display: flex; justify-content: space-between; align-items: center">
              <span style="font-weight: 600">最近审计日志</span>
              <el-button text type="primary" size="small" @click="$router.push('/logs/audit')">
                查看全部
              </el-button>
            </div>
          </template>
          <el-table :data="recentAudits" size="small" stripe style="min-height: 260px">
            <el-table-column prop="eventType" label="事件类型" width="120" />
            <el-table-column prop="operationType" label="操作类型" width="120" />
            <el-table-column label="结果" width="80">
              <template #default="{ row }">
                <el-tag :type="row.eventResult === 0 ? 'success' : 'danger'" size="small">
                  {{ row.eventResult === 0 ? '成功' : '失败' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="createTime" label="时间" width="170">
              <template #default="{ row }">
                {{ formatTime(row.createTime) }}
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, markRaw, onMounted } from 'vue'
import {
  User, Avatar, Lock, OfficeBuilding, FolderOpened, Briefcase
} from '@element-plus/icons-vue'
import { getDashboardStats, getRecentLogins, getRecentAudits } from '@/api/dashboard'
import type { DashboardStats, LoginLogItem, AuditLogItem } from '@/api/dashboard'
import { getAllOAuthClients, type OAuthClientDto } from '@/api/oauthClient'

const stats = ref<DashboardStats>()
const recentLogins = ref<LoginLogItem[]>([])
const recentAudits = ref<AuditLogItem[]>([])

const statCards = ref([
  { label: '用户总数', value: 0, icon: markRaw(User), color: '#409eff' },
  { label: '角色数量', value: 0, icon: markRaw(Avatar), color: '#67c23a' },
  { label: '权限数量', value: 0, icon: markRaw(Lock), color: '#e6a23c' },
  { label: '单位数量', value: 0, icon: markRaw(OfficeBuilding), color: '#909399' },
  { label: '部门数量', value: 0, icon: markRaw(FolderOpened), color: '#f56c6c' },
  { label: '岗位数量', value: 0, icon: markRaw(Briefcase), color: '#9c27b0' }
])

const oauthClients = ref<OAuthClientDto[]>([])

function formatTime(time: string) {
  if (!time) return '-'
  return new Date(time).toLocaleString('zh-CN', {
    month: '2-digit', day: '2-digit',
    hour: '2-digit', minute: '2-digit', second: '2-digit'
  })
}

async function loadData() {
  // 统计数据
  try {
    const statsRes = await getDashboardStats()
    stats.value = statsRes.data
    statCards.value[0].value = statsRes.data.userCount
    statCards.value[1].value = statsRes.data.roleCount
    statCards.value[2].value = statsRes.data.permissionCount
    statCards.value[3].value = statsRes.data.unitCount
    statCards.value[4].value = statsRes.data.departmentCount
    statCards.value[5].value = statsRes.data.positionCount
  } catch (err) {
    console.error('Stats load failed:', err)
  }

  // 最近登录日志
  try {
    const loginsRes = await getRecentLogins(5)
    recentLogins.value = loginsRes.data
  } catch (err) {
    console.error('Login logs load failed:', err)
  }

  // 最近审计日志
  try {
    const auditsRes = await getRecentAudits(5)
    recentAudits.value = auditsRes.data
  } catch (err) {
    console.error('Audit logs load failed:', err)
  }

  // OAuth 客户端
  try {
    const clientsRes = await getAllOAuthClients()
    oauthClients.value = clientsRes.data
  } catch (err) {
    console.error('OAuth clients load failed:', err)
  }
}

onMounted(loadData)
</script>
