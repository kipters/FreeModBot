using System.Threading.Tasks;

namespace FreeModBot.Framework.Abstractions
{
    public interface IBotFramework
    {
        ValueTask StartAsync();
        ValueTask StopAsync();
    }
}
