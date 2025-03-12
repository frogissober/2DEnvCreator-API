using _2DEnvCreator_API.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace _2DEnvCreator_API.Repositories
{
    public class EnvironmentRepository : IEnvironmentRepository
    {
        private readonly string _connectionString;

        public EnvironmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Environment2D>> GetAllEnvironments()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryAsync<Environment2D>("SELECT * FROM Environments");
        }

        public async Task<IEnumerable<Environment2D>> GetEnvironmentsByUserId(string userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryAsync<Environment2D>("SELECT * FROM Environments WHERE UserId = @UserId", new { UserId = userId });
        }

        public async Task<Environment2D?> GetEnvironmentById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<Environment2D>("SELECT * FROM Environments WHERE Id = @Id", new { Id = id });
        }

        public async Task<Environment2D> CreateEnvironment(Environment2D environment, string userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var sql = "INSERT INTO Environments (Name, Height, Width, UserId) VALUES (@Name, @Height, @Width, @UserId); SELECT CAST(SCOPE_IDENTITY() as int)";
            var parameters = new { environment.Name, environment.Height, environment.Width, UserId = userId };
            var id = await connection.ExecuteScalarAsync<int>(sql, parameters);
            environment.Id = id;
            return environment;
        }

        public async Task DeleteEnvironment(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var sql = "DELETE FROM Environments WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
