using _2DEnvCreator_API.Models;

public interface IEnvironmentRepository
{
    Task<IEnumerable<Environment2D>> GetAllEnvironments();
    Task<IEnumerable<Environment2D>> GetEnvironmentsByUserId(string userId);
    Task<Environment2D?> GetEnvironmentById(int id);
    Task<Environment2D> CreateEnvironment(Environment2D environment, string userId);
    Task DeleteEnvironment(int id);
}