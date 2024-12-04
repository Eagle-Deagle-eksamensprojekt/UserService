using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;  // For the IUserDbRepository interface
using UserService.Models;

namespace UserServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserDbRepository _userDbRepository;

        // Constructor to inject dependencies
        public UserController(ILogger<UserController> logger, IUserDbRepository userDbRepository)
        {
            _logger = logger;
            _userDbRepository = userDbRepository;
        }

        // Get a user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userDbRepository.GetUserById(id);

            if (user == null)
            {
                return NotFound(); // Returnerer 404, hvis brugeren ikke findes
            }

            return Ok(user); // Returnerer 200 OK med brugerens data
        }

        // Get all users
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userDbRepository.GetAllUsers();  // Fetch all users from the database
            return Ok(users);  // Return the list of users with status code 200
        }

        // Create a new user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User newUser)
        {
            if (newUser == null || string.IsNullOrWhiteSpace(newUser.Email) || string.IsNullOrWhiteSpace(newUser.Firstname))
            {
                return BadRequest("User data is invalid"); // Return 400 Bad Request if data is invalid
            }

            var wasCreated = await _userDbRepository.CreateUser(newUser);
            if (wasCreated)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser); // Return 201 Created
            }
            return Conflict("User already exists"); // Return 409 Conflict if user already exists
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            // Kald repository for at opdatere brugeren
            var wasUpdated = await _userDbRepository.UpdateUser(id, updatedUser);

            if (!wasUpdated)
            {
                return NotFound(); // Returner 404, hvis brugeren ikke findes
            }

            return Ok(); // Returner 200, hvis opdateringen lykkes
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // Kald repository for at slette brugeren
            var wasDeleted = await _userDbRepository.DeleteUser(id);

            if (!wasDeleted)
            {
                return NotFound(); // Returner 404, hvis brugeren ikke findes
            }

            return NoContent(); // Returner 204, hvis sletningen lykkes
        }

    }
}