using Xunit;
using Moq;
using FluentAssertions;
using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Repositories;
using EmailService.Modules.Users.Services;
using System.Threading.Tasks;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthService _authService;
    private readonly User _testUser;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authService = new AuthService(_userRepositoryMock.Object);
        _testUser = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = "testpassword",
            Role = "user",
            Email = "test@example.com"
        };
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        _userRepositoryMock
            .Setup(repo => repo.FindUserByUsernameAndPassword(_testUser.Username, _testUser.PasswordHash))
            .ReturnsAsync(_testUser);

        var token = await _authService.AuthenticateAsync(_testUser.Username, _testUser.PasswordHash);

        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        _userRepositoryMock
            .Setup(repo => repo.FindUserByUsernameAndPassword(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        var token = await _authService.AuthenticateAsync("invalidUser", "invalidPassword");

        token.Should().BeNull();
    }

    [Fact]
    public void GetUser_ShouldReturnUser_WhenCredentialsAreValid()
    {
        _userRepositoryMock
            .Setup(repo => repo.FindUserByUsernameAndPassword(_testUser.Username, _testUser.PasswordHash))
            .ReturnsAsync(_testUser);

        var user = _authService.GetUser(_testUser.Username, _testUser.PasswordHash);

        user.Should().NotBeNull();
        user.Should().BeEquivalentTo(_testUser);
    }

    [Fact]
    public void GetUser_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        _userRepositoryMock
            .Setup(repo => repo.FindUserByUsernameAndPassword(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        var user = _authService.GetUser("invalidUser", "invalidPassword");

        user.Should().BeNull();
    }
}
