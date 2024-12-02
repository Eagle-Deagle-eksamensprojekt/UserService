using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IUserServiceAPI.Repositories;  // For the IUserDbRepository interface
using  UserService.Models;

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
        return null; // ikke implementeret kode endnu
    }

        // Get all users
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
    {
        return null; // ikke implementeret kode endnu
    }

        // Create a new user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        return null; // ikke implementeret kode endnu
    }

        // Update an existing user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
    {
        return null; // ikke implementeret kode endnu
    }

        // Delete a user by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
    {
        return null; // ikke implementeret kode endnu
    }
    }
}