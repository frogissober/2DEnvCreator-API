using _2DEnvCreator_API.Models;
using _2DEnvCreator_API.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace _2DEnvCreator_API.Repositories
{
    public class Object2DRepository : IObject2DRepository
    {
        private readonly string _connectionString;

        public Object2DRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Object2D>> GetObjectsByEnvironmentId(int environmentId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryAsync<Object2D>(
                "SELECT * FROM Objects2D WHERE EnvironmentId = @EnvironmentId",
                new { EnvironmentId = environmentId });
        }

        public async Task<Object2D?> GetObjectById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<Object2D>(
                "SELECT * FROM Objects2D WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<Object2D> CreateObject(Object2D obj)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var sql = @"INSERT INTO Objects2D (Name, PositionX, PositionY, Rotation, ScaleX, ScaleY, EnvironmentId) 
                       VALUES (@Name, @PositionX, @PositionY, @Rotation, @ScaleX, @ScaleY, @EnvironmentId);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = await connection.ExecuteScalarAsync<int>(sql, obj);
            obj.Id = id;
            return obj;
        }

        public async Task UpdateObject(Object2D obj)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var sql = @"UPDATE Objects2D 
                       SET Name = @Name, PositionX = @PositionX, PositionY = @PositionY, 
                           Rotation = @Rotation, ScaleX = @ScaleX, ScaleY = @ScaleY 
                       WHERE Id = @Id AND EnvironmentId = @EnvironmentId";
            await connection.ExecuteAsync(sql, obj);
        }

        public async Task DeleteObject(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "DELETE FROM Objects2D WHERE Id = @Id",
                new { Id = id });
        }
    }
}