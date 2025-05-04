using Telegram.Bot.Types;

namespace SockerLocatorBot.Interfaces
{
    public interface IBotHandler
    {
        public bool CanHandle(Update update);
        Task HandleUpdate(Update update, CancellationToken cancellationToken);
    }
}
