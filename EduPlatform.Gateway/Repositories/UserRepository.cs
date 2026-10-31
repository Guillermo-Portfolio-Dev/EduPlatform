using Dapper;
using EduPlatform.Gateway.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EduPlatform.Gateway.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("EduPlatform_Identity")!;
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            const string sql = "SELECT Username, PasswordHash, Role FROM Users WHERE Username = @Username";

            var param = new DynamicParameters();
            param.Add("@Username", username);

            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<UserDto>(sql, param, commandType: CommandType.Text);
        }

        public async Task<string?> GetUsernameByRefreshTokenAsync(string refreshToken)
        {
            const string sql = @"
                    SELECT Username FROM RefreshTokens 
                    WHERE Token = @Token AND IsRevoked = 0 AND ExpiresAt > GETDATE()";

            using var conn = new SqlConnection(_connectionString);

            var param = new DynamicParameters();
            param.Add("@Token", refreshToken);

            return await conn.QueryFirstOrDefaultAsync<string>(sql, param, commandType: CommandType.Text);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            const string sql = "UPDATE RefreshTokens SET IsRevoked = 1 WHERE Token = @Token";

            using var conn = new SqlConnection(_connectionString);

            var param = new DynamicParameters();
            param.Add("@Token", refreshToken);

            await conn.ExecuteAsync(sql, param, commandType: CommandType.Text);
        }

        public async Task SaveRefreshTokenAsync(string username, string refreshToken, DateTime expiresAt)
        {
            const string sql = @"
                    INSERT INTO RefreshTokens (Username, Token, ExpiresAt) 
                    VALUES (@Username, @Token, @ExpiresAt)";

            using var connection = new SqlConnection(_connectionString);

            var param = new DynamicParameters();
            param.Add("@Username", username);
            param.Add("@Token", refreshToken);
            param.Add("@ExpiresAt", expiresAt);

            await connection.ExecuteAsync(sql, param, commandType: CommandType.Text);
        }
    }
}
