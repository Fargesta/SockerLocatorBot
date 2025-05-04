using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    public class UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IEnumerable<IBotHandler> botHandlers) : IUpdateHandler
    {
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            logger.LogInformation("HandleError: {Exception}", exception);
            // Cooldown in case of network connection error
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            foreach (var handler in botHandlers)
            {
                if (handler.CanHandle(update))
                {
                    await handler.HandleUpdate(update, cancellationToken);
                    return;
                }
            }

            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id;

            if (chatId == null)
            {
                logger.LogWarning("Update with Chat Id is null");
            }
            else
            {
                await botClient.SendMessage(chatId, "Please type /help command to see how bot operates.");
            }
            return;
        }
    }
}
