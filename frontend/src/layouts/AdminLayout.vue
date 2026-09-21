<template>
  <el-container class="admin-layout">
    <el-aside :width="isCollapse ? '64px' : '220px'" class="aside">
      <div class="logo">
        <span v-if="!isCollapse">认证中心管理系统</span>
        <span v-else>AC</span>
      </div>
      <el-menu
        :default-active="route.path"
        :collapse="isCollapse"
        router
        background-color="#304156"
        text-color="#bfcbd9"
        active-text-color="#409eff"
      >
        <el-menu-item index="/dashboard">
          <el-icon><Odometer /></el-icon>
          <template #title>仪表盘</template>
        </el-menu-item>

        <el-menu-item index="/users">
          <el-icon><User /></el-icon>
          <template #title>用户管理</template>
        </el-menu-item>

        <el-sub-menu index="org">
          <template #title>
            <el-icon><OfficeBuilding /></el-icon>
            <span>组织管理</span>
          </template>
          <el-menu-item index="/organizations/regions">区域管理</el-menu-item>
          <el-menu-item index="/organizations/units">单位管理</el-menu-item>
          <el-menu-item index="/organizations/departments">部门管理</el-menu-item>
          <el-menu-item index="/organizations/positions">岗位管理</el-menu-item>
        </el-sub-menu>

        <el-menu-item index="/roles">
          <el-icon><Avatar /></el-icon>
          <template #title>角色管理</template>
        </el-menu-item>

        <el-menu-item index="/permissions">
          <el-icon><Lock /></el-icon>
          <template #title>权限管理</template>
        </el-menu-item>

        <el-menu-item index="/oauth-clients">
          <el-icon><Connection /></el-icon>
          <template #title>OAuth 客户端</template>
        </el-menu-item>

        <el-sub-menu index="logs">
          <template #title>
            <el-icon><Notebook /></el-icon>
            <span>日志管理</span>
          </template>
          <el-menu-item index="/logs/login">登录日志</el-menu-item>
          <el-menu-item index="/logs/audit">审计日志</el-menu-item>
        </el-sub-menu>
      </el-menu>
    </el-aside>

    <el-container>
      <el-header class="header">
        <el-icon class="collapse-btn" @click="isCollapse = !isCollapse">
          <Fold v-if="!isCollapse" />
          <Expand v-else />
        </el-icon>
        <div class="header-right">
          <span class="user-name">{{ userStore.realName || userStore.loginName || '管理员' }}</span>
          <el-button text @click="handleLogout">
            <el-icon><SwitchButton /></el-icon>
            退出
          </el-button>
        </div>
      </el-header>

      <el-main class="main">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const isCollapse = ref(false)

function handleLogout() {
  userStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.admin-layout {
  height: 100vh;
}
.aside {
  background-color: #304156;
  transition: width 0.3s;
  overflow: hidden;
}
.logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  font-size: 16px;
  font-weight: bold;
  white-space: nowrap;
  border-bottom: 1px solid #3a4a5b;
}
.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid #e6e6e6;
  background: #fff;
}
.collapse-btn {
  font-size: 20px;
  cursor: pointer;
}
.header-right {
  display: flex;
  align-items: center;
  gap: 12px;
}
.user-name {
  font-size: 14px;
  color: #606266;
}
.main {
  background: #f0f2f5;
}
</style>
