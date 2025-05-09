using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    public class NewOrFindCallbackHandler(ILogger<NewOrFindCallbackHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        private LocationState locationState { get; set; } = null!;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            if (update.CallbackQuery is not null && update.CallbackQuery?.Message is not null)
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
                var state = stateService.GetState(chatId);

                if(state is not null && state.State is LocationStateEnum.LocationShared)
                {
                    locationState = state;
                    return true;
                }
            }
            return false;
        }

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {

            if (locationState.State is not LocationStateEnum.LocationShared || update.CallbackQuery is null)
            {
                throw new ArgumentNullException(nameof(locationState), "CallbackQuery is null or wrong state");
            }

            logger.LogInformation($"Handling callback query: {update.CallbackQuery.Data}, Chat Id: {chatId}");

            if (update.CallbackQuery.Data == "ADDNEW")
            {
                locationState.State = LocationStateEnum.WaitingForImage;
                stateService.SetState(chatId, locationState);
                await botClient.SendMessage(chatId, "Please send me a photo of the socket", cancellationToken: cancellationToken);
            }
            else if (update.CallbackQuery.Data == "FINDNEAR")
            {
                locationState.State = LocationStateEnum.FindSocket;
                stateService.SetState(chatId, locationState);
                await botClient.SendMessage(chatId, "Searching closest socket(s)", cancellationToken: cancellationToken);
            }
            else
            {
                stateService.ClearState(chatId);
                throw new ArgumentException("Invalid callback query data", nameof(update.CallbackQuery.Data));
            }
        }
    }
}
