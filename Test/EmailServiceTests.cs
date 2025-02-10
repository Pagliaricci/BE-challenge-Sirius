using Xunit;
using Moq;
using FluentAssertions;
using EmailService.Modules.Email.Models;
using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Repositories;
using EmailService.Modules.Email.EmailProviders;
using EmailService.Modules.Email.Services;


public class EmailServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IEmailProvider> _emailProviderMock;
    private readonly EmailsService _emailService;

    public EmailServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _emailProviderMock = new Mock<IEmailProvider>();
        var emailProviders = new List<IEmailProvider> { _emailProviderMock.Object };
        _emailService = new EmailsService(emailProviders, _userRepositoryMock.Object);
    }

    private readonly SendEmailRequest emailRequest1 = new SendEmailRequest
    {
        To = "admin@gmail.com",
        Subject = "Test Subject",
        Body = "Test Body"
    };

    private readonly SendEmailRequest emailRequest2 = new()
    {
        To = "user@gmail.com",
        Subject = "Test Subject",
        Body = "Test Body"
    };

    private readonly List<User> _users = new()
    {
        new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "admin",
            Role = "admin",
            Email = "admin@gmail.com",
            EmailsSentToday = 0,
            LastEmailReset = DateTime.UtcNow
        },
        new User
        {
            Id = 2,
            Username = "user",
            PasswordHash = "user",
            Role = "user",
            Email = "user@gmail.com",
            EmailsSentToday = 0,
            LastEmailReset = DateTime.UtcNow
        }
    };

    [Fact]
    public async Task SendEmailAsync_ShouldReturnSuccessMessage_WhenEmailIsSentSuccessfully()
    {
        // Arrange
        var user = _users[0];
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _emailProviderMock.Setup(provider => provider.SendEmailAsync(emailRequest1,user)).ReturnsAsync(true);

        // Act
        var result = await _emailService.SendEmailAsync(emailRequest1, user.Id);

        // Assert
        result.Should().Be("Email sent successfully");
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldThrowException_WhenUserHasReachedDailyLimit()
    {
        // Arrange
        var user = _users[0];
        user.EmailsSentToday = 1000;
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        Func<Task> act = async () => await _emailService.SendEmailAsync(emailRequest1, user.Id);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User has reached the daily limit of emails");
    }

    [Fact]
    public async Task SendEmailAsync_ShouldResetEmailsSentToday_WhenNewDayStarts()
    {
        // Arrange
        var user = _users[0];
        user.EmailsSentToday = 10;
        user.LastEmailReset = DateTime.UtcNow.AddDays(-1);
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _emailProviderMock.Setup(provider => provider.SendEmailAsync(emailRequest1,user)).ReturnsAsync(true);

        // Act
        var result = await _emailService.SendEmailAsync(emailRequest1, user.Id);

        // Assert
        result.Should().Be("Email sent successfully");
        user.EmailsSentToday.Should().Be(1);
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldThrowException_WhenAllProvidersFail()
    {
        // Arrange
        var user = _users[0];
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _emailProviderMock.Setup(provider => provider.SendEmailAsync(emailRequest1,user)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _emailService.SendEmailAsync(emailRequest1, user.Id);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("All email providers failed");
    }
}