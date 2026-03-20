namespace Cms.Domain.Entities
{
    /// <summary>
    /// 网站实体类，用于管理多站点
    /// </summary>
    public class CmsWebsite : BaseEntity
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 网站域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 网站 logo
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// SEO 标题
        /// </summary>
        public string? SeoTitle { get; set; }

        /// <summary>
        /// SEO 描述
        /// </summary>
        public string? SeoDescription { get; set; }

        /// <summary>
        /// SEO 关键词
        /// </summary>
        public string? SeoKeywords { get; set; }

        /// <summary>
        /// 网站底部信息
        /// </summary>
        public string? FooterInfo { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 网站下的栏目列表
        /// </summary>
        public List<CmsChannel> Channels { get; set; } = new List<CmsChannel>();

        /// <summary>
        /// 网站下的文章列表
        /// </summary>
        public List<CmsArticle> Articles { get; set; } = new List<CmsArticle>();

        /// <summary>
        /// 网站下的标签列表
        /// </summary>
        public List<CmsTag> Tags { get; set; } = new List<CmsTag>();

        /// <summary>
        /// 网站下的专题列表
        /// </summary>
        public List<CmsTopic> Topics { get; set; } = new List<CmsTopic>();

        /// <summary>
        /// 网站下的媒体资源列表
        /// </summary>
        public List<CmsMediaAsset> MediaAssets { get; set; } = new List<CmsMediaAsset>();

        /// <summary>
        /// 网站下的推荐位列表
        /// </summary>
        public List<CmsRecommendSlot> RecommendSlots { get; set; } = new List<CmsRecommendSlot>();

        /// <summary>
        /// 网站下的SEO重定向规则列表
        /// </summary>
        public List<CmsSeoRedirect> SeoRedirects { get; set; } = new List<CmsSeoRedirect>();
    }
}
