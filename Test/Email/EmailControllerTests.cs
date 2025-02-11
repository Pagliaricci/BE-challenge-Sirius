using Xunit;
using Moq;
using FluentAssertions;
using EmailService.Modules.Email.Models;
using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Repositories;
using EmailService.Modules.Email.Services;
using EmailService.Modules.Email.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

public class EmailControllerTests{

    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly EmailController _emailController;
    private readonly List<User> _users;

    public EmailControllerTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _emailController = new EmailController(_emailServiceMock.Object);
        _users = TestData.GetTestUsers();
    }

    [Fact]
    public async Task CreateEmail_ShouldReturnOk_WhenEmailIsSentSuccessfully()
    {
        var user = _users[0];
        _emailServiceMock.Setup(service => service.SendEmailAsync(TestData.EmailRequest1, user.Id)).ReturnsAsync("Email sent successfully");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        _emailController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };


        var result = await _emailController.CreateEmail(TestData.EmailRequest1);
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().Be("Email sent successfully");
        _emailServiceMock.Verify(service => service.SendEmailAsync(TestData.EmailRequest1, user.Id), Times.Once);
    }

    [Fact]
    public async Task CreateEmail_ShouldReturnBadRequest_WhenUserIdIsInvalid()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "invalid")
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        _emailController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _emailController.CreateEmail(TestData.EmailRequest1);
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.Should().Be("Invalid user ID.");
        _emailServiceMock.Verify(service => service.SendEmailAsync(TestData.EmailRequest1, It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateEmail_ShouldReturnBadRequest_WhenEmailServiceFails()
    {
        var user = _users[0];
        _emailServiceMock.Setup(service => service.SendEmailAsync(TestData.EmailRequest1, user.Id)).ThrowsAsync(new Exception("Failed to send email."));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        _emailController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _emailController.CreateEmail(TestData.EmailRequest1);
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.Should().Be("Failed to send email.");
        _emailServiceMock.Verify(service => service.SendEmailAsync(TestData.EmailRequest1, user.Id), Times.Once);
    }

    [Fact]
    public async Task CreateEmail_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _emailServiceMock.Setup(service => service.SendEmailAsync(TestData.EmailRequest1, 3)).ThrowsAsync(new Exception("Sender or recipient not found."));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "3")
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        _emailController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _emailController.CreateEmail(TestData.EmailRequest1);
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.Should().Be("Sender or recipient not found.");
        _emailServiceMock.Verify(service => service.SendEmailAsync(TestData.EmailRequest1, 3), Times.Once);
    }

    [Fact]
    public async Task CreateEmail_ShouldReturnBadRequest_WhenUserHasReachedDailyLimit()
    {
        var user = _users[0];
        user.EmailsSentToday = 1000;
        _emailServiceMock.Setup(service => service.SendEmailAsync(TestData.EmailRequest1, user.Id)).ThrowsAsync(new Exception("User has reached the daily limit of emails"));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        _emailController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _emailController.CreateEmail(TestData.EmailRequest1);
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.Should().Be("User has reached the daily limit of emails");
        _emailServiceMock.Verify(service => service.SendEmailAsync(TestData.EmailRequest1, user.Id), Times.Once);
    }

    [Fact]
    public async Task CreateEmail_ShouldReturnBadRequest_WhenAllProvidersFail()
    {
        var user = _users[0];
        _emailServiceMock.Setup(service => service.SendEmailAsync(TestData.EmailRequest1, user.Id)).ThrowsAsync(new Exception("All email providers failed"));

         var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        _emailController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _emailController.CreateEmail(TestData.EmailRequest1);
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.Should().Be("All email providers failed");
        _emailServiceMock.Verify(service => service.SendEmailAsync(TestData.EmailRequest1, user.Id), Times.Once);
    }
}

