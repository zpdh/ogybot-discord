namespace ogybot.Communication.Requests;

public record DecrementRewardsRequest(string Username, double Aspects, double Emeralds);