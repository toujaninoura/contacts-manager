using ContactsManager.Application.DTOs.Auth;
using ContactsManager.Application.Services;
using ContactsManager.Domain.Entities;
using ContactsManager.Domain.Interfaces;
using FluentAssertions;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ContactsManager.Tests.Auth;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IAuthRepository> _authRepositoryMock = null!;
    private Mock<ILogger<AuthService>> _loggerMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private IConfiguration _configuration = null!;
    private AuthService _authService = null!;

    [SetUp]
    public void SetUp()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _mapperMock = new Mock<IMapper>();

        var inMemorySettings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "TestSecretKey_MustBe32CharsAtLeast!!",
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience"
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authService = new AuthService(
            _authRepositoryMock.Object,
            _mapperMock.Object,
            _configuration,
            _loggerMock.Object);
    }

    [Test]
    public async Task RegisterAsync_should_return_userDto_when_valid()
    {
        var dto = new RegisterDto("john@example.com", "Password123!", "John", "Doe");
        var createdUser = new User(dto.Email, "hash", dto.FirstName, dto.LastName);
        var expectedDto = new UserDto(createdUser.Id, createdUser.Email, createdUser.FirstName, createdUser.LastName, createdUser.CreatedAt);

        _authRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
        _authRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(createdUser);
        _mapperMock.Setup(m => m.Map<UserDto>(createdUser)).Returns(expectedDto);

        var result = await _authService.RegisterAsync(dto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(dto.Email);
    }

    [Test]
    public async Task RegisterAsync_should_return_fail_when_email_already_exists()
    {
        var dto = new RegisterDto("existing@example.com", "Password123!", "Jane", "Doe");

        _authRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(true);

        var result = await _authService.RegisterAsync(dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Email");
        _authRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task RegisterAsync_should_hash_password_not_store_plain_text()
    {
        var dto = new RegisterDto("hash@example.com", "PlainPassword!", "Hash", "Test");
        User? capturedUser = null;

        _authRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
        _authRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync((User u) => u);
        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto(0, dto.Email, dto.FirstName, dto.LastName, DateTime.UtcNow));

        await _authService.RegisterAsync(dto);

        capturedUser.Should().NotBeNull();
        capturedUser!.PasswordHash.Should().NotBe(dto.Password);
        BCrypt.Net.BCrypt.Verify(dto.Password, capturedUser.PasswordHash).Should().BeTrue();
    }

    [Test]
    public async Task LoginAsync_should_return_token_when_valid_credentials()
    {
        var password = "Password123!";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User("john@example.com", hash, "John", "Doe");
        var userDto = new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.CreatedAt);

        _authRepositoryMock.Setup(r => r.GetByEmailAsync("john@example.com")).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        var result = await _authService.LoginAsync(new LoginDto("john@example.com", password));

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Test]
    public async Task LoginAsync_should_return_fail_when_user_not_found()
    {
        _authRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _authService.LoginAsync(new LoginDto("unknown@example.com", "password"));

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid");
    }

    [Test]
    public async Task LoginAsync_should_return_fail_when_password_incorrect()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!");
        var user = new User("john@example.com", hash, "John", "Doe");

        _authRepositoryMock.Setup(r => r.GetByEmailAsync("john@example.com")).ReturnsAsync(user);

        var result = await _authService.LoginAsync(new LoginDto("john@example.com", "WrongPassword!"));

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid");
    }

    [Test]
    public async Task LoginAsync_should_return_fail_data_null_when_no_valid_credentials()
    {
        _authRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _authService.LoginAsync(new LoginDto("notoken@example.com", "anypassword"));

        result.Success.Should().BeFalse();
        result.Data.Should().BeNull();
    }
}
