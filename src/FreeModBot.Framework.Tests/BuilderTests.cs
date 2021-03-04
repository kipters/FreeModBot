using System;
using System.Threading;
using System.Threading.Tasks;
using FreeModBot.Framework.Abstractions;
using FreeModBot.Framework.Attributes;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace FreeModBot.Framework.Tests
{
    public class BuilderTests
    {
        public Mock<ITelegramBotClient> BotMock { get; }

        public BuilderTests()
        {
            BotMock = new Mock<ITelegramBotClient>();
            BotMock
                .Setup(b => b.GetMeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User
                {
                    Username = "Bot"
                });
        }

        [Theory]
        [InlineData("/test")]
        [InlineData("/test@Bot")]
        public async Task BotHandlesCommand(string msgText)
        {
            // Arrange
            var builder = new BotFrameworkBuilder(BotMock.Object);
            var command = new TestCommand();
            builder.AddCommand<TestCommand>(command);
            var bot = (BotFramework)builder.Build();
            await bot.StartAsync();

            var update = new Update
            {
                Message = new Message
                {
                    Text = msgText
                }
            };

            // Act
            await bot.HandleUpdate(update);

            // Assert
            Assert.True(command.Called);
        }

        [Fact]
        public async Task IgnoresUnregisteredCommands()
        {
            // Arrange
            var builder = new BotFrameworkBuilder(BotMock.Object);
            var command = new TestCommand();
            builder.AddCommand<TestCommand>(command);
            var bot = (BotFramework)builder.Build();

            var update = new Update
            {
                Message = new Message
                {
                    Text = "/foo"
                }
            };

            // Act
            await bot.HandleUpdate(update);

            // Assert
            Assert.False(command.Called);
        }

        [Fact]
        public async Task IgnoresAdminCommandsFromNonAdmin()
        {
            // Arrange
            BotMock
                .Setup(b => b.GetChatAdministratorsAsync(It.IsAny<ChatId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ChatMember>());
            var builder = new BotFrameworkBuilder(BotMock.Object);
            var command = new AdminTestCommand();
            builder.AddCommand<AdminTestCommand>(command);
            var bot = (BotFramework)builder.Build();

            var update = new Update
            {
                Message = new Message
                {
                    Text = "/admin",
                    From = new User { Id = 123 },
                    Chat = new Chat { Id = 456 }
                },
            };

            // Act
            await bot.HandleUpdate(update);

            // Assert
            Assert.False(command.Called);
        }

        [Fact]
        public async Task HandlesAdminCommandsFromAdmin()
        {
            var chat = new Chat { Id = 456 };
            // Arrange
            BotMock
                .Setup(b => b.GetChatAdministratorsAsync(
                    It.Is<ChatId>(c => c.Identifier == chat.Id),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ChatId id, CancellationToken token) => {
                    return new[]
                    {
                        new ChatMember
                        {
                            User = new User { Id = 123 }
                        }
                    };
                });
            var builder = new BotFrameworkBuilder(BotMock.Object);
            var command = new AdminTestCommand();
            builder.AddCommand<AdminTestCommand>(command);
            var bot = (BotFramework)builder.Build();

            var update = new Update
            {
                Message = new Message
                {
                    Text = "/admin",
                    From = new User { Id = 123 },
                    Chat = chat
                },
            };

            // Act
            await bot.HandleUpdate(update);

            // Assert
            Assert.True(command.Called);
        }
    }

    [Command("test")]
    public class TestCommand : ITelegramCommand
    {
        public bool Called { get; private set; }

        public ValueTask HandleCommand(ITelegramBotClient bot, Message message)
        {
            Called = true;
            return ValueTask.CompletedTask;
        }
    }

    [Command("admin", AdminOnly = true)]
    public class AdminTestCommand : ITelegramCommand
    {
        public bool Called { get; private set; }

        public ValueTask HandleCommand(ITelegramBotClient bot, Message message)
        {
            Called = true;
            return ValueTask.CompletedTask;
        }
    }
}
