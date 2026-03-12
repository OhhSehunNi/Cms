using Microsoft.EntityFrameworkCore;
using Cms.Infrastructure.Data; // ʹ CmsDbContext ���Ϳɼ�

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add database context
builder.Services.AddDbContext<Cms.Infrastructure.Data.CmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add cache service
builder.Services.AddSingleton<Cms.Infrastructure.Services.ICacheService>(new Cms.Infrastructure.Services.RedisCacheService(builder.Configuration.GetConnectionString("Redis")));

// Add application services
builder.Services.AddScoped<Cms.Application.Services.IArticleService, Cms.Application.Services.ArticleService>();

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
