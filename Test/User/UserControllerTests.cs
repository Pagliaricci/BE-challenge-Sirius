using Xunit;
using Moq;
using FluentAssertions;
using EmailService.Modules.Users.Controllers;
using EmailService.Modules.Users.Services;
using EmailService.Modules.Users.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _userController = new UserController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnOk_WhenUsersExist()
    {
        var users = new List<UserDTO>
        {
            new() { Id = 1, Username = "user1", Role = "Admin", Email = "user1@example.com" },
            new() { Id = 2, Username = "user2", Role = "User", Email = "user2@example.com" }
        };
        _userServiceMock.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(users);

        var result = await _userController.GetAllUsers();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().BeEquivalentTo(users);
        _userServiceMock.Verify(service => service.GetAllUsersAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnOk_WhenUserExists()
    {
        var user = new User { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = "hashedpassword" };
        _userServiceMock.Setup(service => service.GetUserByIdAsync(1)).ReturnsAsync(user);

        var result = await _userController.GetUserById(1);
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().BeEquivalentTo(user);
        _userServiceMock.Verify(service => service.GetUserByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        _userServiceMock.Setup(service => service.GetUserByIdAsync(1)).ReturnsAsync((User)null);

        var result = await _userController.GetUserById(1);
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        _userServiceMock.Verify(service => service.GetUserByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreated_WhenUserIsCreated()
    {
        var user = new CreateUser { Username = "user1", Password = "password", Role = "Admin", Email = "user@gmail.com" };
        var createdUser = new User { Id = 1, Username = user.Username, Email = user.Email, Role = user.Role, PasswordHash = "hashedpassword" };
        _userServiceMock.Setup(service => service.CreateUserAsync(user)).ReturnsAsync(createdUser);

        var result = await _userController.CreateUser(user);
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult.ActionName.Should().Be("GetUserById");
        createdResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, object> { { "id", createdUser.Id } });
        createdResult.Value.Should().BeEquivalentTo(createdUser);
        _userServiceMock.Verify(service => service.CreateUserAsync(user), Times.Once);
    }
}