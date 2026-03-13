using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    /// <summary>
    /// 网站服务实现类，用于网站相关的业务逻辑
    /// </summary>
    public class WebsiteService : IWebsiteService
    {
        private readonly CmsDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public WebsiteService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 根据 ID 获取网站
        /// </summary>
        /// <param name="id">网站 ID</param>
        /// <returns>网站 DTO</returns>
        public async Task<WebsiteDto> GetByIdAsync(int id)
        {
            var website = await _dbContext.CmsWebsites.FirstOrDefaultAsync(w => w.Id == id);
            if (website == null)
                return null;

            return MapToDto(website);
        }

        /// <summary>
        /// 根据域名获取网站
        /// </summary>
        /// <param name="domain">网站域名</param>
        /// <returns>网站 DTO</returns>
        public async Task<WebsiteDto> GetByDomainAsync(string domain)
        {
            var website = await _dbContext.CmsWebsites.FirstOrDefaultAsync(w => w.Domain == domain);
            if (website == null)
                return null;

            return MapToDto(website);
        }

        /// <summary>
        /// 获取网站列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>网站 DTO 列表</returns>
        public async Task<List<WebsiteDto>> GetListAsync(int page, int pageSize, string? keyword = null)
        {
            IQueryable<CmsWebsite> query = _dbContext.CmsWebsites;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(w => w.Name.Contains(keyword) || w.Domain.Contains(keyword));
            }

            var websites = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return websites.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 创建网站
        /// </summary>
        /// <param name="websiteDto">网站 DTO</param>
        /// <returns>创建后的网站 DTO</returns>
        public async Task<WebsiteDto> CreateAsync(WebsiteDto websiteDto)
        {
            var website = new CmsWebsite
            {
                Name = websiteDto.Name,
                Domain = websiteDto.Domain,
                Logo = websiteDto.Logo,
                SeoTitle = websiteDto.SeoTitle,
                SeoDescription = websiteDto.SeoDescription,
                SeoKeywords = websiteDto.SeoKeywords,
                FooterInfo = websiteDto.FooterInfo,
                IsEnabled = websiteDto.IsEnabled,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.CmsWebsites.Add(website);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(website.Id);
        }

        /// <summary>
        /// 更新网站
        /// </summary>
        /// <param name="websiteDto">网站 DTO</param>
        /// <returns>更新后的网站 DTO</returns>
        public async Task<WebsiteDto> UpdateAsync(WebsiteDto websiteDto)
        {
            var website = await _dbContext.CmsWebsites.FirstOrDefaultAsync(w => w.Id == websiteDto.Id);
            if (website == null)
                return null;

            website.Name = websiteDto.Name;
            website.Domain = websiteDto.Domain;
            website.Logo = websiteDto.Logo;
            website.SeoTitle = websiteDto.SeoTitle;
            website.SeoDescription = websiteDto.SeoDescription;
            website.SeoKeywords = websiteDto.SeoKeywords;
            website.FooterInfo = websiteDto.FooterInfo;
            website.IsEnabled = websiteDto.IsEnabled;
            website.UpdatedAt = DateTime.Now;

            _dbContext.CmsWebsites.Update(website);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(website.Id);
        }

        /// <summary>
        /// 删除网站
        /// </summary>
        /// <param name="id">网站 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var website = await _dbContext.CmsWebsites.FirstOrDefaultAsync(w => w.Id == id);
            if (website != null)
            {
                _dbContext.CmsWebsites.Remove(website);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="website">网站实体</param>
        /// <returns>网站 DTO</returns>
        private WebsiteDto MapToDto(CmsWebsite website)
        {
            return new WebsiteDto
            {
                Id = website.Id,
                Name = website.Name,
                Domain = website.Domain,
                Logo = website.Logo,
                SeoTitle = website.SeoTitle,
                SeoDescription = website.SeoDescription,
                SeoKeywords = website.SeoKeywords,
                FooterInfo = website.FooterInfo,
                IsEnabled = website.IsEnabled,
                CreatedAt = website.CreatedAt,
                UpdatedAt = website.UpdatedAt
            };
        }
    }
}
