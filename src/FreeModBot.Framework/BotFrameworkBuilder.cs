using System.Collections.Generic;
using FreeModBot.Framework.Abstractions;
using Telegram.Bot;

namespace FreeModBot.Framework
{
    public class BotFrameworkBuilder : IBotFrameworkBuilder
    {
        private readonly ITelegramBotClient _botClient;

        public BotFrameworkBuilder(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        private List<ITelegramCommand> Commands { get; } = new();
        public void AddCommand<TCommand>(TCommand command) where TCommand : ITelegramCommand
        {
            Commands.Add(command);
        }

        public IBotFramework Build()
        {
            return new BotFramework(_botClient, Commands);
        }
    }
}
