using System;
using System.Threading;
using System.Threading.Tasks;
using FreeModBot.Commands;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace FreeModBot.Tests
{
    public class PingCommandTest
    {
        [Fact]
        public async Task RepliesToUser()
        {
            // Arrange
            var sut = new PingCommand();
            var bot = new Mock<ITelegramBotClient>();
            bot.Setup(b => b.SendTextMessageAsync(
                It.Is<ChatId>(cid => cid.Identifier == 123),
                It.Is<string>(x => !string.IsNullOrWhiteSpace(x)),
                It.IsAny<ParseMode>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                456,
                It.IsAny<IReplyMarkup>(),
                It.IsAny<CancellationToken>()
            ))
            .Verifiable();

            var msg = new Message
            {
                Chat = new Chat { Id = 123 },
                MessageId = 456
            };

            // Act
            await sut.HandleCommand(bot.Object, msg);

            // Assert
            bot.VerifyAll();
        }

        [Fact]
        public async Task RepliesPong()
        {
            // Arrange
            var sut = new PingCommand();
            var bot = new Mock<ITelegramBotClient>();
            bot.Setup(b => b.SendTextMessageAsync(
                It.Is<ChatId>(cid => cid.Identifier == 123),
                "_Pong_",
                It.Is<ParseMode>(pm => pm == ParseMode.Markdown || pm == ParseMode.MarkdownV2),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                456,
                It.IsAny<IReplyMarkup>(),
                It.IsAny<CancellationToken>()
            ))
            .Verifiable();

            var msg = new Message
            {
                Chat = new Chat { Id = 123 },
                MessageId = 456
            };

            // Act
            await sut.HandleCommand(bot.Object, msg);

            // Assert
            bot.VerifyAll();
        }
    }
}
