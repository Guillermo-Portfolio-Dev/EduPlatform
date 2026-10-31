using EduPlatform.Gateway.Models;

namespace EduPlatform.Gateway.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto?> GetByUsernameAsync(string username);
        Task SaveRefreshTokenAsync(string username, string refreshToken, DateTime expiresAt);
        Task<string?> GetUsernameByRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
