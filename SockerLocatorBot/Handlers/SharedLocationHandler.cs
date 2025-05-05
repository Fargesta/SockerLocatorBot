using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SockerLocatorBot.Handlers
{
    public class SharedLocationHandler(ILogger<SharedLocationHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        public bool CanHandle(Update update)
            => update.Message?.Location != null && stateService.GetState(update.Message.Chat.Id) == UserState.None;

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            if (update.Message?.Location == null)
            {
                throw new ArgumentNullException(nameof(update.Message.Location), "Location is null");
            }
            var lat = update.Message.Location.Latitude;
            var lon = update.Message.Location.Longitude;

            logger.LogInformation($"Handling location. Chat Id {update.Message.Chat.Id}");

            var inlineMarkup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Add New", "add_new"),
                InlineKeyboardButton.WithCallbackData("Find", "find_near")
            });

            await botClient.SendMessage(update.Message.Chat, "Got your location! What next?", replyMarkup: inlineMarkup);
            stateService.SetState(update.Message.Chat.Id, UserState.WaitingForImage);
        }
    }
}
