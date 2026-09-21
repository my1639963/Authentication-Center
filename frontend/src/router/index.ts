import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/login/LoginView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/',
    component: () => import('@/layouts/AdminLayout.vue'),
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/dashboard/DashboardView.vue'),
        meta: { title: '仪表盘', icon: 'Odometer' }
      },
      {
        path: 'users',
        name: 'Users',
        component: () => import('@/views/user/UserListView.vue'),
        meta: { title: '用户管理', icon: 'User' }
      },
      {
        path: 'roles',
        name: 'Roles',
        component: () => import('@/views/role/RoleListView.vue'),
        meta: { title: '角色管理', icon: 'Avatar' }
      },
      {
        path: 'permissions',
        name: 'Permissions',
        component: () => import('@/views/permission/PermissionListView.vue'),
        meta: { title: '权限管理', icon: 'Lock' }
      },
      {
        path: 'organizations/regions',
        name: 'Regions',
        component: () => import('@/views/organization/RegionView.vue'),
        meta: { title: '区域管理', icon: 'MapLocation', parent: '组织管理' }
      },
      {
        path: 'organizations/units',
        name: 'Units',
        component: () => import('@/views/organization/UnitView.vue'),
        meta: { title: '单位管理', icon: 'OfficeBuilding', parent: '组织管理' }
      },
      {
        path: 'organizations/departments',
        name: 'Departments',
        component: () => import('@/views/organization/DepartmentView.vue'),
        meta: { title: '部门管理', icon: 'FolderOpened', parent: '组织管理' }
      },
      {
        path: 'organizations/positions',
        name: 'Positions',
        component: () => import('@/views/organization/PositionView.vue'),
        meta: { title: '岗位管理', icon: 'Briefcase', parent: '组织管理' }
      },
      {
        path: 'oauth-clients',
        name: 'OAuthClients',
        component: () => import('@/views/oauth-client/OAuthClientView.vue'),
        meta: { title: 'OAuth 客户端', icon: 'Connection' }
      },
      {
        path: 'logs/login',
        name: 'LoginLogs',
        component: () => import('@/views/log/LoginLogView.vue'),
        meta: { title: '登录日志', icon: 'Notebook', parent: '日志管理' }
      },
      {
        path: 'logs/audit',
        name: 'AuditLogs',
        component: () => import('@/views/log/AuditLogView.vue'),
        meta: { title: '审计日志', icon: 'Document', parent: '日志管理' }
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth !== false && !token) {
    return { name: 'Login', query: { redirect: to.fullPath } }
  }
})

export default router
