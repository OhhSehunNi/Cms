using Cms.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Cms.Web.Middleware
{
    public class WebsiteResolverMiddleware
    {
        private readonly RequestDelegate _next;

        public WebsiteResolverMiddleware(RequestDelegate next)
        {
            _next = next;
        }

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
