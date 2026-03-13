namespace Cms.Application.DTOs
{
    /// <summary>
    /// 栏目数据传输对象，用于栏目相关的请求和响应
    /// </summary>
    public class ChannelDto
    {
        /// <summary>
        /// 栏目 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 栏目 slug（URL 友好的名称）
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 父栏目 ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 父栏目名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否在导航中显示
        /// </summary>
        public bool IsShowInNav { get; set; }

        /// <summary>
        /// SEO 标题
        /// </summary>
        public string SeoTitle { get; set; }

        /// <summary>
        /// SEO 描述
        /// </summary>
        public string SeoDescription { get; set; }

        /// <summary>
        /// SEO 关键词
        /// </summary>
        public string SeoKeywords { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public string TemplateType { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 子栏目列表
        /// </summary>
        public List<ChannelDto> Children { get; set; } = new List<ChannelDto>();
    }
}