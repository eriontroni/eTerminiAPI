using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using eTerminiAPI.Application.DTOs.Auth;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;
using eTerminiAPI.Domain.Entities;
using eTerminiAPI.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace eTerminiAPI.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;

    public AuthService(IUnitOfWork uow, IConfiguration config)
    {
        _uow = uow;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _uow.Users.FindAsync(u => u.Email == dto.Email);
        if (existing.Any())
            throw new InvalidOperationException("Email është tashmë i regjistruar.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = dto.TenantId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.Citizen,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        return await BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var users = await _uow.Users.FindAsync(u => u.Email == dto.Email.ToLower().Trim());
        var user = users.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Email ose fjalëkalimi është i gabuar.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Llogaria është çaktivizuar.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email ose fjalëkalimi është i gabuar.");

        return await BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var tokens = await _uow.RefreshTokens.FindAsync(
            t => t.Token == refreshToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

        var token = tokens.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Token i pavlefshëm ose i skaduar.");

        token.IsRevoked = true;
        _uow.RefreshTokens.Update(token);

        var users = await _uow.Users.FindAsync(u => u.Id == token.UserId);
        var user = users.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Përdoruesi nuk u gjet.");

        return await BuildAuthResponse(user);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var tokens = await _uow.RefreshTokens.FindAsync(t => t.Token == refreshToken);
        var token = tokens.FirstOrDefault();
        if (token is not null)
        {
            token.IsRevoked = true;
            _uow.RefreshTokens.Update(token);
            await _uow.SaveChangesAsync();
        }
    }

    private async Task<AuthResponseDto> BuildAuthResponse(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = await CreateRefreshTokenAsync(user.Id);
        var expiry = DateTime.UtcNow.AddMinutes(GetConfigInt("Jwt:AccessTokenExpiryMinutes", 60));

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiry,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = user.Role.ToString()
        };
    }

    private string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(GetConfigInt("Jwt:AccessTokenExpiryMinutes", 60));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("tenantId", user.TenantId.ToString()),
            new Claim("fullName", $"{user.FirstName} {user.LastName}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> CreateRefreshTokenAsync(Guid userId)
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var tokenString = Convert.ToBase64String(tokenBytes);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = tokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(GetConfigInt("Jwt:RefreshTokenExpiryDays", 7)),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _uow.RefreshTokens.AddAsync(refreshToken);
        await _uow.SaveChangesAsync();

        return tokenString;
    }

    private int GetConfigInt(string key, int fallback)
        => int.TryParse(_config[key], out var v) ? v : fallback;
}
