using Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cms.Infrastructure.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options)
        {}

        public DbSet<CmsWebsite> CmsWebsites { get; set; }
        public DbSet<CmsChannel> CmsChannels { get; set; }
        public DbSet<CmsArticle> CmsArticles { get; set; }
        public DbSet<CmsArticleContent> CmsArticleContents { get; set; }
        public DbSet<CmsTag> CmsTags { get; set; }
        public DbSet<CmsArticleTag> CmsArticleTags { get; set; }
        public DbSet<CmsTopic> CmsTopics { get; set; }
        public DbSet<CmsTopicArticle> CmsTopicArticles { get; set; }
        public DbSet<CmsMediaAsset> CmsMediaAssets { get; set; }
        public DbSet<CmsRecommendSlot> CmsRecommendSlots { get; set; }
        public DbSet<CmsRecommendItem> CmsRecommendItems { get; set; }
        public DbSet<CmsUser> CmsUsers { get; set; }
        public DbSet<CmsRole> CmsRoles { get; set; }
        public DbSet<CmsPermission> CmsPermissions { get; set; }
        public DbSet<CmsUserRole> CmsUserRoles { get; set; }
        public DbSet<CmsRolePermission> CmsRolePermissions { get; set; }
        public DbSet<CmsOperationLog> CmsOperationLogs { get; set; }

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

            modelBuilder.Entity<CmsOperationLog>()
                .HasOne(ol => ol.User)
                .WithMany()
                .HasForeignKey(ol => ol.UserId);

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
        }
    }
}