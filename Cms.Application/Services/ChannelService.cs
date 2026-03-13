using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    /// <summary>
    /// 栏目服务实现类，用于栏目相关的业务逻辑
    /// </summary>
    public class ChannelService : IChannelService
    {
        private readonly CmsDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public ChannelService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 根据 ID 获取栏目
        /// </summary>
        /// <param name="id">栏目 ID</param>
        /// <returns>栏目 DTO</returns>
        public async Task<ChannelDto> GetByIdAsync(int id)
        {
            var channel = await _dbContext.CmsChannels
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (channel == null)
                return null;

            return MapToDto(channel);
        }

        /// <summary>
        /// 获取栏目树
        /// </summary>
        /// <returns>栏目 DTO 列表</returns>
        public async Task<List<ChannelDto>> GetTreeAsync()
        {
            var channels = await _dbContext.CmsChannels
                .Include(c => c.Children)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var rootChannels = channels.Where(c => c.ParentId == null).ToList();
            return rootChannels.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 获取栏目列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>栏目 DTO 列表</returns>
        public async Task<List<ChannelDto>> GetListAsync(int page, int pageSize, string keyword = null)
        {
            IQueryable<CmsChannel> query = _dbContext.CmsChannels
                .Include(c => c.Parent);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Name.Contains(keyword) || c.Slug.Contains(keyword));
            }

            var channels = await query
                .OrderBy(c => c.SortOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return channels.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 创建栏目
        /// </summary>
        /// <param name="channelDto">栏目 DTO</param>
        /// <returns>创建后的栏目 DTO</returns>
        public async Task<ChannelDto> CreateAsync(ChannelDto channelDto)
        {
            var channel = MapToEntity(channelDto);
            channel.CreatedAt = DateTime.Now;
            channel.UpdatedAt = DateTime.Now;

            _dbContext.CmsChannels.Add(channel);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(channel.Id);
        }

        /// <summary>
        /// 更新栏目
        /// </summary>
        /// <param name="channelDto">栏目 DTO</param>
        /// <returns>更新后的栏目 DTO</returns>
        public async Task<ChannelDto> UpdateAsync(ChannelDto channelDto)
        {
            var channel = await _dbContext.CmsChannels.FindAsync(channelDto.Id);
            if (channel == null)
                throw new Exception("Channel not found");

            UpdateEntityFromDto(channel, channelDto);
            channel.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(channel.Id);
        }

        /// <summary>
        /// 删除栏目
        /// </summary>
        /// <param name="id">栏目 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var channel = await _dbContext.CmsChannels.FindAsync(id);
            if (channel != null)
            {
                channel.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取导航栏目
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>栏目 DTO 列表</returns>
        public async Task<List<ChannelDto>> GetNavigationChannelsAsync(int websiteId)
        {
            var channels = await _dbContext.CmsChannels
                .Include(c => c.Children)
                .Where(c => c.WebsiteId == websiteId && c.IsShowInNav && c.IsEnabled && !c.IsDeleted)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var rootChannels = channels.Where(c => c.ParentId == null).ToList();
            return rootChannels.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="channel">栏目实体</param>
        /// <returns>栏目 DTO</returns>
        private ChannelDto MapToDto(CmsChannel channel)
        {
            return new ChannelDto
            {
                Id = channel.Id,
                Name = channel.Name,
                Slug = channel.Slug,
                ParentId = channel.ParentId,
                ParentName = channel.Parent?.Name,
                SortOrder = channel.SortOrder,
                IsShowInNav = channel.IsShowInNav,
                SeoTitle = channel.SeoTitle,
                SeoDescription = channel.SeoDescription,
                SeoKeywords = channel.SeoKeywords,
                TemplateType = channel.TemplateType,
                IsEnabled = channel.IsEnabled,
                Children = channel.Children.Select(MapToDto).ToList()
            };
        }

        /// <summary>
        /// 将 DTO 映射为实体
        /// </summary>
        /// <param name="channelDto">栏目 DTO</param>
        /// <returns>栏目实体</returns>
        private CmsChannel MapToEntity(ChannelDto channelDto)
        {
            return new CmsChannel
            {
                Name = channelDto.Name,
                Slug = channelDto.Slug,
                ParentId = channelDto.ParentId,
                SortOrder = channelDto.SortOrder,
                IsShowInNav = channelDto.IsShowInNav,
                SeoTitle = channelDto.SeoTitle,
                SeoDescription = channelDto.SeoDescription,
                SeoKeywords = channelDto.SeoKeywords,
                TemplateType = channelDto.TemplateType,
                IsEnabled = channelDto.IsEnabled
            };
        }

        /// <summary>
        /// 从 DTO 更新实体
        /// </summary>
        /// <param name="channel">栏目实体</param>
        /// <param name="channelDto">栏目 DTO</param>
        private void UpdateEntityFromDto(CmsChannel channel, ChannelDto channelDto)
        {
            channel.Name = channelDto.Name;
            channel.Slug = channelDto.Slug;
            channel.ParentId = channelDto.ParentId;
            channel.SortOrder = channelDto.SortOrder;
            channel.IsShowInNav = channelDto.IsShowInNav;
            channel.SeoTitle = channelDto.SeoTitle;
            channel.SeoDescription = channelDto.SeoDescription;
            channel.SeoKeywords = channelDto.SeoKeywords;
            channel.TemplateType = channelDto.TemplateType;
            channel.IsEnabled = channelDto.IsEnabled;
        }
    }
}