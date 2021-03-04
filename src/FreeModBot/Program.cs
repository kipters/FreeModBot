using System;
using System.Threading.Tasks;
using FreeModBot.Commands;
using FreeModBot.Framework;
using FreeModBot.Framework.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FreeModBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, builder) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddConsole();
                    }
                    else
                    {
                        builder.AddJsonConsole();
                    }
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<BotService>();
                    services.AddSingleton<ITelegramBotClient>(s =>
                    {
                        var config = s.GetRequiredService<IConfiguration>();
                        var token = config["Telegram:Token"];
                        var bot = new TelegramBotClient(token);
                        return bot;
                    });
                    services.AddSingleton<IBotFramework>(s =>
                    {
                        var botClient = s.GetRequiredService<ITelegramBotClient>();
                        var builder = new BotFrameworkBuilder(botClient);
                        builder.AddCommand(new PingCommand());
                        return builder.Build();
                    });
                })
                // .UseBotFramework(o =>
                // {
                //     o.AddCommand<PingCommand>();
                //     o.AddCommand<KickCommand>();
                // })
                .UseSystemd()
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}
