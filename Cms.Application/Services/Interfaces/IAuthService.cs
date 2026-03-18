using Cms.Application.Services.Dtos;
using Cms.Domain.Entities;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public interface IAuthService
    {
        Task<TokenResponseDto> GenerateTokens(CmsUser user, int websiteId);
        Task<TokenResponseDto> RefreshToken(string refreshToken, int userId);
        void RevokeToken(int userId);
        bool IsTokenRevoked(int userId);
        Task<CmsUser> ValidateUser(string username, string password);
    }
}