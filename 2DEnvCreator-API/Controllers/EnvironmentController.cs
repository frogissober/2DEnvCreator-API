using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _2DEnvCreator_API.Models;
using _2DEnvCreator_API.Repositories;

namespace _2DEnvCreator_API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly IEnvironmentRepository _environmentRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<EnvironmentController> _logger;

        public EnvironmentController(IEnvironmentRepository environmentRepository, IAuthenticationService authenticationService, ILogger<EnvironmentController> logger)
        {
            _environmentRepository = environmentRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Environment2D>>> GetEnvironments()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt");
                return Unauthorized();
            }

            _logger.LogInformation($"Fetching environments for user ID: {userId}");
            var environments = await _environmentRepository.GetEnvironmentsByUserId(userId);
            return Ok(environments);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Environment2D>> GetEnvironment(int id)
        {
            _logger.LogInformation($"Fetching environment with ID: {id}");
            var environment = await _environmentRepository.GetEnvironmentById(id);
            if (environment == null)
            {
                _logger.LogWarning($"Environment with ID: {id} not found");
                return NotFound();
            }
            return Ok(environment);
        }

        [HttpPost]
        public async Task<ActionResult<Environment2D>> CreateEnvironment(Environment2D environment)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt");
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state");
                return BadRequest(ModelState);
            }

            var createdEnvironment = await _environmentRepository.CreateEnvironment(environment, userId);
            _logger.LogInformation($"Environment created with ID: {createdEnvironment.Id}");
            return CreatedAtAction(nameof(GetEnvironment), new { id = createdEnvironment.Id }, createdEnvironment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnvironment(int id)
        {
            _logger.LogInformation($"Attempting to delete environment with ID: {id}");
            var environment = await _environmentRepository.GetEnvironmentById(id);
            if (environment == null)
            {
                _logger.LogWarning($"Environment with ID: {id} not found");
                return NotFound();
            }

            var userId = _authenticationService.GetCurrentAuthenticatedUserId();

            await _environmentRepository.DeleteEnvironment(id);
            _logger.LogInformation($"Environment with ID: {id} deleted");
            return NoContent();
        }
    }
}
