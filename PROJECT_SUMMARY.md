# CMS新闻站引擎项目总结

## 项目概述

本项目是一个基于.NET 8的新闻站引擎，采用模块化单体架构，专注于内容管理和页面渲染的一体化解决方案。系统设计遵循"固定模板、固定模型、简化运营动作、强化发布稳定性、优先保障SEO与性能"的原则，目标是构建一个稳定可运营的新闻站引擎。

## 项目定位

- **面向资讯站/新闻站**
- **内容管理 + 页面渲染一体化**
- **模板固定，改动频率低**
- **编辑人员主要关注：标题、摘要、正文、头图/视频、栏目、标签、发布时间**
- **系统重点追求：稳定、易运营、易维护、SEO友好、前台性能好**

一句话概括：做一套"稳定可运营的新闻站引擎"，而不是一个大而全的平台。

## 技术架构

### 技术栈
- **框架**: .NET 8 / ASP.NET Core 8
- **前台**: ASP.NET Core MVC + Razor (服务端渲染)
- **后台接口**: ASP.NET Core Web API
- **管理后台前端**: Vue + Vue Router + Pinia + Axios + Element Plus/Ant Design Vue
- **数据访问**: Entity Framework Core + Dapper
  - **EF Core**: 常规增删改查、事务型业务、实体映射
  - **Dapper**: 后台复杂列表查询、前台高性能只读查询、统计/聚合查询
- **数据库**: SQL Server 或 MySQL
  - **SQL Server**: 偏.NET传统企业栈，更顺手
  - **MySQL**: 更看重成本和Linux部署便利
- **缓存**: MemoryCache (单机部署，不需要Redis)
- **富文本编辑器**: CKEditor 4.16.2
- **前端框架**: Bootstrap 5.3

### 项目结构
```
Cms/
├── Cms.Domain/           # 领域层 - 实体、值对象、枚举、核心业务规则
├── Cms.Infrastructure/    # 基础设施层 - EF Core DbContext、Repository实现、Dapper查询实现、文件存储、缓存实现
├── Cms.Application/      # 应用服务层 - 用例编排、Command/Query、DTO、接口定义、权限校验编排、发布流程编排、富文本清洗、缓存清理编排
├── Cms.Web/            # 前台网站 - 新闻展示、网站路由、前台页面渲染、SEO页面输出
├── Cms.WebApi/         # 后台接口 - 为Vue管理后台提供API、登录认证、用户/角色/权限管理、栏目/文章/标签/专题/推荐位/资源管理
├── Cms.Admin/          # 后台管理 - 内容管理（Vue前端）
└── Cms.Shared/         # 共享层 - 公共常量、基础枚举、通用返回结构、公共异常、公共工具接口
```

## 核心功能模块

### 1. 数据库模型设计
- **CmsWebsite**: 站点表，支持多域名站群
- **CmsChannel**: 栏目表，支持多级栏目结构
- **CmsArticle**: 文章主表，存储文章基础信息
- **CmsArticleContent**: 文章内容表，存储HTML和纯文本内容
- **CmsTag**: 标签表
- **CmsArticleTag**: 文章标签关联表
- **CmsTopic**: 专题表
- **CmsTopicArticle**: 专题文章关联表
- **CmsMediaAsset**: 媒体资源表
- **CmsRecommendSlot**: 推荐位表
- **CmsRecommendItem**: 推荐项表
- **CmsUser**: 用户表
- **CmsRole**: 角色表
- **CmsPermission**: 权限表
- **CmsUserRole**: 用户角色关联表
- **CmsRolePermission**: 角色权限关联表
- **CmsOperationLog**: 操作日志表

### 2. Website管理模块
- 站点名称、域名配置
- 站点Logo、默认SEO配置
- 页脚信息管理
- 站点启用/禁用控制
- 支持多域名站群

### 3. 栏目管理模块
- 栏目树维护
- 栏目排序
- 栏目启用/停用
- 栏目SEO
- 栏目slug
- 栏目模板类型
- 是否显示在导航
- 技术要点：栏目是新闻站骨架，建议控制在2~3级，栏目查询要做缓存，URL规则要提前固定

### 4. 文章管理模块
- 文章新增/编辑
- 草稿保存
- 发布
- 下线
- 删除
- 置顶/推荐/头条
- 封面图/视频
- 作者/来源/摘要/标签
- SEO字段
- 建议拆表：Articles、ArticleContents
- 技术要点：主表存轻字段，正文单独拆表，支持草稿与发布态分离，列表查询优先用Dapper，保存时做富文本清洗

### 5. 标签管理模块
- 标签维护
- 标签与文章关联
- 标签页聚合展示
- 技术要点：标签独立表，中间关系表单独建，标签页可作为SEO聚合页

### 6. 专题管理模块
- 专题创建
- 专题内容聚合
- 专题SEO
- 专题前台展示
- 说明：专题不是标签，建议独立建模

### 7. 推荐位/广告位管理模块
- 首页焦点图
- 首页头条
- 栏目推荐
- 热门推荐
- 文末相关阅读
- 广告位管理
- 技术要点：推荐位独立建模，支持排序，支持生效时间，首页和栏目页内容不要写死在代码里

### 8. 媒体资源管理模块
- 图片上传
- 视频上传或视频地址录入
- 文件类型校验
- 文件大小限制
- 简单分类
- 引用关系查询
- 技术要点：统一上传接口，独立资源表，资源是否被文章引用要可追踪

### 9. SEO管理模块
- 文章SEO
- 栏目SEO
- 专题SEO
- 默认SEO
- slug
- sitemap
- robots
- canonical
- 重定向规则（可后加）
- 技术要点：SEO是传统新闻站核心能力之一，URL要尽早固定，模板里不要写死SEO信息

### 10. 用户/角色/权限管理模块
- 用户管理
- 角色管理
- 菜单权限
- 按钮权限
- 栏目范围权限
- 建议角色：超级管理员、编辑、审核/发布人员、运营人员
- 技术要点：采用RBAC，叠加栏目范围控制，对新闻站来说，栏目级数据权限很实用

### 11. 操作日志/审计日志模块
- 登录日志
- 文章操作日志
- 发布日志
- 删除日志
- 权限变更日志
- 站点/栏目配置修改日志
- 技术要点：发布、删除、下线必须留痕，方便排查问题和责任追踪

### 12. 搜索模块
- 前台站内搜索
- 后台文章搜索
- 标题/摘要/标签检索
- 技术要点：一期可以先用数据库搜索，正文保存PlainText，后续数据量大了再升级全文检索

### 13. 前台页面渲染
- 首页展示（焦点图、栏目新闻区块、热门推荐）
- 栏目列表页（分页、筛选）
- 文章详情页（面包屑、正文、上一篇/下一篇、相关阅读）
- 标签聚合页
- 专题页
- 搜索结果页
- 404页面
- sitemap.xml
- robots.txt
- 响应式设计

### 14. 缓存策略
- MemoryCache（单机部署，不需要Redis）
- 适合缓存的内容：首页数据、栏目树、栏目列表、文章详情、推荐位、热门文章、标签页聚合
- 缓存失效建议：文章发布后，清理文章详情缓存、所属栏目页缓存、首页推荐缓存、标签页缓存、专题页缓存
- 缓存键规则：必须包含WebsiteId，例如：website:1:home, website:1:channel:12:list:1, website:2:article:345, website:3:tag:ai

### 15. 富文本清洗
- 目的：防XSS、清除Word垃圾HTML、去掉无用style/font/span、保证前台样式稳定
- 放置位置：Application层，在保存ArticleContent前执行
- 推荐流程：编辑器HTML → 清洗HTML → 提取PlainText → 保存ArticleContent
- 建议保存字段：HtmlContent、PlainText、WordCount

### 16. 草稿与发布分离
- 建议至少支持状态：草稿、待审核（可选）、已发布、已下线
- 技术要点：编辑中的内容不能直接影响线上，发布动作要明确，可逐步增加版本或发布快照

### 17. URL与路由设计
- 建议提前固定URL规则：
  - 首页：/
  - 栏目页：/news/
  - 文章页：/news/2026/03/12/12345.html
  - 专题页：/special/ai-2026/
  - 标签页：/tag/dotnet/
- 技术要点：URL稳定优先，slug规则统一，后期改URL成本很高

## 数据库配置

### 连接字符串
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CmsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 缓存配置
```csharp
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
```

## 路由设计

### 前台路由
- `/` - 首页
- `/{channelSlug}` - 栏目列表页
- `/{channelSlug}/{year:int}/{month:int}/{day:int}/{id}.html` - 文章详情页
- `/tag/{tagSlug}` - 标签聚合页
- `/special/{topicSlug}` - 专题页
- `/search` - 搜索页
- `/sitemap.xml` - 站点地图
- `/robots.txt` - 爬虫规则

### 后台API路由
- `/api/auth/*` - 认证相关
- `/api/articles/*` - 文章管理
- `/api/channels/*` - 栏目管理
- `/api/tags/*` - 标签管理
- `/api/topics/*` - 专题管理
- `/api/media/*` - 媒体资源管理
- `/api/recommend/*` - 推荐位管理
- `/api/users/*` - 用户管理
- `/api/roles/*` - 角色管理
- `/api/websites/*` - 站点管理

## 已实现的服务

### 应用服务层
- `IArticleService` / `ArticleService` - 文章服务
- `IUserService` / `UserService` - 用户服务
- `IChannelService` / `ChannelService` - 栏目服务
- `IMediaAssetService` / `MediaAssetService` - 媒体资源服务
- `IRecommendService` / `RecommendService` - 推荐位服务
- `ISEOService` / `SEOService` - SEO服务
- `IWebsiteService` / `WebsiteService` - 站点服务
- `ITagService` / `TagService` - 标签服务
- `ITopicService` / `TopicService` - 专题服务

### 基础设施服务层
- `ICacheService` / `MemoryCacheService` - 缓存服务
- `CmsDbContext` - 数据库上下文

## 部署方案

### 服务器环境
- Linux服务器
- 建议使用：Nginx、.NET 8 Runtime、systemd
- 数据库可单独部署，也可同机部署（看规模）

### 部署形态
建议部署为两个独立站点：

**管理后台站点**
- 一个独立站点，用于后台管理
- 例如：admin.yourdomain.com
- 职责：访问Vue管理后台、调用WebApi、后台管理、文章发布、资源上传、权限管理
- 这部分是独立后台入口，不对外开放普通前台访问

**Web前台站点**
- 一个独立站点，用于新闻网站前台展示
- 可以同时绑定多个域名：www.sitea.com、www.siteb.com、www.sitec.com
- 这些域名都指向同一个Web前台程序
- 也就是说：程序是一套，域名可以有多个，根据请求域名识别当前属于哪个Website，再按Website读取对应的数据和配置

### 前台多域名绑定方案
这是站群新闻站的关键点：

**核心思路**
- 同一个Cms.Web程序绑定多个域名
- 每次请求进入系统时：
  1. 读取请求Host
  2. 根据Host查询Website
  3. 获取当前WebsiteId
  4. 后续所有前台数据查询，都按WebsiteId过滤

**示例**
- www.sitea.com -> WebsiteId = 1
- www.siteb.com -> WebsiteId = 2
- www.sitec.com -> WebsiteId = 3

这样虽然程序只有一套，但能根据不同域名展示不同站点内容。

### 数据隔离要求
既然前台要根据域名区分站点内容，那么数据库层面就不能只在文章表里随便区分一下，而是要明确站点级数据隔离。

**建议这些表带WebsiteId**：
- Channels
- Articles
- Tags
- Topics
- RecommendSlots
- RecommendItems
- MediaAssets
- SeoRedirects
- WebsiteSettings

ArticleContents可以不带WebsiteId，因为它可以通过ArticleId反查所属站点。

### 查询规则
前台任何查询都必须带WebsiteId过滤：
- 栏目列表
- 文章详情
- 标签页
- 专题页
- 推荐位
- 热门文章
- sitemap
- robots
- SEO配置

否则会出现跨站串数据问题。

### 缓存规则
缓存key也必须带WebsiteId：
- website:1:home
- website:1:channel:12:list:1
- website:2:article:345
- website:3:tag:ai

否则不同域名命中同一份缓存，会直接串站。

### 站点识别实现建议
建议在前台项目里做一个CurrentWebsiteResolver或中间件：
- 职责：从HttpContext.Request.Host取域名、查询Website配置、得到当前WebsiteId、存入当前请求上下文、后续应用层/查询层统一读取
- 这样整个前台查询链路都能基于当前站点工作

### Nginx部署建议
**管理后台**
- 独立server配置，反向代理到后台应用

**Web前台**
- 一个前台程序可配置多个server_name，或者多个server指向同一个upstream
- 例如思路上是：多个域名都进同一个Cms.Web，Cms.Web内部根据Host判断Website
- 这样最符合"多域名 + 单前台程序"的目标

### 应用进程建议
建议后台和前台分开部署成两个systemd服务：
- cms-admin.service
- cms-web.service

**好处**：
- 前后台独立重启
- 日志独立
- 部署更清晰
- 资源隔离更好

### 这种部署方式的优点
这个方案非常适合当前目标：
- 后台独立，安全边界清晰
- 前台只有一套程序，维护成本低
- 多域名绑定方便
- 支持站群
- 不容易出现多套代码分叉
- 模板固定的前提下，数据驱动差异最合理

## 后续可改进方向

### 功能扩展
1. **评论系统**: 添加文章评论功能
2. **会员系统**: 实现用户注册、登录、个人中心
3. **多语言支持**: 实现国际化功能
4. **复杂审批流**: 添加文章审核流程
5. **统计分析**: 实现访问统计、用户行为分析
6. **广告管理系统**: 完善广告位管理

### 性能优化
1. **CDN集成**: 静态资源CDN加速
2. **图片压缩**: 自动图片压缩和优化
3. **数据库优化**: 添加索引、查询优化
4. **分布式缓存**: Redis集群支持（当需要扩展时）

### 安全增强
1. **HTTPS强制**: 启用HTTPS
2. **防CSRF**: 添加CSRF保护
3. **防XSS**: 输入输出过滤
4. **敏感词检测**: 内容审核功能

### 运维支持
1. **日志系统**: 完善日志记录
2. **监控告警**: 系统监控和告警
3. **备份恢复**: 数据库备份和恢复
4. **部署脚本**: 自动化部署脚本

## 开发说明

### 运行项目
1. 确保SQL Server LocalDB或MySQL已安装
2. 运行`dotnet build`编译项目
3. 运行`dotnet run --project Cms.Web`启动前台
4. 运行`dotnet run --project Cms.WebApi`启动后台API

### 数据库迁移
```bash
dotnet ef migrations add InitialCreate --project Cms.Infrastructure --startup-project Cms.Web
dotnet ef database update --project Cms.Infrastructure --startup-project Cms.Web
```

### 依赖安装
```bash
dotnet restore
```

## 总结

本项目成功实现了一个功能完整的新闻站引擎，涵盖了内容管理、用户认证、权限管理、SEO优化等核心功能。系统架构清晰，代码结构合理，易于维护和扩展。通过采用固定模板和简化运营动作的设计理念，系统具有良好的稳定性和可维护性。

特别支持多域名站群功能，通过单一程序实例绑定多个域名，根据域名自动识别站点并隔离数据，实现了高效的站群管理。

系统采用MemoryCache进行缓存，优化了前台性能，同时通过EF Core和Dapper的职责分离，保证了数据访问的效率和可维护性。

本系统非常适合新闻站和资讯站的使用场景，为内容管理和发布提供了稳定可靠的解决方案。