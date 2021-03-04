using System.Threading;
using System.Threading.Tasks;
using FreeModBot.Framework.Abstractions;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace FreeModBot
{
    public class BotService : IHostedService
    {
        private readonly IBotFramework _bot;

        public BotService(IBotFramework bot)
        {
            _bot = bot;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bot.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bot.StopAsync();
        }
    }
}
