using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Cms.Application.Services
{
    public class UserService : IUserService
    {
        private readonly CmsDbContext _dbContext;

        public UserService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _dbContext.CmsUsers
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return null;

            return MapToDto(user);
        }

        public async Task<UserDto> GetByUsernameAsync(string username)
        {
            var user = await _dbContext.CmsUsers
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            return MapToDto(user);
        }

        public async Task<List<UserDto>> GetListAsync(int page, int pageSize, string keyword = null)
        {
            IQueryable<CmsUser> query = _dbContext.CmsUsers
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.Username.Contains(keyword) || u.DisplayName.Contains(keyword));
            }

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto> CreateAsync(RegisterDto registerDto)
        {
            var user = new CmsUser
            {
                Username = registerDto.Username,
                PasswordHash = HashPassword(registerDto.Password),
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                IsEnabled = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.CmsUsers.Add(user);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(user.Id);
        }

        public async Task<UserDto> UpdateAsync(UserDto userDto)
        {
            var user = await _dbContext.CmsUsers.FindAsync(userDto.Id);
            if (user == null)
                throw new Exception("User not found");

            user.Email = userDto.Email;
            user.DisplayName = userDto.DisplayName;
            user.IsEnabled = userDto.IsEnabled;
            user.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(user.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _dbContext.CmsUsers.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _dbContext.CmsUsers.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
            if (user == null)
                return false;

            return VerifyPassword(password, user.PasswordHash);
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var userRoles = await _dbContext.CmsUserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            return userRoles.Select(ur => ur.Role.Name).ToList();
        }

        public async Task<bool> CheckPermissionAsync(int userId, string permissionCode)
        {
            var hasPermission = await _dbContext.CmsUserRoles
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .AnyAsync(ur => ur.UserId == userId && ur.Role.RolePermissions.Any(rp => rp.Permission.Code == permissionCode));

            return hasPermission;
        }

        private UserDto MapToDto(CmsUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName,
                IsEnabled = user.IsEnabled,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }
    }
}