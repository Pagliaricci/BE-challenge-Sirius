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
    private readonly List<User> _users;

    public EmailServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _emailProviderMock = new Mock<IEmailProvider>();
        var emailProviders = new List<IEmailProvider> { _emailProviderMock.Object };
        _emailService = new EmailsService(emailProviders, _userRepositoryMock.Object);
        _users = TestData.GetTestUsers();
    }

    [Fact]
    public async Task SendEmailAsync_ShouldReturnSuccessMessage_WhenEmailIsSentSuccessfully()
    {
        var user = _users[0];
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _emailProviderMock
            .Setup(provider => provider.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<User>()))
            .ReturnsAsync(true);

        var result = await _emailService.SendEmailAsync(TestData.EmailRequest1, user.Id);

        result.Should().Be("Email sent successfully");
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
        _emailProviderMock.Verify(provider => provider.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldThrowException_WhenUserHasReachedDailyLimit()
    {
        var user = _users[0];
        user.EmailsSentToday = 1000;
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        Func<Task> act = async () => await _emailService.SendEmailAsync(TestData.EmailRequest1, user.Id);

        await act.Should().ThrowAsync<Exception>().WithMessage("User has reached the daily limit of emails");
    }

    [Fact]
    public async Task SendEmailAsync_ShouldResetEmailsSentToday_WhenNewDayStarts()
    {
        var user = _users[0];
        user.EmailsSentToday = 10;
        user.LastEmailReset = DateTime.UtcNow.AddDays(-1);
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _emailProviderMock
            .Setup(provider => provider.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<User>()))
            .ReturnsAsync(true);

        var result = await _emailService.SendEmailAsync(TestData.EmailRequest1, user.Id);

        result.Should().Be("Email sent successfully");
        user.EmailsSentToday.Should().Be(1);
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
        _emailProviderMock.Verify(provider => provider.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldThrowException_WhenAllProvidersFail()
    {
        var user = _users[0];
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _emailProviderMock
            .Setup(provider => provider.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<User>()))
            .ReturnsAsync(false);

        Func<Task> act = async () => await _emailService.SendEmailAsync(TestData.EmailRequest1, user.Id);
        await act.Should().ThrowAsync<Exception>().WithMessage("All email providers failed");
    }

    [Fact]
    public async Task SendEmailAsync_ShouldThrowException_WhenUserNotFound()
    {
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null!);

        Func<Task> act = async () => await _emailService.SendEmailAsync(TestData.EmailRequest1, 1);
        await act.Should().ThrowAsync<Exception>().WithMessage("Sender or recipient not found");
    }


}
