namespace Cms.Domain.Entities
{
    /// <summary>
    /// 文章标签关联实体类，用于管理文章和标签的多对多关系
    /// </summary>
    public class CmsArticleTag : BaseEntity
    {
        /// <summary>
        /// 文章 ID
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// 文章对象
        /// </summary>
        public CmsArticle Article { get; set; }

        /// <summary>
        /// 标签 ID
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// 标签对象
        /// </summary>
        public CmsTag Tag { get; set; }
    }
}