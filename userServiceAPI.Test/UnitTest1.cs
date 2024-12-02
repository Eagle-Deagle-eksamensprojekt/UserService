using NUnit.Framework;
using Moq;
using UserService.Models;
using UserServiceAPI.Controllers;
using IUserServiceAPI.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace UnitTestController.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserDbRepository> _userDbRepositoryMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private UserController _userController;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<UserController>>();
            _userDbRepositoryMock = new Mock<IUserDbRepository>();
            _userController = new UserController(_loggerMock.Object, _userDbRepositoryMock.Object);
        }

        // GetUser
        [Test]
        public async Task GetUser_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var userId = "user_123";
            var testUser = new User { Id = userId, Firstname = "Test User" };

            _userDbRepositoryMock.Setup(repo => repo.GetUserById(userId))
                                 .ReturnsAsync(testUser); // Return testUser if user exists

            // Act
            var result = await _userController.GetUserById(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.Not.Null);
            var returnedUser = okResult.Value as User;
            Assert.That(returnedUser, Is.Not.Null);
            Assert.That(returnedUser.Id, Is.EqualTo(userId));
        }

        // GetUser
        [Test]
        public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "non_existing_user";

            _userDbRepositoryMock.Setup(repo => repo.GetUserById(userId))
                                 .ReturnsAsync((User)null); // Return null if user doesn't exist

            // Act
            var result = await _userController.GetUserById(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        // GetAllUsers
        [Test]
        public async Task GetAllUsers_ShouldReturnListOfUsers_WhenUsersExist()
        {
            // Arrange
            var testUsers = new List<User>
            {
                new User { Id = "user_001", Firstname = "Test User 1" },
                new User { Id = "user_002", Firstname = "Test User 2" },
                new User { Id = "user_003", Firstname = "Test User 3" }
            };

            _userDbRepositoryMock.Setup(repo => repo.GetAllUsers())
                                 .ReturnsAsync(testUsers); // Return list of users

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<User>>(okResult.Value);

            var returnedUsers = okResult.Value as List<User>;
            Assert.AreEqual(3, returnedUsers.Count);
            Assert.AreEqual("user_001", returnedUsers[0].Id);
            Assert.AreEqual("user_002", returnedUsers[1].Id);
            Assert.AreEqual("user_003", returnedUsers[2].Id);
        }

        // GetAllUsers
        [Test]
        public async Task GetAllUsers_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var testUsers = new List<User>(); // Empty list

            _userDbRepositoryMock.Setup(repo => repo.GetAllUsers())
                                 .ReturnsAsync(testUsers); // Return empty list

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<User>>(okResult.Value);

            var returnedUsers = okResult.Value as List<User>;
            Assert.IsNotNull(returnedUsers);
            Assert.IsEmpty(returnedUsers);
        }

        // CreateUser
        [Test]
        public async Task CreateUser_ShouldReturnStatus201Created_WhenUserIsValid()
        {
            // Arrange
            var testUser = new User { Id = "user_123", Firstname = "Test User" };

            _userDbRepositoryMock.Setup(repo => repo.CreateUser(testUser))
                                 .ReturnsAsync(true); // Returns true when user is created

            // Act
            var result = await _userController.CreateUser(testUser);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        // CreateUser
        [Test]
        public async Task CreateUser_ShouldReturnStatus400BadRequest_WhenUserIsInvalid()
        {
            // Arrange
            var testUser = new User { Id = "user_123", Firstname = "Test User" };

            _userDbRepositoryMock.Setup(repo => repo.CreateUser(testUser))
                                 .ReturnsAsync(false); // Returns false if user cannot be created

            // Act
            var result = await _userController.CreateUser(testUser);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
            var badRequestResult = result as BadRequestResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        // CreateUser
        [Test]
        public async Task CreateUser_ShouldReturnStatus409Conflict_WhenUserAlreadyExists()
        {
            // Arrange
            var testUser = new User { Id = "user_123", Firstname = "Test User" };

            _userDbRepositoryMock.Setup(repo => repo.CreateUser(testUser))
                                 .ReturnsAsync(false); // Returns false if user already exists

            // Act
            var result = await _userController.CreateUser(testUser);

            // Assert
            Assert.IsInstanceOf<ConflictResult>(result);
            var conflictResult = result as ConflictResult;
            Assert.IsNotNull(conflictResult);
            Assert.AreEqual(409, conflictResult.StatusCode);
        }

        // DeleteUser
        [Test]
        public async Task DeleteUser_ShouldReturnStatus204NoContent_WhenUserIsDeleted()
        {
            // Arrange
            var userId = "user_123";

            _userDbRepositoryMock.Setup(repo => repo.DeleteUser(userId))
                                 .ReturnsAsync(true); // Returns true when user is deleted

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        // DeleteUser
        [Test]
        public async Task DeleteUser_ShouldReturnStatus404NotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "non_existing_user";

            _userDbRepositoryMock.Setup(repo => repo.DeleteUser(userId))
                                 .ReturnsAsync(false); // Returns false if user doesn't exist

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        // UpdateUser
        [Test]
        public async Task UpdateUser_ShouldReturnStatus200OK_WhenUserIsUpdated()
        {
            // Arrange
            var userId = "user_123";
            var testUser = new User { Id = userId, Firstname = "Updated User" };

            _userDbRepositoryMock.Setup(repo => repo.UpdateUser(userId, testUser))
                                 .ReturnsAsync(true); // Returns true when user is updated

            // Act
            var result = await _userController.UpdateUser(userId, testUser);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            _userDbRepositoryMock.Verify(repo => repo.UpdateUser(userId, testUser), Times.Once);
        }

        // UpdateUser
        [Test]
        public async Task UpdateUser_ShouldReturnStatus404NotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "non_existing_user";
            var testUser = new User { Id = userId, Firstname = "Test User" };

            _userDbRepositoryMock.Setup(repo => repo.UpdateUser(userId, testUser))
                                 .ReturnsAsync(false); // Returns false if user doesn't exist

            // Act
            var result = await _userController.UpdateUser(userId, testUser);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}
