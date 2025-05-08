using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using SockerLocatorBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SockerLocatorBot.Handlers
{
    internal class SocketTypeCallbackHandler(ILogger<SocketTypeCallbackHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        public bool CanHandle(Update update)
        {
            if (update.CallbackQuery is not null && update.CallbackQuery?.Message is not null)
            {
                var state = stateService.GetState(update.CallbackQuery.Message.Chat.Id);

                if (state is not null && state.State is LocationStateEnum.WaitingForType)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            if (update.CallbackQuery == null || update.CallbackQuery.Message == null)
            {
                throw new ArgumentNullException(nameof(update.CallbackQuery), "CallbackQuery or Message is null");
            }

            var state = stateService.GetState(update.CallbackQuery.Message.Chat.Id);

            if (state == null || state.State is not LocationStateEnum.LocationShared)
            {
                throw new ArgumentNullException(nameof(state), "State is null or wrong state");
            }

            logger.LogInformation($"Handling callback query: {update.CallbackQuery.Data}, Chat Id: {update.CallbackQuery.Message.Chat.Id}");

            var inlineMarkup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Skip", "SKIP"),
                InlineKeyboardButton.WithCallbackData("Cancel", "CANCEL")
            });

            await botClient.SendMessage(update.CallbackQuery.Message.Chat.Id, "Searching closest socket(s)", cancellationToken: cancellationToken);

            state.SocketType = update.CallbackQuery.Data switch
            {
                "2PIN" => "2PIN",
                "4PIN" => "4PIN",
                "5PIN" => "5PIN",
                _ => "UNKN"
            };

            state.State = LocationStateEnum.WaitingForDescription;
        }
    }
}
