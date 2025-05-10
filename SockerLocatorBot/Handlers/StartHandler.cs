using DbManager;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class StartHandler(ILogger<StartHandler> logger, ITelegramBotClient botClient, PgContext pgContext) : IBotHandler
    {
        public bool CanHandle(Update update)
        {
            return true;
        }

        public Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
