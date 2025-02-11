using Xunit;
using Moq;
using FluentAssertions;
using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Repositories;
using EmailService.Modules.Users.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;
    private readonly User _testUser;
    private readonly CreateUser _testNewUser;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);

        _testUser = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = "hashedpassword",
            Role = "user",
            Email = "test@example.com",
            EmailsSentToday = 0,
            LastEmailReset = DateTime.UtcNow
        };

        _testNewUser = new CreateUser
        {
            Username = "newuser",
            Password = "newpassword",
            Role = "user",
            Email = "newuser@example.com"
        };
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(_testUser.Id)).ReturnsAsync(_testUser);

        var result = await _userService.GetUserByIdAsync(_testUser.Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_testUser);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null!);

        var result = await _userService.GetUserByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnListOfUsers()
    {
        var users = new List<User> { _testUser };
        _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users);

        var result = await _userService.GetAllUsersAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(users, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnUser_WhenUserIsCreatedSuccessfully()
    {
        _userRepositoryMock.Setup(repo => repo.FindUserByUsernameAndPassword(_testNewUser.Username, _testNewUser.Password))
            .ReturnsAsync((User)null!);
        _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<CreateUser>()))
            .ReturnsAsync(new User
            {
                Id = 2,
                Username = _testNewUser.Username,
                PasswordHash = "hashedpassword",
                Role = _testNewUser.Role,
                Email = _testNewUser.Email,
                EmailsSentToday = 0,
                LastEmailReset = DateTime.UtcNow
            });

        var result = await _userService.CreateUserAsync(_testNewUser);

        result.Should().NotBeNull();
        result.Username.Should().Be(_testNewUser.Username);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowException_WhenUserAlreadyExists()
    {
        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(_testNewUser.Email))
            .ReturnsAsync(_testUser);

        Func<Task> act = async () => await _userService.CreateUserAsync(_testNewUser);

        await act.Should().ThrowAsync<Exception>().WithMessage("User already exists.");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowException_WhenUserDataIsInvalid()
    {
        var invalidUser = new CreateUser { Username = "", Password = "", Role = "", Email = "" };

        Func<Task> act = async () => await _userService.CreateUserAsync(invalidUser);

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid data.");
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnTrue_WhenUserIsUpdatedSuccessfully()
    {
        _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(_testUser)).ReturnsAsync(true);

        var result = await _userService.UpdateUserAsync(_testUser);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserIsDeletedSuccessfully()
    {
        _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(_testUser.Id)).ReturnsAsync(true);

        var result = await _userService.DeleteUserAsync(_testUser.Id);

        result.Should().BeTrue();
    }
}