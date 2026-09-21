# Vue 3 前端管理系统

## 技术栈
- Vue 3 + Vite + TypeScript
- Element Plus (UI 组件库)
- Pinia (状态管理)
- Vue Router (路由)
- Axios (HTTP 请求)

## 项目结构

```
frontend/
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
├── env.d.ts
├── src/
│   ├── main.ts
│   ├── App.vue
│   ├── api/              # API 请求模块
│   │   ├── request.ts    # Axios 实例配置（拦截器、token 注入）
│   │   ├── auth.ts       # 登录 API
│   │   ├── user.ts       # 用户管理 API
│   │   ├── admin.ts      # 角色/权限管理 API
│   │   ├── organization.ts # 组织管理 API（区域/单位/部门/岗位）
│   │   ├── oauthClient.ts  # OAuth 客户端 API
│   │   └── log.ts        # 日志 API
│   ├── router/
│   │   └── index.ts      # 路由配置（含路由守卫）
│   ├── stores/
│   │   └── user.ts       # 用户状态（token、用户信息）
│   ├── layouts/
│   │   └── AdminLayout.vue  # 后台管理布局（侧边栏 + 顶部栏 + 内容区）
│   ├── views/
│   │   ├── login/
│   │   │   └── LoginView.vue          # 登录页
│   │   ├── dashboard/
│   │   │   └── DashboardView.vue      # 仪表盘首页
│   │   ├── user/
│   │   │   └── UserListView.vue       # 用户管理（列表、搜索、CRUD 弹窗）
│   │   ├── role/
│   │   │   └── RoleListView.vue       # 角色管理（CRUD、权限分配）
│   │   ├── permission/
│   │   │   └── PermissionListView.vue # 权限管理（列表、分组查看）
│   │   ├── organization/
│   │   │   ├── RegionView.vue         # 区域管理（树形）
│   │   │   ├── UnitView.vue           # 单位管理（树形 + 分页）
│   │   │   ├── DepartmentView.vue     # 部门管理（树形）
│   │   │   └── PositionView.vue       # 岗位管理（分页列表）
│   │   ├── oauth-client/
│   │   │   └── OAuthClientView.vue    # OAuth 客户端管理
│   │   └── log/
│   │       ├── LoginLogView.vue       # 登录日志
│   │       └── AuditLogView.vue       # 审计日志
│   └── utils/
│       └── index.ts      # 工具函数
```

## 路由设计

| 路径 | 页面 | 说明 |
|------|------|------|
| /login | LoginView | 登录页（无需认证） |
| / | AdminLayout | 后台布局容器 |
| /dashboard | DashboardView | 仪表盘 |
| /users | UserListView | 用户管理 |
| /roles | RoleListView | 角色管理 |
| /permissions | PermissionListView | 权限管理 |
| /organizations/regions | RegionView | 区域管理 |
| /organizations/units | UnitView | 单位管理 |
| /organizations/departments | DepartmentView | 部门管理 |
| /organizations/positions | PositionView | 岗位管理 |
| /oauth-clients | OAuthClientView | OAuth 客户端 |
| /logs/login | LoginLogView | 登录日志 |
| /logs/audit | AuditLogView | 审计日志 |

## 后端 CORS 配置修改

修改 `src/AuthCenter.Web/Program.cs`：
- 添加 `builder.Services.AddCors(...)` 配置，允许前端开发服务器 `http://localhost:5173` 的跨域请求
- 在中间件管道中添加 `app.UseCors()`

## 实施步骤

### 步骤 1：初始化前端项目
- 使用 `npm create vue@latest` 创建 Vue 3 + TypeScript 项目到 `frontend/` 目录
- 安装依赖：element-plus, pinia, vue-router, axios

### 步骤 2：后端 CORS 配置
- 修改 `Program.cs` 添加 CORS 服务注册和中间件

### 步骤 3：搭建基础架构
- 配置 Axios 实例（baseURL、token 拦截器、错误处理）
- 配置 Pinia store（用户 token 持久化）
- 配置 Vue Router（路由定义 + 登录守卫）
- 创建 AdminLayout 布局组件

### 步骤 4：实现登录页
- LoginView.vue：表单验证，调用 `/api/auth/login`，存储 token

### 步骤 5：实现各管理页面
- 用户管理：分页列表 + 搜索 + 创建/编辑/启用/禁用/重置密码
- 角色管理：列表 + CRUD + 权限分配弹窗
- 权限管理：列表 + 按分组筛选
- 组织管理：区域树、单位树+分页、部门树、岗位分页列表
- OAuth 客户端：CRUD 管理
- 日志查看：登录日志 + 审计日志（分页查询 + 筛选）
- 仪表盘：简单的欢迎页/统计概览

### 步骤 6：验证
- 启动前端开发服务器，确认页面可正常访问和导航
