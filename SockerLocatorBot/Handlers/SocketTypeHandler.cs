using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SockerLocatorBot.Handlers
{
    internal class SocketTypeHandler(ILogger<SocketTypeHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        private LocationState? locationState { get; set; } = null;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            if (update.CallbackQuery is not null && update.CallbackQuery?.Message is not null)
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
                var state = stateService.GetState(chatId);

                if (state is not null && state.State is LocationStateEnum.WaitingForType)
                {
                    locationState = state;
                    return true;
                }
            }
            return false;
        }

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            if (locationState is null || update.CallbackQuery is null)
            {
                throw new ArgumentNullException(nameof(locationState), "State or CallbackQuery is null");
            }

            logger.LogInformation($"Handling callback query: {update.CallbackQuery.Data}, Chat Id: {chatId}");

            var inlineMarkup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Skip", "SKIP"),
                InlineKeyboardButton.WithCallbackData("Cancel", "CANCEL")
            });

            await botClient.SendMessage(chatId, "You can add location description or press Skip", replyMarkup: inlineMarkup, cancellationToken: cancellationToken);

            locationState.SocketType = update.CallbackQuery.Data switch
            {
                "2PIN" => "2PIN",
                "4PIN" => "4PIN",
                "5PIN" => "5PIN",
                _ => "UNKN"
            };

            locationState.State = LocationStateEnum.WaitingForDescription;
            stateService.SetState(chatId, locationState);
        }
    }
}
