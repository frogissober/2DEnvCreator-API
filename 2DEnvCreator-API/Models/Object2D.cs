using _2DEnvCreator_API.Models;

namespace _2DEnvCreator_API.Models
{
    public class Object2D
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public float Rotation { get; set; }
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
        public required int EnvironmentId { get; set; }
    }
}
