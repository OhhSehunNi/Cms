-- CMS新闻站引擎数据库表结构
-- 创建时间: 2026-03-11
-- 数据库: SQL Server

-- 创建数据库
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CmsDb')
BEGIN
    CREATE DATABASE CmsDb;
END
GO

USE CmsDb;
GO

-- =============================================
-- 1. 栏目表 (CmsChannels)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsChannels')
BEGIN
    CREATE TABLE CmsChannels (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Slug NVARCHAR(200) NULL,
        ParentId INT NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        IsShowInNav BIT NOT NULL DEFAULT 0,
        SeoTitle NVARCHAR(200) NULL,
        SeoDescription NVARCHAR(500) NULL,
        SeoKeywords NVARCHAR(200) NULL,
        TemplateType NVARCHAR(50) NULL,
        IsEnabled BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_CmsChannels_Parent FOREIGN KEY (ParentId) REFERENCES CmsChannels(Id)
    );
    
    CREATE INDEX IX_CmsChannels_ParentId ON CmsChannels(ParentId);
    CREATE INDEX IX_CmsChannels_Slug ON CmsChannels(Slug);
    CREATE INDEX IX_CmsChannels_IsEnabled ON CmsChannels(IsEnabled);
    CREATE INDEX IX_CmsChannels_IsDeleted ON CmsChannels(IsDeleted);
END
GO

-- =============================================
-- 2. 文章主表 (CmsArticles)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsArticles')
BEGIN
    CREATE TABLE CmsArticles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(500) NOT NULL,
        SubTitle NVARCHAR(500) NULL,
        Summary NVARCHAR(2000) NULL,
        CoverImage NVARCHAR(500) NULL,
        VideoUrl NVARCHAR(500) NULL,
        ChannelId INT NOT NULL,
        Author NVARCHAR(100) NULL,
        Source NVARCHAR(200) NULL,
        PublishTime DATETIME2 NOT NULL DEFAULT GETDATE(),
        Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
        IsTop BIT NOT NULL DEFAULT 0,
        IsRecommended BIT NOT NULL DEFAULT 0,
        IsHeadline BIT NOT NULL DEFAULT 0,
        SortOrder INT NOT NULL DEFAULT 0,
        SeoTitle NVARCHAR(200) NULL,
        SeoDescription NVARCHAR(500) NULL,
        SeoKeywords NVARCHAR(200) NULL,
        Slug NVARCHAR(200) NULL,
        ViewCount INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_CmsArticles_Channel FOREIGN KEY (ChannelId) REFERENCES CmsChannels(Id)
    );
    
    CREATE INDEX IX_CmsArticles_ChannelId ON CmsArticles(ChannelId);
    CREATE INDEX IX_CmsArticles_Status ON CmsArticles(Status);
    CREATE INDEX IX_CmsArticles_IsTop ON CmsArticles(IsTop);
    CREATE INDEX IX_CmsArticles_IsRecommended ON CmsArticles(IsRecommended);
    CREATE INDEX IX_CmsArticles_IsHeadline ON CmsArticles(IsHeadline);
    CREATE INDEX IX_CmsArticles_PublishTime ON CmsArticles(PublishTime);
    CREATE INDEX IX_CmsArticles_IsDeleted ON CmsArticles(IsDeleted);
    CREATE INDEX IX_CmsArticles_Slug ON CmsArticles(Slug);
END
GO

-- =============================================
-- 3. 文章内容表 (CmsArticleContents)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsArticleContents')
BEGIN
    CREATE TABLE CmsArticleContents (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ArticleId INT NOT NULL,
        HtmlContent NVARCHAR(MAX) NULL,
        TextContent NVARCHAR(MAX) NULL,
        ExtendJson NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_CmsArticleContents_Article FOREIGN KEY (ArticleId) REFERENCES CmsArticles(Id),
        CONSTRAINT UQ_CmsArticleContents_ArticleId UNIQUE (ArticleId)
    );
    
    CREATE INDEX IX_CmsArticleContents_ArticleId ON CmsArticleContents(ArticleId);
END
GO

-- =============================================
-- 4. 标签表 (CmsTags)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsTags')
BEGIN
    CREATE TABLE CmsTags (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Slug NVARCHAR(100) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    
    CREATE INDEX IX_CmsTags_Slug ON CmsTags(Slug);
    CREATE INDEX IX_CmsTags_IsDeleted ON CmsTags(IsDeleted);
END
GO

-- =============================================
-- 5. 文章标签关联表 (CmsArticleTags)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsArticleTags')
BEGIN
    CREATE TABLE CmsArticleTags (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ArticleId INT NOT NULL,
        TagId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_CmsArticleTags_Article FOREIGN KEY (ArticleId) REFERENCES CmsArticles(Id),
        CONSTRAINT FK_CmsArticleTags_Tag FOREIGN KEY (TagId) REFERENCES CmsTags(Id),
        CONSTRAINT UQ_CmsArticleTags_Article_Tag UNIQUE (ArticleId, TagId)
    );
    
    CREATE INDEX IX_CmsArticleTags_ArticleId ON CmsArticleTags(ArticleId);
    CREATE INDEX IX_CmsArticleTags_TagId ON CmsArticleTags(TagId);
END
GO

-- =============================================
-- 6. 媒体资源表 (CmsMediaAssets)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsMediaAssets')
BEGIN
    CREATE TABLE CmsMediaAssets (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(500) NOT NULL,
        Type NVARCHAR(100) NULL,
        Path NVARCHAR(1000) NOT NULL,
        Url NVARCHAR(1000) NOT NULL,
        Size BIGINT NOT NULL DEFAULT 0,
        GroupName NVARCHAR(100) NULL,
        Extension NVARCHAR(50) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    
    CREATE INDEX IX_CmsMediaAssets_Group ON CmsMediaAssets(GroupName);
    CREATE INDEX IX_CmsMediaAssets_IsDeleted ON CmsMediaAssets(IsDeleted);
END
GO

-- =============================================
-- 7. 推荐位表 (CmsRecommendSlots)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsRecommendSlots')
BEGIN
    CREATE TABLE CmsRecommendSlots (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Code NVARCHAR(100) NOT NULL,
        Type NVARCHAR(50) NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        IsEnabled BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT UQ_CmsRecommendSlots_Code UNIQUE (Code)
    );
    
    CREATE INDEX IX_CmsRecommendSlots_Code ON CmsRecommendSlots(Code);
    CREATE INDEX IX_CmsRecommendSlots_IsEnabled ON CmsRecommendSlots(IsEnabled);
    CREATE INDEX IX_CmsRecommendSlots_IsDeleted ON CmsRecommendSlots(IsDeleted);
END
GO

-- =============================================
-- 8. 推荐项表 (CmsRecommendItems)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsRecommendItems')
BEGIN
    CREATE TABLE CmsRecommendItems (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SlotId INT NOT NULL,
        ArticleId INT NOT NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        StartTime DATETIME2 NULL,
        EndTime DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_CmsRecommendItems_Slot FOREIGN KEY (SlotId) REFERENCES CmsRecommendSlots(Id),
        CONSTRAINT FK_CmsRecommendItems_Article FOREIGN KEY (ArticleId) REFERENCES CmsArticles(Id)
    );
    
    CREATE INDEX IX_CmsRecommendItems_SlotId ON CmsRecommendItems(SlotId);
    CREATE INDEX IX_CmsRecommendItems_ArticleId ON CmsRecommendItems(ArticleId);
    CREATE INDEX IX_CmsRecommendItems_StartTime ON CmsRecommendItems(StartTime);
    CREATE INDEX IX_CmsRecommendItems_EndTime ON CmsRecommendItems(EndTime);
END
GO

-- =============================================
-- 9. 用户表 (CmsUsers)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsUsers')
BEGIN
    CREATE TABLE CmsUsers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Email NVARCHAR(200) NULL,
        DisplayName NVARCHAR(100) NULL,
        IsEnabled BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT UQ_CmsUsers_Username UNIQUE (Username)
    );
    
    CREATE INDEX IX_CmsUsers_Username ON CmsUsers(Username);
    CREATE INDEX IX_CmsUsers_Email ON CmsUsers(Email);
    CREATE INDEX IX_CmsUsers_IsEnabled ON CmsUsers(IsEnabled);
    CREATE INDEX IX_CmsUsers_IsDeleted ON CmsUsers(IsDeleted);
END
GO

-- =============================================
-- 10. 角色表 (CmsRoles)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsRoles')
BEGIN
    CREATE TABLE CmsRoles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsEnabled BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT UQ_CmsRoles_Name UNIQUE (Name)
    );
    
    CREATE INDEX IX_CmsRoles_Name ON CmsRoles(Name);
    CREATE INDEX IX_CmsRoles_IsEnabled ON CmsRoles(IsEnabled);
    CREATE INDEX IX_CmsRoles_IsDeleted ON CmsRoles(IsDeleted);
END
GO

-- =============================================
-- 11. 权限表 (CmsPermissions)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsPermissions')
BEGIN
    CREATE TABLE CmsPermissions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Code NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT UQ_CmsPermissions_Code UNIQUE (Code)
    );
    
    CREATE INDEX IX_CmsPermissions_Code ON CmsPermissions(Code);
    CREATE INDEX IX_CmsPermissions_IsDeleted ON CmsPermissions(IsDeleted);
END
GO

-- =============================================
-- 12. 用户角色关联表 (CmsUserRoles)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsUserRoles')
BEGIN
    CREATE TABLE CmsUserRoles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        RoleId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_CmsUserRoles_User FOREIGN KEY (UserId) REFERENCES CmsUsers(Id),
        CONSTRAINT FK_CmsUserRoles_Role FOREIGN KEY (RoleId) REFERENCES CmsRoles(Id),
        CONSTRAINT UQ_CmsUserRoles_User_Role UNIQUE (UserId, RoleId)
    );
    
    CREATE INDEX IX_CmsUserRoles_UserId ON CmsUserRoles(UserId);
    CREATE INDEX IX_CmsUserRoles_RoleId ON CmsUserRoles(RoleId);
END
GO

-- =============================================
-- 13. 角色权限关联表 (CmsRolePermissions)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsRolePermissions')
BEGIN
    CREATE TABLE CmsRolePermissions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        RoleId INT NOT NULL,
        PermissionId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_CmsRolePermissions_Role FOREIGN KEY (RoleId) REFERENCES CmsRoles(Id),
        CONSTRAINT FK_CmsRolePermissions_Permission FOREIGN KEY (PermissionId) REFERENCES CmsPermissions(Id),
        CONSTRAINT UQ_CmsRolePermissions_Role_Permission UNIQUE (RoleId, PermissionId)
    );
    
    CREATE INDEX IX_CmsRolePermissions_RoleId ON CmsRolePermissions(RoleId);
    CREATE INDEX IX_CmsRolePermissions_PermissionId ON CmsRolePermissions(PermissionId);
END
GO

-- =============================================
-- 14. 操作日志表 (CmsOperationLogs)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CmsOperationLogs')
BEGIN
    CREATE TABLE CmsOperationLogs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        OperationType NVARCHAR(100) NOT NULL,
        OperationContent NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_CmsOperationLogs_User FOREIGN KEY (UserId) REFERENCES CmsUsers(Id)
    );
    
    CREATE INDEX IX_CmsOperationLogs_UserId ON CmsOperationLogs(UserId);
    CREATE INDEX IX_CmsOperationLogs_OperationType ON CmsOperationLogs(OperationType);
    CREATE INDEX IX_CmsOperationLogs_CreatedAt ON CmsOperationLogs(CreatedAt);
END
GO

-- =============================================
-- 插入初始数据
-- =============================================

-- 插入默认角色
IF NOT EXISTS (SELECT * FROM CmsRoles WHERE Name = N'超级管理员')
BEGIN
    INSERT INTO CmsRoles (Name, Description, IsEnabled) VALUES 
    (N'超级管理员', N'系统超级管理员，拥有所有权限', 1);
END

IF NOT EXISTS (SELECT * FROM CmsRoles WHERE Name = N'编辑')
BEGIN
    INSERT INTO CmsRoles (Name, Description, IsEnabled) VALUES 
    (N'编辑', N'内容编辑人员', 1);
END

IF NOT EXISTS (SELECT * FROM CmsRoles WHERE Name = N'审核人员')
BEGIN
    INSERT INTO CmsRoles (Name, Description, IsEnabled) VALUES 
    (N'审核人员', N'内容审核和发布人员', 1);
END

IF NOT EXISTS (SELECT * FROM CmsRoles WHERE Name = N'运营人员')
BEGIN
    INSERT INTO CmsRoles (Name, Description, IsEnabled) VALUES 
    (N'运营人员', N'网站运营人员', 1);
END
GO

-- 插入默认管理员账户 (密码: admin123)
-- 密码哈希: 使用SHA256加密 "admin123"
IF NOT EXISTS (SELECT * FROM CmsUsers WHERE Username = N'admin')
BEGIN
    INSERT INTO CmsUsers (Username, PasswordHash, Email, DisplayName, IsEnabled) VALUES 
    (N'admin', 'xK5bPbivJGjt0/0dRjwyy4mLEF5bVZvXzR4dPJnVnK4=', 'admin@cms.com', N'管理员', 1);
END
GO

-- 将管理员账户关联到超级管理员角色
IF NOT EXISTS (SELECT * FROM CmsUserRoles WHERE UserId = 1 AND RoleId = 1)
BEGIN
    INSERT INTO CmsUserRoles (UserId, RoleId) VALUES (1, 1);
END
GO

-- 插入默认推荐位
IF NOT EXISTS (SELECT * FROM CmsRecommendSlots WHERE Code = 'homepage_carousel')
BEGIN
    INSERT INTO CmsRecommendSlots (Name, Code, Type, SortOrder, IsEnabled) VALUES 
    (N'首页焦点轮播', 'homepage_carousel', 'Carousel', 1, 1);
END

IF NOT EXISTS (SELECT * FROM CmsRecommendSlots WHERE Code = 'homepage_headline')
BEGIN
    INSERT INTO CmsRecommendSlots (Name, Code, Type, SortOrder, IsEnabled) VALUES 
    (N'首页头条', 'homepage_headline', 'Headline', 2, 1);
END

IF NOT EXISTS (SELECT * FROM CmsRecommendSlots WHERE Code = 'homepage_recommended')
BEGIN
    INSERT INTO CmsRecommendSlots (Name, Code, Type, SortOrder, IsEnabled) VALUES 
    (N'首页推荐', 'homepage_recommended', 'Recommended', 3, 1);
END
GO

PRINT '数据库表结构创建完成！';
GO
