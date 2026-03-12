using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    public class ChannelService : IChannelService
    {
        private readonly CmsDbContext _dbContext;

        public ChannelService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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

        public async Task<List<ChannelDto>> GetTreeAsync()
        {
            var channels = await _dbContext.CmsChannels
                .Include(c => c.Children)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var rootChannels = channels.Where(c => c.ParentId == null).ToList();
            return rootChannels.Select(MapToDto).ToList();
        }

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

        public async Task<ChannelDto> CreateAsync(ChannelDto channelDto)
        {
            var channel = MapToEntity(channelDto);
            channel.CreatedAt = DateTime.Now;
            channel.UpdatedAt = DateTime.Now;

            _dbContext.CmsChannels.Add(channel);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(channel.Id);
        }

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

        public async Task DeleteAsync(int id)
        {
            var channel = await _dbContext.CmsChannels.FindAsync(id);
            if (channel != null)
            {
                channel.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<ChannelDto>> GetNavigationChannelsAsync()
        {
            var channels = await _dbContext.CmsChannels
                .Include(c => c.Children)
                .Where(c => c.IsShowInNav && c.IsEnabled && !c.IsDeleted)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var rootChannels = channels.Where(c => c.ParentId == null).ToList();
            return rootChannels.Select(MapToDto).ToList();
        }

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