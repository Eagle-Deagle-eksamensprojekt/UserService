using MongoDB.Driver;
using UserService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Services;

namespace Services
{
    public class UserMongoDBService : IUserDbRepository
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly ILogger<UserMongoDBService> _logger;

        public UserMongoDBService(ILogger<UserMongoDBService> logger, IConfiguration configuration)
        {
            _logger = logger;

            var connectionString = configuration["MongoConnectionString"] ?? "<blank>";
            var databaseName = configuration["DatabaseName"] ?? "<blank>";
            var collectionName = configuration["CollectionName"] ?? "<blank>";
            
            _logger.LogInformation($"Connecting to MongoDB using: {connectionString}");
            _logger.LogInformation($"Using database: {databaseName}");
            _logger.LogInformation($"Using collection: {collectionName}");

            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _userCollection = database.GetCollection<User>(collectionName);
                _logger.LogInformation("Connected to MongoDB.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to connect to MongoDB: {0}", ex.Message);
                throw; 
            }
        }

        public async Task<bool> CreateUser(User user)
        {
            try
            {
                await _userCollection.InsertOneAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create user: {0}", ex.Message);
                return false;
            }
        }

        public async Task<User> GetUserById(string id)
        {
            return await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userCollection.Find(u => true).ToListAsync();
        }

        public async Task<bool> UpdateUser(string id, User updatedUser)
        {
            try
            {
                var result = await _userCollection.ReplaceOneAsync(u => u.Id == id, updatedUser);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update user: {0}", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteUser(string id)
        {
            try
            {
                var result = await _userCollection.DeleteOneAsync(u => u.Id == id);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete user: {0}", ex.Message);
                return false;
            }
        }

        public async Task<List<User>> GetUsersByOwnerId(string ownerId)
        {
            return await _userCollection.Find(u => u.Id == ownerId).ToListAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                _logger.LogInformation($"Getting user by email: {email}");
                return await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get user by email: {0}", ex.Message);
                return null;
            }
            
        }

    }
}