using Microsoft.EntityFrameworkCore;
using Cms.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add NSwag
builder.Services.AddOpenApiDocument();

// Add database context
builder.Services.AddDbContext<Cms.Infrastructure.Data.CmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add application services
builder.Services.AddScoped<Cms.Application.Services.IArticleService, Cms.Application.Services.ArticleService>();
builder.Services.AddScoped<Cms.Application.Services.IChannelService, Cms.Application.Services.ChannelService>();
builder.Services.AddScoped<Cms.Application.Services.IMediaAssetService, Cms.Application.Services.MediaAssetService>();
builder.Services.AddScoped<Cms.Application.Services.IUserService, Cms.Application.Services.UserService>();
builder.Services.AddScoped<Cms.Application.Services.IWebsiteService, Cms.Application.Services.WebsiteService>();
builder.Services.AddScoped<Cms.Application.Services.IRoleService, Cms.Application.Services.RoleService>();
builder.Services.AddScoped<Cms.Application.Services.IPermissionService, Cms.Application.Services.PermissionService>();
builder.Services.AddScoped<Cms.Application.Services.ITagService, Cms.Application.Services.TagService>();
builder.Services.AddScoped<Cms.Application.Services.ITopicService, Cms.Application.Services.TopicService>();
builder.Services.AddScoped<Cms.Application.Services.IRecommendService, Cms.Application.Services.RecommendService>();
builder.Services.AddScoped<Cms.Application.Services.ISEOService, Cms.Application.Services.SEOService>();
builder.Services.AddScoped<Cms.Application.Services.IOperationLogService, Cms.Application.Services.OperationLogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Use NSwag
app.UseOpenApi();
app.UseSwaggerUi();

app.MapGet("/", () => new
{
    message = "CMS API",
    version = "1.0.0",
    endpoints = new
    {
        auth = new
        {
            login = "/api/auth/login",
            logout = "/api/auth/logout"
        },
        openapi = "/swagger/v1/swagger.json",
        swagger_ui = "/swagger"
    }
});

app.Run();
