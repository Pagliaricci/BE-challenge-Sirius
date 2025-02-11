using Xunit;
using Moq;
using FluentAssertions;
using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Services;
using EmailService.Modules.Users.Controllers;
using Microsoft.AspNetCore.Mvc;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        var loginRequest = new LoginRequest { Username = "user", Password = "password" };
        var token = "valid-token";
        _authServiceMock.Setup(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password)).ReturnsAsync(token);

        var result = await _authController.Login(loginRequest);
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().BeEquivalentTo(new { token });
        _authServiceMock.Verify(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        var loginRequest = new LoginRequest { Username = "user", Password = "wrongpassword" };
        _authServiceMock.Setup(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password)).ReturnsAsync((string)null);

        var result = await _authController.Login(loginRequest);
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult.Value.Should().BeEquivalentTo(new { message = "Usuario o contraseña incorrectos" });
        _authServiceMock.Verify(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenUsernameIsEmpty()
    {
        var loginRequest = new LoginRequest { Username = "", Password = "password" };
        var result = await _authController.Login(loginRequest);
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult.Value.Should().BeEquivalentTo(new { message = "Usuario o contraseña incorrectos" });
        _authServiceMock.Verify(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsEmpty()
    {
        var loginRequest = new LoginRequest { Username = "user", Password = "" };
        var result = await _authController.Login(loginRequest);
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult.Value.Should().BeEquivalentTo(new { message = "Usuario o contraseña incorrectos" });
        _authServiceMock.Verify(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password), Times.Once);
    }
}
