using Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cms.Infrastructure.Data
{
    /// <summary>
    /// 内容管理系统数据库上下文
    /// 负责管理系统中的所有实体和数据库关系
    /// </summary>
    public class CmsDbContext : DbContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">数据库上下文选项</param>
        public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options)
        {}

        /// <summary>
        /// 网站实体集合
        /// </summary>
        public DbSet<CmsWebsite> CmsWebsites { get; set; }
        
        /// <summary>
        /// 栏目实体集合
        /// </summary>
        public DbSet<CmsChannel> CmsChannels { get; set; }
        
        /// <summary>
        /// 文章实体集合
        /// </summary>
        public DbSet<CmsArticle> CmsArticles { get; set; }
        
        /// <summary>
        /// 文章内容实体集合
        /// </summary>
        public DbSet<CmsArticleContent> CmsArticleContents { get; set; }
        
        /// <summary>
        /// 标签实体集合
        /// </summary>
        public DbSet<CmsTag> CmsTags { get; set; }
        
        /// <summary>
        /// 文章标签关联实体集合
        /// </summary>
        public DbSet<CmsArticleTag> CmsArticleTags { get; set; }
        
        /// <summary>
        /// 专题实体集合
        /// </summary>
        public DbSet<CmsTopic> CmsTopics { get; set; }
        
        /// <summary>
        /// 专题文章关联实体集合
        /// </summary>
        public DbSet<CmsTopicArticle> CmsTopicArticles { get; set; }
        
        /// <summary>
        /// 媒体资源实体集合
        /// </summary>
        public DbSet<CmsMediaAsset> CmsMediaAssets { get; set; }
        
        /// <summary>
        /// 推荐位实体集合
        /// </summary>
        public DbSet<CmsRecommendSlot> CmsRecommendSlots { get; set; }
        
        /// <summary>
        /// 推荐内容实体集合
        /// </summary>
        public DbSet<CmsRecommendItem> CmsRecommendItems { get; set; }
        
        /// <summary>
        /// 用户实体集合
        /// </summary>
        public DbSet<CmsUser> CmsUsers { get; set; }
        
        /// <summary>
        /// 角色实体集合
        /// </summary>
        public DbSet<CmsRole> CmsRoles { get; set; }
        
        /// <summary>
        /// 权限实体集合
        /// </summary>
        public DbSet<CmsPermission> CmsPermissions { get; set; }
        
        /// <summary>
        /// 用户角色关联实体集合
        /// </summary>
        public DbSet<CmsUserRole> CmsUserRoles { get; set; }
        
        /// <summary>
        /// 角色权限关联实体集合
        /// </summary>
        public DbSet<CmsRolePermission> CmsRolePermissions { get; set; }
        
        /// <summary>
        /// 角色栏目关联实体集合
        /// </summary>
        public DbSet<CmsRoleChannel> CmsRoleChannels { get; set; }
        
        /// <summary>
        /// 操作日志实体集合
        /// </summary>
        public DbSet<CmsOperationLog> CmsOperationLogs { get; set; }
        
        /// <summary>
        /// 登录日志实体集合
        /// </summary>
        public DbSet<CmsLoginLog> CmsLoginLogs { get; set; }
        
        /// <summary>
        /// SEO重定向实体集合
        /// </summary>
        public DbSet<CmsSeoRedirect> CmsSeoRedirects { get; set; }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置实体关系
            modelBuilder.Entity<CmsChannel>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId);

            modelBuilder.Entity<CmsChannel>()
                .HasOne(c => c.Website)
                .WithMany(w => w.Channels)
                .HasForeignKey(c => c.WebsiteId);

            modelBuilder.Entity<CmsArticle>()
                .HasOne(a => a.Channel)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.ChannelId);

            modelBuilder.Entity<CmsArticle>()
                .HasOne(a => a.Website)
                .WithMany(w => w.Articles)
                .HasForeignKey(a => a.WebsiteId);

            modelBuilder.Entity<CmsArticleContent>()
                .HasOne(ac => ac.Article)
                .WithOne(a => a.Content)
                .HasForeignKey<CmsArticleContent>(ac => ac.ArticleId);

            modelBuilder.Entity<CmsArticleTag>()
                .HasOne(at => at.Article)
                .WithMany(a => a.ArticleTags)
                .HasForeignKey(at => at.ArticleId);

            modelBuilder.Entity<CmsArticleTag>()
                .HasOne(at => at.Tag)
                .WithMany(t => t.ArticleTags)
                .HasForeignKey(at => at.TagId);

            modelBuilder.Entity<CmsTag>()
                .HasOne(t => t.Website)
                .WithMany(w => w.Tags)
                .HasForeignKey(t => t.WebsiteId);

            modelBuilder.Entity<CmsMediaAsset>()
                .HasOne(ma => ma.Website)
                .WithMany(w => w.MediaAssets)
                .HasForeignKey(ma => ma.WebsiteId);

            modelBuilder.Entity<CmsRecommendSlot>()
                .HasOne(rs => rs.Website)
                .WithMany(w => w.RecommendSlots)
                .HasForeignKey(rs => rs.WebsiteId);

            modelBuilder.Entity<CmsRecommendItem>()
                .HasOne(ri => ri.Slot)
                .WithMany(s => s.RecommendItems)
                .HasForeignKey(ri => ri.SlotId);

            modelBuilder.Entity<CmsRecommendItem>()
                .HasOne(ri => ri.Article)
                .WithMany()
                .HasForeignKey(ri => ri.ArticleId);

            modelBuilder.Entity<CmsUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<CmsUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<CmsRolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<CmsRolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder.Entity<CmsRoleChannel>()
                .HasOne(rc => rc.Role)
                .WithMany(r => r.RoleChannels)
                .HasForeignKey(rc => rc.RoleId);

            modelBuilder.Entity<CmsRoleChannel>()
                .HasOne(rc => rc.Channel)
                .WithMany()
                .HasForeignKey(rc => rc.ChannelId);

            modelBuilder.Entity<CmsOperationLog>()
                .HasOne(ol => ol.User)
                .WithMany()
                .HasForeignKey(ol => ol.UserId);

            modelBuilder.Entity<CmsLoginLog>()
                .HasOne(ll => ll.User)
                .WithMany()
                .HasForeignKey(ll => ll.UserId);

            modelBuilder.Entity<CmsTopic>()
                .HasOne(t => t.Website)
                .WithMany(w => w.Topics)
                .HasForeignKey(t => t.WebsiteId);

            modelBuilder.Entity<CmsTopicArticle>()
                .HasOne(ta => ta.Topic)
                .WithMany(t => t.TopicArticles)
                .HasForeignKey(ta => ta.TopicId);

            modelBuilder.Entity<CmsTopicArticle>()
                .HasOne(ta => ta.Article)
                .WithMany()
                .HasForeignKey(ta => ta.ArticleId);

            modelBuilder.Entity<CmsSeoRedirect>()
                .HasOne(sr => sr.Website)
                .WithMany(w => w.SeoRedirects)
                .HasForeignKey(sr => sr.WebsiteId);
        }
    }
}