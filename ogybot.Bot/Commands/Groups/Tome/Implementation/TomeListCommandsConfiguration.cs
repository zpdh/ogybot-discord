﻿using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Handlers;
using ogybot.Domain.Accessors;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Tome.Implementation;

public sealed partial class TomeListCommands : TomeCommand
{
    private readonly IListCommandValidator _commandValidator;

    public TomeListCommands(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor,
        ITomeListClient tomeListClient,
        IListCommandValidator commandValidator) : base(exceptionHandler, configurationAccessor, tomeListClient)
    {
        _commandValidator = commandValidator;
    }
}