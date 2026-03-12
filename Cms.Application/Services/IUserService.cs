using Cms.Application.DTOs;

namespace Cms.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> GetByUsernameAsync(string username);
        Task<List<UserDto>> GetListAsync(int page, int pageSize, string? keyword = null);
        Task<UserDto> CreateAsync(RegisterDto registerDto);
        Task<UserDto> UpdateAsync(UserDto userDto);
        Task DeleteAsync(int id);
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<bool> CheckPermissionAsync(int userId, string permissionCode);
    }
}