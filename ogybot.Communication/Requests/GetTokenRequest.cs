namespace ogybot.Communication.Requests;

public record GetTokenRequest(string Code, string Grant_Type = "authorization_code", string McUsername = "!bot");
public record RefreshTokenRequest(string RefreshToken, string Grant_Type = "refresh_token");