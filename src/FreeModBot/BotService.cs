using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace FreeModBot
{
    public class BotService : IHostedService
    {
        private readonly ILogger<BotService> _logger;
        private readonly ITelegramBotClient _bot;

        public BotService(ILogger<BotService> logger, ITelegramBotClient bot)
        {
            _logger = logger;
            _bot = bot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var me = await _bot.GetMeAsync();
            _logger.LogInformation("Bot is starting ({username})", me.Username);
            _bot.OnReceiveGeneralError += OnGeneralError;
            _bot.OnMessage += OnMessage;

            _bot.StartReceiving();
        }

        private void OnGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            _logger.LogError(e.Exception, "Telegram error");
        }

        private async void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message?.Text is null)
            {
                return;
            }

            var text = e.Message.Text.Trim();
            if (text.StartsWith("/ping"))
            {
                await _bot.SendTextMessageAsync(e.Message.Chat.Id, "Pong!", replyToMessageId: e.Message.MessageId);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _bot.StopReceiving();
            _logger.LogInformation("Bot is stopping");
            _bot.OnMessage -= OnMessage;
            return Task.CompletedTask;
        }
    }
}
