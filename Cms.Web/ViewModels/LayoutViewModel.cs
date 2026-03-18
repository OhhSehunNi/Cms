using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    /// <summary>
    /// 布局视图模型
    /// 用于网站布局的展示
    /// </summary>
    public class LayoutViewModel
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public string WebsiteName { get; set; }
        /// <summary>
        /// 网站Logo
        /// </summary>
        public string WebsiteLogo { get; set; }
        /// <summary>
        /// 页脚信息
        /// </summary>
        public string FooterInfo { get; set; }
        /// <summary>
        /// 导航菜单项
        /// </summary>
        public List<NavigationItemViewModel> NavigationItems { get; set; }
    }

    /// <summary>
    /// 导航菜单项视图模型
    /// 用于导航菜单的展示
    /// </summary>
    public class NavigationItemViewModel
    {
        /// <summary>
        /// 频道ID
        /// </summary>
        public int ChannelId { get; set; }
        /// <summary>
        /// 频道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 频道Slug
        /// </summary>
        public string ChannelSlug { get; set; }
        /// <summary>
        /// 子菜单项
        /// </summary>
        public List<NavigationItemViewModel> Children { get; set; }
    }
}