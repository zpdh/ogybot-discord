using Discord;
using ogybot.Domain.Enums;

namespace ogybot.Domain.Entities;

public sealed class PageSessionInfo
{
    required public IUserMessage Message { get; set; }
    required public CancellationTokenSource TimeoutCts { get; set; }
}