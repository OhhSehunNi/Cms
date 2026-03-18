using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;

namespace Cms.Web.ViewComponents
{
    /// <summary>
    /// 导航视图组件
    /// 用于显示网站导航菜单
    /// </summary>
    public class NavigationViewComponent : ViewComponent
    {
        /// <summary>
        /// 频道服务接口
        /// </summary>
        private readonly IChannelService _channelService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="channelService">频道服务实例</param>
        public NavigationViewComponent(IChannelService channelService)
        {
            _channelService = channelService;
        }

        /// <summary>
        /// 调用视图组件
        /// </summary>
        /// <returns>视图组件结果</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取导航菜单数据（这里需要根据实际实现调整）
            var navigationItems = await GetNavigationItems(websiteId);
            
            return View(navigationItems);
        }

        /// <summary>
        /// 获取导航菜单项
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>导航菜单项列表</returns>
        private async Task<List<NavigationItemViewModel>> GetNavigationItems(int websiteId)
        {
            // 实际实现中，应该调用服务获取栏目树
            // 这里为了演示，返回模拟数据
            return new List<NavigationItemViewModel>
            {
                new NavigationItemViewModel
                {
                    ChannelId = 1,
                    ChannelName = "首页",
                    ChannelSlug = "",
                    Children = new List<NavigationItemViewModel>()
                },
                new NavigationItemViewModel
                {
                    ChannelId = 2,
                    ChannelName = "新闻",
                    ChannelSlug = "news",
                    Children = new List<NavigationItemViewModel>
                    {
                        new NavigationItemViewModel
                        {
                            ChannelId = 3,
                            ChannelName = "国内新闻",
                            ChannelSlug = "news/domestic",
                            Children = new List<NavigationItemViewModel>()
                        },
                        new NavigationItemViewModel
                        {
                            ChannelId = 4,
                            ChannelName = "国际新闻",
                            ChannelSlug = "news/international",
                            Children = new List<NavigationItemViewModel>()
                        }
                    }
                },
                new NavigationItemViewModel
                {
                    ChannelId = 5,
                    ChannelName = "科技",
                    ChannelSlug = "tech",
                    Children = new List<NavigationItemViewModel>()
                },
                new NavigationItemViewModel
                {
                    ChannelId = 6,
                    ChannelName = "娱乐",
                    ChannelSlug = "entertainment",
                    Children = new List<NavigationItemViewModel>()
                }
            };
        }
    }
}