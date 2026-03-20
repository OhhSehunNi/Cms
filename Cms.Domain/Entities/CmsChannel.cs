using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 栏目实体类，用于管理网站栏目
    /// </summary>
    public class CmsChannel : BaseEntity
    {
        /// <summary>
        /// 栏目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 栏目 slug（URL 友好的名称）
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// 父栏目 ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 父栏目对象
        /// </summary>
        [ForeignKey("ParentId")]
        public CmsChannel Parent { get; set; }

        /// <summary>
        /// 子栏目列表
        /// </summary>
        public List<CmsChannel> Children { get; set; } = new List<CmsChannel>();

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
        /// 模板类型
        /// </summary>
        public string? TemplateType { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 网站 ID
        /// </summary>
        [Column("WebsiteId")]
        public int WebsiteId { get; set; }

        /// <summary>
        /// 网站对象
        /// </summary>
        [ForeignKey("WebsiteId")]
        public CmsWebsite Website { get; set; }

        /// <summary>
        /// 栏目下的文章列表
        /// </summary>
        public List<CmsArticle> Articles { get; set; } = new List<CmsArticle>();

        /// <summary>
        /// 创建栏目
        /// </summary>
        public void Create()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 更新栏目
        /// </summary>
        public void Update()
        {
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 删除栏目
        /// </summary>
        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 更改父栏目
        /// </summary>
        /// <param name="newParentId">新的父栏目 ID</param>
        public void ChangeParent(int? newParentId)
        {
            ParentId = newParentId;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 启用栏目
        /// </summary>
        public void Enable()
        {
            IsEnabled = true;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 停用栏目
        /// </summary>
        public void Disable()
        {
            IsEnabled = false;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 验证 Slug 是否有效
        /// </summary>
        /// <param name="slug">Slug 值</param>
        /// <returns>是否有效</returns>
        public static bool IsValidSlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return false;

            // 仅允许字母、数字、-
            return Regex.IsMatch(slug, "^[a-zA-Z0-9-]+$");
        }

        /// <summary>
        /// 检查是否存在循环引用
        /// </summary>
        /// <param name="potentialParent">潜在的父栏目</param>
        /// <returns>是否存在循环引用</returns>
        public bool CheckCircularReference(CmsChannel potentialParent)
        {
            if (potentialParent == null)
                return false;

            // 检查潜在父栏目是否是当前栏目的子栏目
            var current = potentialParent;
            while (current != null)
            {
                if (current.Id == Id)
                    return true;
                current = current.Parent;
            }

            return false;
        }

        /// <summary>
        /// 获取当前栏目的层级
        /// </summary>
        /// <returns>层级数</returns>
        public int GetLevel()
        {
            int level = 1;
            var current = Parent;
            while (current != null)
            {
                level++;
                current = current.Parent;
            }
            return level;
        }

        /// <summary>
        /// 检查是否可以添加子栏目
        /// </summary>
        /// <returns>是否可以添加子栏目</returns>
        public bool CanAddChild()
        {
            return GetLevel() < 3;
        }
    }
}