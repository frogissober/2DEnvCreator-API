using _2DEnvCreator_API.Models;

namespace _2DEnvCreator_API.Interfaces
{
    public interface IObject2DRepository
    {
        Task<IEnumerable<Object2D>> GetObjectsByEnvironmentId(int environmentId);
        Task<Object2D?> GetObjectById(int id);
        Task<Object2D> CreateObject(Object2D obj);
        Task UpdateObject(Object2D obj);
        Task DeleteObject(int id);
    }
}