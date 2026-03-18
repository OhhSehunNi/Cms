using Cms.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Cms.Web.Middleware
{
    /// <summary>
    /// 网站解析中间件
    /// 根据请求域名解析对应的网站信息，并将其存储在请求上下文中
    /// </summary>
    public class WebsiteResolverMiddleware
    {
        /// <summary>
        /// 下一个中间件委托
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next">下一个中间件委托</param>
        public WebsiteResolverMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 中间件处理方法
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="websiteService">网站服务实例</param>
        /// <returns>异步任务</returns>
        public async Task InvokeAsync(HttpContext context, IWebsiteService websiteService)
        {
            var domain = context.Request.Host.Host;
            var website = await websiteService.GetByDomainAsync(domain);

            if (website != null)
            {
                context.Items["WebsiteId"] = website.Id;
                context.Items["Website"] = website;
            }

            await _next(context);
        }
    }
}
