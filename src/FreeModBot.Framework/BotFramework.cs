using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FreeModBot.Framework.Abstractions;
using FreeModBot.Framework.Attributes;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace FreeModBot.Framework
{
    public class BotFramework : IBotFramework
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ImmutableDictionary<string, ITelegramCommand> _commands;
        private readonly ImmutableArray<ITelegramCommand> _adminCommands;
        private string _username;

        public BotFramework(ITelegramBotClient botClient, IReadOnlyList<ITelegramCommand> commands)
        {
            _botClient = botClient;
            var pairs = commands
                .Select(x => (txt: x.GetType().GetCustomAttribute<CommandAttribute>(), cmd: x))
                .Where(x => x.txt is not null)
                .Select(x => (txt: x.txt!, x.cmd));

            _commands = pairs.ToImmutableDictionary(x => x.txt.Command, x => x.cmd);
            _adminCommands = pairs
                .Where(x => x.txt.AdminOnly)
                .Select(x => x.cmd)
                .ToImmutableArray();
        }

        public async ValueTask HandleUpdate(Update update)
        {
            if (update?.Message?.Text is null)
            {
                return;
            }

            var text = update.Message.Text;

            if (!text.StartsWith("/"))
            {
                return;
            }

            var commandText = text
                .Substring(1)
                .Replace($"@{_username}", "");

            if (!_commands.TryGetValue(commandText, out var cmd))
            {
                return;
            }

            if (_adminCommands.Contains(cmd))
            {
                var admins = await _botClient.GetChatAdministratorsAsync(update.Message.Chat);
                if (admins.Any(x => x.User.Id == update.Message.From.Id))
                {
                    await cmd.HandleCommand(_botClient, update.Message);
                }
            }
            else
            {
                await cmd.HandleCommand(_botClient, update.Message);
            }
        }

        public async ValueTask StartAsync()
        {
            var me = await _botClient.GetMeAsync();
            _username = me.Username;
            _botClient.OnMessage += OnMessage;
            _botClient.StartReceiving();
        }

        private void OnMessage(object? sender, MessageEventArgs e)
        {
            OnMessageAsync(sender, e)
                .GetAwaiter()
                .GetResult();
        }

        private async ValueTask OnMessageAsync(object? sender, MessageEventArgs e)
        {
            var update = new Update { Message = e.Message };
            await HandleUpdate(update);
        }

        public ValueTask StopAsync()
        {
            _botClient.StopReceiving();
            _botClient.OnMessage -= OnMessage;
            return ValueTask.CompletedTask;
        }
    }
}
