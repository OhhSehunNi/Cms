namespace Cms.Domain.Entities
{
    /// <summary>
    /// 实体基类，包含所有实体的公共属性
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 实体主键 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否删除（软删除）
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}