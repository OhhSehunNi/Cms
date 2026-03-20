using Cms.Application.Services;
using Cms.Infrastructure.Data;
using Cms.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAuthorization();

// Add NSwag
builder.Services.AddOpenApiDocument(config =>
{
    config.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Name = "Authorization",
        Description = "输入JWT Token进行认证"
    });
    // 为所有操作自动添加安全需求（Swagger UI 会在请求头注入 Authorization）
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
});

// Add Memory Cache
builder.Services.AddMemoryCache();

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
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoginLogService, LoginLogService>();
builder.Services.AddScoped<IRoleChannelService, RoleChannelService>();
builder.Services.AddScoped<IHtmlSanitizerService, HtmlSanitizerService>();

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    //options.RequireHttpsMetadata = true;
    //options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "your-secret-key-here-change-in-production"))
    };
    
    // 添加JWT事件处理
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            //Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
           // Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            // 从请求头中获取token
            var authHeader = context.Request.Headers.Authorization.ToString();
            //Console.WriteLine($"Original authorization header: {authHeader}");
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring(7).Trim();
                Console.WriteLine($"Original token: {token}");
                // 移除可能的大括号
                if (token.StartsWith("{") && token.EndsWith("}"))
                {
                    token = token.Substring(1, token.Length - 2).Trim();
                    Console.WriteLine($"Updated token: {token}");
                    // 同时更新请求头
                    context.Request.Headers.Authorization = "Bearer " + token;
                    Console.WriteLine($"Updated authorization header: Bearer {token}");
                }
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

// Add filters
builder.Services.AddScoped<JwtAuthenticationFilter>();
builder.Services.AddMvc(options =>
{
    options.Filters.AddService<JwtAuthenticationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
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
