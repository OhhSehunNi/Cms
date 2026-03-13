namespace Cms.Application.DTOs
{
    /// <summary>
    /// 推荐位数据传输对象，用于推荐位相关的请求和响应
    /// </summary>
    public class RecommendSlotDto
    {
        /// <summary>
        /// 推荐位 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 推荐位名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 推荐位代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 推荐位类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 推荐位项目列表
        /// </summary>
        public List<RecommendItemDto> RecommendItems { get; set; } = new List<RecommendItemDto>();
    }

    /// <summary>
    /// 推荐位项目数据传输对象，用于推荐位项目相关的请求和响应
    /// </summary>
    public class RecommendItemDto
    {
        /// <summary>
        /// 推荐位项目 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 推荐位 ID
        /// </summary>
        public int SlotId { get; set; }

        /// <summary>
        /// 文章 ID
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string ArticleTitle { get; set; }

        /// <summary>
        /// 文章封面图片
        /// </summary>
        public string ArticleCoverImage { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}