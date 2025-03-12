namespace _2DEnvCreator_API.Models
{
    public class Environment2D
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
