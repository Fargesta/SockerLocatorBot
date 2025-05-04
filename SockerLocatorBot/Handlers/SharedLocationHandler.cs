using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class SharedLocationHandler : IUpdateHandler
    {
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
