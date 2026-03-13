using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    public class LayoutViewModel
    {
        public string WebsiteName { get; set; }
        public string WebsiteLogo { get; set; }
        public string FooterInfo { get; set; }
        public List<NavigationItemViewModel> NavigationItems { get; set; }
    }

    public class NavigationItemViewModel
    {
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string ChannelSlug { get; set; }
        public List<NavigationItemViewModel> Children { get; set; }
    }
}