using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserServiceAPI.Controllers;
using IUserServiceAPI.Repositories;
using System.Threading.Tasks;

namespace UserServiceTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserDbRepository> _userRepositoryMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private UserController _userController;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<UserController>>();
            _userRepositoryMock = new Mock<IUserDbRepository>();
            _userController = new UserController(_loggerMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public async Task CreateUser_ShouldReturnOkResult_WhenUserIsCreated()
        {
            // Arrange
            var newUser = new User
            {
                Id = "1",
                Firstname = "John",
                Lastname = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                IsAdmin = false,
                IsSeller = false,
                PhoneNumber = "12345678"
            };

            // Mock the repository method
            _userRepositoryMock.Setup(repo => repo.CreateUser(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var result = await _userController.CreateUser(newUser);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);  // Check if the result is an OkResult
            _userRepositoryMock.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Once);  // Verify the CreateUser method was called once
        }
    }
}