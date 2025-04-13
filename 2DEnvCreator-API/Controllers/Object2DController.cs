using Microsoft.AspNetCore.Mvc;
using _2DEnvCreator_API.Models;
using _2DEnvCreator_API.Interfaces;

namespace _2DEnvCreator_API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class Object2DController : ControllerBase
    {
        private readonly IObject2DRepository _object2DRepository;

        public Object2DController(IObject2DRepository object2DRepository)
        {
            _object2DRepository = object2DRepository;
        }

        [HttpGet("environment/{environmentId}")]
        public async Task<ActionResult<IEnumerable<Object2D>>> GetObjectsByEnvironment(int environmentId)
        {
            var objects = await _object2DRepository.GetObjectsByEnvironmentId(environmentId);
            return Ok(objects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Object2D>> GetObject(int id)
        {
            var obj = await _object2DRepository.GetObjectById(id);
            if (obj == null)
            {
                return NotFound();
            }
            return obj;
        }

        [HttpPost]
        public async Task<ActionResult<Object2D>> CreateObject(Object2D obj)
        {
            var createdObject = await _object2DRepository.CreateObject(obj);
            return CreatedAtAction(nameof(GetObject), new { id = createdObject.Id }, createdObject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateObject(int id, Object2D obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }
            
            await _object2DRepository.UpdateObject(obj);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObject(int id)
        {
            await _object2DRepository.DeleteObject(id);
            return NoContent();
        }
    }
}