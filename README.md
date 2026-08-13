# ERP-Learning

一个用于学习全栈开发的 ERP 管理系统项目。

## 技术栈

- 前端：Vue 3 + Vite + Element Plus + Vue Router + Axios
- 后端：ASP.NET Core + Dapper
- 数据库：SQL Server

## 项目结构

- ERP.Server：ASP.NET Core 后端项目
- Vue3：Vue 3 前端项目
- Database：数据库初始化脚本

## 已完成功能

- 用户管理
- 商品管理
- 商品分类管理
- 供应商管理
- 公司管理
- 站点管理
- 地区数据管理

## 开发计划

- JWT 身份认证
- Redis 缓存
- 仓库管理
- 到货单管理
- 库存管理

## 运行环境

- .NET
- Node.js
- SQL Server

## 项目配置

### 前端配置

复制 `Vue3/.env.example` 并创建 `.env.development` 文件，然后配置后端 API 地址：

```env
VITE_API_BASE_URL=https://localhost:xxxx/api
```

其中 `xxxx` 请修改为本地 ASP.NET Core 后端实际运行的端口。

### 后端配置

数据库连接字符串不保存在 Git 仓库中，请使用 .NET User Secrets 配置。

在 `ERP.Server/ERP.Server` 项目中配置 `ConnectionStrings:DefaultConnection`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "你的 SQL Server 连接字符串"
  }
}
```

请根据本地 SQL Server 环境配置服务器地址、数据库名称、用户名和密码等信息。

## 数据库初始化

数据库初始化脚本位于：

```text
Database/ERP.Database.sql
```

在 SQL Server 中执行该脚本即可创建 `ERP_DB` 数据库及项目所需的数据表。

当前初始化脚本主要用于首次创建数据库结构，不包含本地测试数据。

## 安装与启动

### 1. 安装前端依赖

进入 `Vue3` 目录：

```bash
cd Vue3
npm install
```

### 2. 配置前端环境

根据 `Vue3/.env.example` 创建 `.env.development`，并配置自己的后端 API 地址。

### 3. 初始化数据库

在 SQL Server 中执行：

```text
Database/ERP.Database.sql
```

创建 `ERP_DB` 数据库及相关数据表。

### 4. 配置后端数据库连接

使用 .NET User Secrets 配置：

```text
ConnectionStrings:DefaultConnection
```

### 5. 启动后端

使用 Visual Studio 打开：

```text
ERP.Server/ERP.Server.slnx
```

启动 ASP.NET Core 项目。

### 6. 启动前端

进入 `Vue3` 目录并执行：

```bash
npm run dev
```

根据终端输出的本地地址访问 ERP 前端项目。