using System.Threading.Tasks;
using FreeModBot.Framework.Abstractions;
using FreeModBot.Framework.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FreeModBot.Commands
{
    [Command("ping")]
    public class PingCommand : ITelegramCommand
    {
        public async ValueTask HandleCommand(ITelegramBotClient bot, Message message)
        {
            await bot.SendTextMessageAsync(
                message.Chat.Id,
                "_Pong_",
                ParseMode.Markdown,
                replyToMessageId: message.MessageId
            );
        }
    }
}
