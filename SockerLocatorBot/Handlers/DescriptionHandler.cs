using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SockerLocatorBot.Handlers
{
    internal class DescriptionHandler(ILogger<DescriptionHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        private LocationState? locationState { get; set; } = null;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            if (update.CallbackQuery is not null && update.CallbackQuery.Message is not null)
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
                var state = stateService.GetState(chatId);
                if (state is not null && state.State is LocationStateEnum.WaitingForDescription)
                {
                    locationState = state;
                    return true;
                }
            }
            else if (update.Message is not null && update.Message.Type is Telegram.Bot.Types.Enums.MessageType.Text)
            {
                chatId = update.Message.Chat.Id;
                var state = stateService.GetState(chatId);
                if (state is not null && state.State is LocationStateEnum.WaitingForDescription)
                {
                    locationState = state;
                    return true;
                }
            }
            return false;
        }

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            if (locationState is null)
            {
                throw new ArgumentNullException(nameof(locationState), "State is null");
            }

            logger.LogInformation($"Handling description. Chat Id {chatId}");

            if (update.CallbackQuery is not null)
            {
                locationState.Description = "No description";
            }
            else if (!string.IsNullOrEmpty(update.Message?.Text))
            {
                locationState.Description = update.Message.Text;
            }

            locationState.State = LocationStateEnum.ReadyToSave;

            var summary = $"Location: {locationState.Location}\n" +
                $"Numer of image: {locationState.Photos.Count().ToString() }\n" +
                $"Socket Type: {locationState.SocketType}\n" +
                $"Description: {locationState.Description}";

            var inlineMarkup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Save", "SAVE"),
                InlineKeyboardButton.WithCallbackData("Cancel", "CANCEL")
            });

            await botClient.SendMessage(chatId, summary, replyMarkup: inlineMarkup, cancellationToken: cancellationToken);
            stateService.ClearState(chatId);
        }
    }
}
