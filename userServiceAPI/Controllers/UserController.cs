using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;  // For the IUserDbRepository interface
using UserService.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Diagnostics;

namespace UserServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserDbRepository _userDbRepository;

        // Constructor to inject dependencies
        public UserController(ILogger<UserController> logger, IUserDbRepository userDbRepository)
        {
            _logger = logger;
            _userDbRepository = userDbRepository;

            // For at modtage opkald fra anden service
            // Log the IP address of the service
            var hostName = System.Net.Dns.GetHostName(); // Get the name of the host
            var ips = System.Net.Dns.GetHostAddresses(hostName); // Get the IP addresses associated with the host
            var _ipaddr = ips.First().MapToIPv4().ToString(); // Get the first IPv4 address
            _logger.LogInformation($"XYZ Service responding from {_ipaddr}"); // Log the IP address
        }

        /// <summary>
        /// Hent version af Service
        /// </summary>
        [HttpGet("version")]
        public async Task<Dictionary<string,string>> GetVersion()
        {
            var properties = new Dictionary<string, string>();
            var assembly = typeof(Program).Assembly;

            properties.Add("service", "OrderService");
            var ver = FileVersionInfo.GetVersionInfo(
                typeof(Program).Assembly.Location).ProductVersion ?? "N/A";
            properties.Add("version", ver);
            
            var hostName = System.Net.Dns.GetHostName();
            var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
            var ipa = ips.First().MapToIPv4().ToString() ?? "N/A";
            properties.Add("ip-address", ipa);
            
            return properties;
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
            if (newUser == null || string.IsNullOrWhiteSpace(newUser.Email) || string.IsNullOrWhiteSpace(newUser.Firstname) || string.IsNullOrWhiteSpace(newUser.Password))
            {
                return BadRequest("User data is invalid");
            }

            // Generér salt
            byte[] saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(saltBytes);
            }
            newUser.Salt = Convert.ToBase64String(saltBytes);

            // Hash passwordet med salt
            newUser.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: newUser.Password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // Gem i databasen
            var wasCreated = await _userDbRepository.CreateUser(newUser);
            if (wasCreated)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
            }
            return Conflict("User already exists");
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

        // Denne bliver brugt af AuthService
        // Get der returnere user by email
        // Hent en bruger ved email, bruges af authService
        [HttpGet("byEmail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            _logger.LogInformation("Getting user by email");
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogInformation("Email is required.");
                return BadRequest("Email is required.");
            }

            _logger.LogInformation($"Getting user by email: {email}");
            var user = await _userDbRepository.GetUserByEmail(email);
            _logger.LogInformation($"User found with email: {email}");

            if (user == null)
            {
                _logger.LogInformation($"User not found with email: {email}");
                return NotFound("User not found");
            }

            _logger.LogInformation($"User found with email: {email}");
            return Ok(user);
        }
    }
}