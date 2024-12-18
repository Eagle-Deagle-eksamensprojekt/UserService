using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;  // For the IUserDbRepository interface
using UserService.Models;

namespace Services
{
    public interface IUserDbRepository
    {
        Task<bool> CreateUser(User user);  // Create a new user
        Task<User> GetUserById(string id);  // Retrieve a user by their ID
        Task<List<User>> GetAllUsers();  // Retrieve all users
        Task<bool> UpdateUser(string id, User updatedUser);  // Update user information
        Task<bool> DeleteUser(string id);  // Delete a user by their ID
        Task<List<User>> GetUsersByOwnerId(string ownerId);  // Get users by an associated owner ID
        Task<User> GetUserByEmail(string email);  // Get a user by their email address // bliver brugt i login
    }
}