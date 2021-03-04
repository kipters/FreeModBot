using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreeModBot.Framework.Abstractions
{

    public interface ITelegramCommand
    {
        ValueTask HandleCommand(ITelegramBotClient bot, Message message);
    }
}
