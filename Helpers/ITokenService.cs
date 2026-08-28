namespace WebApplication1.Helpers;

public interface ITokenService
{
    string CreateJwt(int userId, string name, string email, string role);
    string CreateSecureToken();
    string HashToken(string token);
}
