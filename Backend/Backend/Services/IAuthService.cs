using Backend.Models.DTO;

namespace Backend.Services;

public interface IAuthService
{
    public Task<(bool Succeeded, string Token, IEnumerable<string> Errors)> Register(RegisterRequest registerRequest);
    public Task<(bool Succeeded, string Token)> Login(LoginRequest loginRequest);
}