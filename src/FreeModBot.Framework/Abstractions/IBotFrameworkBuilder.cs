namespace FreeModBot.Framework.Abstractions
{
    public interface IBotFrameworkBuilder
    {
        void AddCommand<TCommand>(TCommand command) where TCommand : ITelegramCommand;

        IBotFramework Build();
    }
}
