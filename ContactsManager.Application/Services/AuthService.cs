using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using ContactsManager.Application.Common;
using ContactsManager.Application.DTOs.Auth;
using ContactsManager.Application.Interfaces;
using ContactsManager.Domain.Entities;
using ContactsManager.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ContactsManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthRepository authRepository,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterDto dto)
    {
        _logger.LogInformation("Register attempt for email: {Email}", dto.Email);

        if (string.IsNullOrWhiteSpace(dto.Password))
            return ApiResponse<UserDto>.Fail("Password is required.");

        var emailExists = await _authRepository.EmailExistsAsync(dto.Email);
        if (emailExists)
        {
            _logger.LogWarning("Registration failed: email already in use - {Email}", dto.Email);
            return ApiResponse<UserDto>.Fail("Email is already in use.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User(dto.Email, passwordHash, dto.FirstName, dto.LastName);
        var created = await _authRepository.CreateAsync(user);

        _logger.LogInformation("User registered successfully with id {Id}", created.Id);

        var userDto = _mapper.Map<UserDto>(created);
        return ApiResponse<UserDto>.Ok(userDto, "Registration successful.");
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

        var user = await _authRepository.GetByEmailAsync(dto.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: invalid credentials for {Email}", dto.Email);
            return ApiResponse<AuthResponseDto>.Fail("Invalid email or password.");
        }

        var (token, expiresAt) = GenerateJwtToken(user);

        _logger.LogInformation("User {Id} logged in successfully", user.Id);

        var userDto = _mapper.Map<UserDto>(user);
        var authResponse = new AuthResponseDto(token, expiresAt, userDto);

        return ApiResponse<AuthResponseDto>.Ok(authResponse, "Login successful.");
    }

    private (string token, DateTime expiresAt) GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
            throw new InvalidOperationException("JWT key is missing or too short (min 32 chars).");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: BuildClaims(user),
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private static Claim[] BuildClaims(User user) =>
    [
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
        new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    ];
}
