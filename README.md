# 统一认证中心

统一认证中心为业务子系统提供统一的用户身份认证能力，实现"一次登录、多系统访问"。系统基于 **OAuth 2.0 / OpenID Connect** 标准协议，通过 JWT 令牌机制完成身份认证与验证，支持 SSO 单点登录。

## 架构定位

```text
                  ┌─────────────────────┐
                  │     统一认证中心      │
                  │                     │
                  │  用户身份管理        │
                  │  组织机构管理        │
                  │  OAuth 2.0 / OIDC   │
                  │  JWT / JWKS         │
                  │  SSO 单点登录        │
                  │  认证审计            │
                  └──────────┬──────────┘
                             │
                       OAuth / OIDC
                             │
          ┌──────────────────┼──────────────────┐
          ▼                  ▼                  ▼
    ┌──────────┐       ┌──────────┐       ┌──────────┐
    │ 子系统A  │       │ 子系统B   │       │ 子系统C  │
    └────┬─────┘       └────┬─────┘       └────┬─────┘
         ▼                  ▼                  ▼
     JWT 本地验证        JWT 本地验证        JWT 本地验证
     本地角色/权限       本地角色/权限       本地角色/权限
```

**核心原则：统一身份、统一认证、标准协议、分散授权、分散验证。**

- 认证中心负责身份认证（"用户是谁"）
- 业务子系统负责业务授权（"用户能做什么"）
- 业务系统通过标准 OAuth/OIDC 协议接入，无需依赖专用 SDK

## 技术栈

| 层级        | 技术                                                           |
| ----------- | -------------------------------------------------------------- |
| 后端框架    | .NET 10 / ASP.NET Core                                         |
| ORM         | Entity Framework Core 10                                       |
| OAuth/OIDC  | OpenIddict 7.7                                                 |
| 数据库      | MySQL（开发）/ 国产数据库（生产，如人大金仓、达梦、OceanBase） |
| API 文档    | OpenAPI + Scalar                                               |
| 前端框架    | Vue 3 + TypeScript                                             |
| UI 组件     | Element Plus                                                   |
| 状态管理    | Pinia                                                          |
| 构建工具    | Vite                                                           |
| 路由        | Vue Router 4                                                   |
| HTTP 客户端 | Axios                                                          |

## 项目结构

```text
Authentication-Center/
├── src/
│   ├── AuthCenter.Domain/            # 领域层 — 实体、枚举、仓储接口、领域服务接口
│   ├── AuthCenter.Application/       # 应用层 — DTO、应用服务接口与实现
│   ├── AuthCenter.Infrastructure/    # 基础设施层 — EF Core、仓储实现、密码与 ID 生成
│   ├── AuthCenter.OpenIddict/        # OpenIddict 集成 — OAuth/OIDC 协议服务
│   └── AuthCenter.Web/               # 表示层 — ASP.NET Core Web 应用、API 控制器
├── frontend/                         # Vue 3 前端管理应用
├── tests/
│   └── AuthCenter.Tests/             # 单元测试
├── docs/                             # 需求说明、详细设计、数据库脚本等文档
└── tools/                            # 辅助工具
```

## 功能模块

### 用户管理

- 用户 CRUD（创建、查询、修改、删除）
- 用户启用/禁用
- 密码管理（设置、修改、管理员重置）
- 密码历史记录与复杂度校验
- 登录失败锁定策略

### 组织机构管理

- **行政区划**：省、市、县及下级行政区划维护
- **单位管理**：单位树结构，支持层级关系
- **部门管理**：单位内部部门管理
- **岗位/职务管理**：基础职务信息管理
- **用户任职管理**：用户与组织关系维护，支持主任职与多任职

### 认证服务

- OAuth 2.0 Authorization Code + PKCE
- OAuth 2.0 Client Credentials
- Refresh Token（含 Rotation 机制）
- OpenID Connect（Discovery、UserInfo、ID Token）
- JWT Access Token（RS256 签名）
- JWKS 公钥发布
- SSO 单点登录 / 单点注销

### 管理权限

- 认证中心自身 RBAC 权限控制
- 管理角色分配（超级管理员、用户管理员、组织管理员等）
- 管理权限校验（用户管理、组织管理、Client 管理、审计查询等）

### 审计日志

- 登录日志（用户、时间、IP、结果、失败原因）
- 操作审计日志（INSERT-only + Hash Chain 完整性校验）
- OAuth Token 审计

### OAuth Client 管理

- 客户端注册与管理
- 通过 OpenIddict Manager API 操作，不直接 CRUD 框架表

## 快速开始

### 环境要求

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (前端开发)
- MySQL 或其他兼容数据库

### 后端启动

1. 配置数据库连接字符串：

```json
// src/AuthCenter.Web/appsettings.json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=3306;Database=authcenter;Uid=root;Pwd=your_password;"
  }
}
```

2. 初始化数据库（参考 `docs/` 目录下的 SQL 脚本或 EF Core Migration）
3. 启动后端服务：

```bash
dotnet run --project src/AuthCenter.Web
```

后端默认运行在 `http://localhost:5221`。

### 前端启动

```bash
cd frontend
npm install
npm run dev
```

前端开发服务器会自动代理 `/api` 和 `/oauth2` 请求到后端。

### API 文档

开发环境下访问 Scalar API 文档：

```text
http://localhost:5221/scalar
```

## 认证协议接入

业务子系统通过标准 OAuth 2.0 / OIDC 协议接入：

| 端点                                  | 方法 | 说明           |
| ------------------------------------- | ---- | -------------- |
| `/oauth2/authorize`                 | GET  | 授权端点       |
| `/oauth2/token`                     | POST | Token 获取     |
| `/oauth2/revoke`                    | POST | Token 撤销     |
| `/oauth2/introspect`                | POST | Token 内省     |
| `/oauth2/jwks`                      | GET  | JWKS 公钥      |
| `/.well-known/openid-configuration` | GET  | OIDC Discovery |
| `/userinfo`                         | GET  | 用户信息       |

### JWT Claims 示例

```json
{
  "iss": "https://auth.example.gov.cn",
  "sub": "100001",
  "aud": "agriculture-api",
  "exp": 1780000000,
  "iat": 1779999400,
  "jti": "xxx",
  "username": "zhangsan",
  "name": "张三",
  "unit_id": "100",
  "department_id": "200",
  "position_id": "10",
  "token_version": 3
}
```

JWT 中只携带用户身份与主任职信息，**不包含**业务角色、业务权限和 DataScope。

## 管理 API

| 接口                                    | 说明                 |
| --------------------------------------- | -------------------- |
| `GET/POST /api/users`                 | 用户查询与创建       |
| `GET/PUT/DELETE /api/users/{id}`      | 用户详情、修改、删除 |
| `POST /api/users/{id}/enable`         | 启用用户             |
| `POST /api/users/{id}/disable`        | 禁用用户             |
| `POST /api/users/{id}/reset-password` | 重置密码             |
| `GET/POST /api/units`                 | 单位管理             |
| `GET/POST /api/departments`           | 部门管理             |
| `GET/POST /api/regions`               | 行政区划管理         |
| `GET/POST /api/positions`             | 岗位/职务管理        |
| `GET/POST /api/clients`               | OAuth Client 管理    |
| `GET/POST /api/admin/roles`           | 管理角色管理         |
| `GET /api/admin/permissions`          | 管理权限查询         |

## 信创适配

系统面向信创环境设计与部署：

- **数据库**：通过 EF Core 抽象层隔离数据库差异，支持人大金仓、达梦、OceanBase 等国产数据库
- **操作系统**：支持国产操作系统部署
- **密码服务**：提供 `ICryptoProvider` 抽象层，支持软件密码与国密硬件设备适配
- **部署方式**：生产环境采用 Linux + .NET Runtime + 系统服务，不依赖 Docker

## 安全特性

- 密码不可逆 Hash 存储，禁止明文
- 连续登录失败 5 次锁定账户 15 分钟
- JWT 非对称密钥签名（RS256）
- 密钥轮换支持（双 Key 重叠策略）
- Token Version 安全失效机制
- Refresh Token Rotation 防重放
- 审计日志 Hash Chain 完整性校验
- 身份证号加密存储 + Hash 查询
- 全部外部接口 HTTPS

## 文档

- [需求说明](docs/需求说明.md)
- [详细设计](docs/详细设计.md)
- [数据库详细设计](docs/数据库详细设计.md)
- [数据库建表脚本](docs/MYSQLCreateScript.sql)
- [基线迁移脚本](docs/ApplyBaselineMigration.sql)
- [测试数据](docs/SeedTestData.sql)

## 许可证

本项目遵循 [LICENSE](LICENSE) 文件中的许可协议。
