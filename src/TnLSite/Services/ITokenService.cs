namespace TnLSite.Services;

public interface ITokenService {
    string CreateToken(string userId);
    bool ValidateToken(string userId, string token);
}
