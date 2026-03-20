using Cms.Application.Services.Dtos;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;

namespace Cms.Application.Services
{
    /// <summary>
    /// 文章 Dapper 服务，用于高性能查询
    /// </summary>
    public class ArticleDapperService
    {
        private readonly string _connectionString;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">配置对象</param>
        public ArticleDapperService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("连接字符串未配置");
        }

        /// <summary>
        /// 获取文章列表（Dapper 实现）
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="channelId">栏目 ID</param>
        /// <param name="status">状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isTop">是否置顶</param>
        /// <param name="isRecommended">是否推荐</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>文章 DTO 列表</returns>
        public async Task<List<ArticleDto>> GetListAsync(int page, int pageSize, string? keyword = null, int? channelId = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null, bool? isTop = null, bool? isRecommended = null, int websiteId = 1)
        {
            // 使用配置的连接字符串创建连接
            using (var connection = Microsoft.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                
                // 确保连接是打开的
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var sql = @"
                    SELECT 
                        a.Id, a.Title, a.SubTitle, a.Summary, a.CoverImage, a.VideoUrl, 
                        a.ChannelId, c.Name as ChannelName, c.Slug as ChannelSlug, 
                        a.Author, a.Source, a.PublishTime, a.Status, 
                        a.IsTop, a.IsRecommended, a.IsHeadline, a.SortOrder, 
                        a.SeoTitle, a.SeoDescription, a.SeoKeywords, a.Slug, a.ViewCount,
                        a.CreatedAt, a.UpdatedAt, a.IsDeleted
                    FROM CmsArticles a
                    LEFT JOIN CmsChannels c ON a.ChannelId = c.Id
                    WHERE a.WebsiteId = @WebsiteId AND a.IsDeleted = 0
                ";

                var parameters = new DynamicParameters();
                parameters.Add("WebsiteId", websiteId);

                // 添加筛选条件
                if (!string.IsNullOrEmpty(keyword))
                {
                    sql += " AND (a.Title LIKE @Keyword OR a.Summary LIKE @Keyword)";
                    parameters.Add("Keyword", $"%{keyword}%");
                }

                if (channelId.HasValue)
                {
                    sql += " AND a.ChannelId = @ChannelId";
                    parameters.Add("ChannelId", channelId.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    sql += " AND a.Status = @Status";
                    parameters.Add("Status", status);
                }

                if (startDate.HasValue)
                {
                    sql += " AND a.PublishTime >= @StartDate";
                    parameters.Add("StartDate", startDate.Value);
                }

                if (endDate.HasValue)
                {
                    sql += " AND a.PublishTime <= @EndDate";
                    parameters.Add("EndDate", endDate.Value);
                }

                if (isTop.HasValue)
                {
                    sql += " AND a.IsTop = @IsTop";
                    parameters.Add("IsTop", isTop.Value);
                }

                if (isRecommended.HasValue)
                {
                    sql += " AND a.IsRecommended = @IsRecommended";
                    parameters.Add("IsRecommended", isRecommended.Value);
                }

                // 添加排序
                sql += " ORDER BY a.IsTop DESC, a.PublishTime DESC";

                // 添加分页
                sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                parameters.Add("Offset", (page - 1) * pageSize);
                parameters.Add("PageSize", pageSize);

                var articles = await connection.QueryAsync<ArticleDto, string, string, ArticleDto>(
                    sql,
                    (article, channelName, channelSlug) =>
                    {
                        article.ChannelName = channelName;
                        article.ChannelSlug = channelSlug;
                        return article;
                    },
                    parameters,
                    splitOn: "ChannelName,ChannelSlug"
                );

                return articles.ToList();
            }
        }

        /// <summary>
        /// 获取文章总数（用于分页）
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="channelId">栏目 ID</param>
        /// <param name="status">状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isTop">是否置顶</param>
        /// <param name="isRecommended">是否推荐</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>文章总数</returns>
        public async Task<int> GetCountAsync(string? keyword = null, int? channelId = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null, bool? isTop = null, bool? isRecommended = null, int websiteId = 1)
        {
            // 使用配置的连接字符串创建连接
            using (var connection = Microsoft.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                
                // 确保连接是打开的
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var sql = @"
                    SELECT COUNT(*)
                    FROM CmsArticles a
                    WHERE a.WebsiteId = @WebsiteId AND a.IsDeleted = 0
                ";

                var parameters = new DynamicParameters();
                parameters.Add("WebsiteId", websiteId);

                // 添加筛选条件
                if (!string.IsNullOrEmpty(keyword))
                {
                    sql += " AND (a.Title LIKE @Keyword OR a.Summary LIKE @Keyword)";
                    parameters.Add("Keyword", $"%{keyword}%");
                }

                if (channelId.HasValue)
                {
                    sql += " AND a.ChannelId = @ChannelId";
                    parameters.Add("ChannelId", channelId.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    sql += " AND a.Status = @Status";
                    parameters.Add("Status", status);
                }

                if (startDate.HasValue)
                {
                    sql += " AND a.PublishTime >= @StartDate";
                    parameters.Add("StartDate", startDate.Value);
                }

                if (endDate.HasValue)
                {
                    sql += " AND a.PublishTime <= @EndDate";
                    parameters.Add("EndDate", endDate.Value);
                }

                if (isTop.HasValue)
                {
                    sql += " AND a.IsTop = @IsTop";
                    parameters.Add("IsTop", isTop.Value);
                }

                if (isRecommended.HasValue)
                {
                    sql += " AND a.IsRecommended = @IsRecommended";
                    parameters.Add("IsRecommended", isRecommended.Value);
                }

                return await connection.QueryFirstAsync<int>(sql, parameters);
            }
        }
    }
}
