using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Cms.Infrastructure.Data;
using Cms.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add database context
builder.Services.AddDbContext<CmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add memory cache
builder.Services.AddMemoryCache();

// Add cache service
builder.Services.AddSingleton<Cms.Application.Services.ICacheService, Cms.Application.Services.CacheService>();
builder.Services.AddSingleton<Cms.Domain.Services.ICacheService>(sp => 
    (Cms.Domain.Services.ICacheService)sp.GetRequiredService<Cms.Application.Services.ICacheService>()
);

// Add application services
builder.Services.AddScoped<Cms.Application.Services.IHtmlSanitizerService, Cms.Application.Services.HtmlSanitizerService>();
builder.Services.AddScoped<Cms.Application.Services.IArticleService, Cms.Application.Services.ArticleService>();
builder.Services.AddScoped<Cms.Application.Services.IChannelService, Cms.Application.Services.ChannelService>();
builder.Services.AddScoped<Cms.Application.Services.IWebsiteService, Cms.Application.Services.WebsiteService>();
builder.Services.AddScoped<Cms.Application.Services.ITagService, Cms.Application.Services.TagService>();
builder.Services.AddScoped<Cms.Application.Services.ITopicService, Cms.Application.Services.TopicService>();
builder.Services.AddScoped<Cms.Application.Services.IRecommendService, Cms.Application.Services.RecommendService>();
builder.Services.AddScoped<Cms.Application.Services.ISEOService, Cms.Application.Services.SEOService>();
builder.Services.AddScoped<Cms.Application.Services.IMediaAssetService, Cms.Application.Services.MediaAssetService>();

// Add configuration as a service
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CmsDbContext>();
    // Ensure database is created
    dbContext.Database.EnsureCreated();
    
    // Seed default website if not exists
    if (!dbContext.CmsWebsites.Any())
    {
        var defaultWebsite = new CmsWebsite
        {
            Name = "默认网站",
            Domain = "localhost",
            IsEnabled = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        dbContext.CmsWebsites.Add(defaultWebsite);
        dbContext.SaveChanges();
        
        // Seed default channel
        var defaultChannel = new CmsChannel
        {
            Name = "首页",
            Slug = "home",
            IsShowInNav = true,
            IsEnabled = true,
            WebsiteId = defaultWebsite.Id,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        dbContext.CmsChannels.Add(defaultChannel);
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// Add website resolver middleware
app.UseMiddleware<Cms.Web.Middleware.WebsiteResolverMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
