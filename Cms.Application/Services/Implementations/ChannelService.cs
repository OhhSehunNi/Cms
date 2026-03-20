using Cms.Application.Services.Dtos;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Cms.Application.Services
{
    /// <summary>
    /// 栏目服务实现类，用于栏目相关的业务逻辑
    /// </summary>
    public class ChannelService : IChannelService
    {
        private readonly CmsDbContext _dbContext;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="cacheService">缓存服务</param>
        public ChannelService(CmsDbContext dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
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
        /// <param name="websiteId">网站 ID</param>
        /// <returns>栏目 DTO 列表</returns>
        public async Task<List<ChannelDto>> GetTreeAsync(int websiteId)
        {
            string cacheKey = $"channel:tree:{websiteId}";
            var cachedTree = _cacheService.Get<List<ChannelDto>>(cacheKey);
            if (cachedTree != null)
                return cachedTree;

            var channels = await _dbContext.CmsChannels
                .Include(c => c.Children)
                .Where(c => c.WebsiteId == websiteId && !c.IsDeleted)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var rootChannels = channels.Where(c => c.ParentId == null).ToList();
            var tree = rootChannels.Select(MapToDto).ToList();

            _cacheService.Set(cacheKey, tree, TimeSpan.FromHours(1));
            return tree;
        }

        /// <summary>
        /// 获取栏目列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>栏目 DTO 列表</returns>
        public async Task<List<ChannelDto>> GetListAsync(int page, int pageSize, string keyword = null, int websiteId = 1)
        {
            IQueryable<CmsChannel> query = _dbContext.CmsChannels
                .Include(c => c.Parent)
                .Where(c => c.WebsiteId == websiteId && !c.IsDeleted);

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
            // 验证 Slug
            if (!string.IsNullOrEmpty(channelDto.Slug) && !CmsChannel.IsValidSlug(channelDto.Slug))
                throw new Exception("Slug 只能包含字母、数字和-");

            // 检查 Slug 唯一性
            if (!string.IsNullOrEmpty(channelDto.Slug))
            {
                var existingChannel = await _dbContext.CmsChannels
                    .Where(c => c.Slug == channelDto.Slug && c.WebsiteId == channelDto.WebsiteId && !c.IsDeleted)
                    .FirstOrDefaultAsync();
                if (existingChannel != null)
                    throw new Exception("Slug 已存在");
            }

            // 检查父栏目
            if (channelDto.ParentId.HasValue)
            {
                var parentChannel = await _dbContext.CmsChannels.FindAsync(channelDto.ParentId);
                if (parentChannel == null)
                    throw new Exception("父栏目不存在");

                // 检查层级
                if (parentChannel.GetLevel() >= 3)
                    throw new Exception("超过最大层级限制");
            }

            var channel = MapToEntity(channelDto);
            channel.Create();

            _dbContext.CmsChannels.Add(channel);
            await _dbContext.SaveChangesAsync();

            // 清除缓存
            ClearChannelCache(channel.WebsiteId);

            return await GetByIdAsync(channel.Id);
        }

        /// <summary>
        /// 更新栏目
        /// </summary>
        /// <param name="channelDto">栏目 DTO</param>
        /// <returns>更新后的栏目 DTO</returns>
        public async Task<ChannelDto> UpdateAsync(ChannelDto channelDto)
        {
            var channel = await _dbContext.CmsChannels
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Id == channelDto.Id);
            if (channel == null)
                throw new Exception("栏目不存在");

            // 验证 Slug
            if (!string.IsNullOrEmpty(channelDto.Slug) && !CmsChannel.IsValidSlug(channelDto.Slug))
                throw new Exception("Slug 只能包含字母、数字和-");

            // 检查 Slug 唯一性
            if (!string.IsNullOrEmpty(channelDto.Slug) && channelDto.Slug != channel.Slug)
            {
                var existingChannel = await _dbContext.CmsChannels
                    .Where(c => c.Slug == channelDto.Slug && c.WebsiteId == channelDto.WebsiteId && c.Id != channelDto.Id && !c.IsDeleted)
                    .FirstOrDefaultAsync();
                if (existingChannel != null)
                    throw new Exception("Slug 已存在");
            }

            // 检查父栏目
            if (channelDto.ParentId.HasValue)
            {
                var parentChannel = await _dbContext.CmsChannels.FindAsync(channelDto.ParentId);
                if (parentChannel == null)
                    throw new Exception("父栏目不存在");

                // 检查循环引用
                if (channel.CheckCircularReference(parentChannel))
                    throw new Exception("不能设置子栏目为父栏目");

                // 检查层级
                int newLevel = 1;
                var current = parentChannel;
                while (current != null)
                {
                    newLevel++;
                    current = current.Parent;
                }
                if (newLevel > 3)
                    throw new Exception("超过最大层级限制");
            }

            UpdateEntityFromDto(channel, channelDto);
            channel.Update();

            await _dbContext.SaveChangesAsync();

            // 清除缓存
            ClearChannelCache(channel.WebsiteId);

            return await GetByIdAsync(channel.Id);
        }

        /// <summary>
        /// 删除栏目
        /// </summary>
        /// <param name="id">栏目 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var channel = await _dbContext.CmsChannels
                .Include(c => c.Children)
                .Include(c => c.Articles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (channel == null)
                throw new Exception("栏目不存在");

            // 检查是否有子栏目
            if (channel.Children.Any())
                throw new Exception("存在子栏目，无法删除");

            // 检查是否有文章
            if (channel.Articles.Any())
                throw new Exception("存在文章，无法删除");

            channel.Delete();
            await _dbContext.SaveChangesAsync();

            // 清除缓存
            ClearChannelCache(channel.WebsiteId);
        }

        /// <summary>
        /// 获取导航栏目
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>栏目 DTO 列表</returns>
        public async Task<List<ChannelDto>> GetNavigationChannelsAsync(int websiteId)
        {
            string cacheKey = $"channel:navigation:{websiteId}";
            var cachedNavigation = _cacheService.Get<List<ChannelDto>>(cacheKey);
            if (cachedNavigation != null)
                return cachedNavigation;

            var channels = await _dbContext.CmsChannels
                .Include(c => c.Children)
                .Where(c => c.WebsiteId == websiteId && c.IsShowInNav && c.IsEnabled && !c.IsDeleted)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var rootChannels = channels.Where(c => c.ParentId == null).ToList();
            var navigation = rootChannels.Select(MapToDto).ToList();

            _cacheService.Set(cacheKey, navigation, TimeSpan.FromHours(1));
            return navigation;
        }

        /// <summary>
        /// 修改排序
        /// </summary>
        /// <param name="sortRequests">排序请求</param>
        /// <returns></returns>
        public async Task UpdateSortAsync(List<SortRequestDto> sortRequests)
        {
            foreach (var request in sortRequests)
            {
                var channel = await _dbContext.CmsChannels.FindAsync(request.Id);
                if (channel != null)
                {
                    channel.SortOrder = request.SortOrder;
                    channel.Update();
                }
            }

            await _dbContext.SaveChangesAsync();

            // 清除缓存
            if (sortRequests.Any())
            {
                var firstChannel = await _dbContext.CmsChannels.FindAsync(sortRequests[0].Id);
                if (firstChannel != null)
                {
                    ClearChannelCache(firstChannel.WebsiteId);
                }
            }
        }

        /// <summary>
        /// 启用/停用栏目
        /// </summary>
        /// <param name="id">栏目 ID</param>
        /// <returns>更新后的栏目 DTO</returns>
        public async Task<ChannelDto> ToggleStatusAsync(int id)
        {
            var channel = await _dbContext.CmsChannels.FindAsync(id);
            if (channel == null)
                throw new Exception("栏目不存在");

            if (channel.IsEnabled)
                channel.Disable();
            else
                channel.Enable();

            await _dbContext.SaveChangesAsync();

            // 清除缓存
            ClearChannelCache(channel.WebsiteId);

            return await GetByIdAsync(channel.Id);
        }

        /// <summary>
        /// 清除栏目缓存
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        private void ClearChannelCache(int websiteId)
        {
            _cacheService.Remove($"channel:tree:{websiteId}");
            _cacheService.Remove($"channel:navigation:{websiteId}");
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
                WebsiteId = channel.WebsiteId,
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
                IsEnabled = channelDto.IsEnabled,
                WebsiteId = channelDto.WebsiteId
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