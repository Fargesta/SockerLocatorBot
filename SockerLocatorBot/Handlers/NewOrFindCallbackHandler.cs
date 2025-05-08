using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    public class NewOrFindCallbackHandler(ILogger<NewOrFindCallbackHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        public bool CanHandle(Update update)
            => update.CallbackQuery != null && (update.CallbackQuery.Data == "add_new" || update.CallbackQuery.Data == "find_near");

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

            if (update.CallbackQuery.Data == "add_new")
            {
                state.State = LocationStateEnum.LocationShared;
                stateService.SetState(update.CallbackQuery.Message.Chat.Id, state);
                await botClient.SendMessage(update.CallbackQuery.Message.Chat.Id, "Please send me a photo of the socket", cancellationToken: cancellationToken);
            }
            else if (update.CallbackQuery.Data == "find_near")
            {
                state.State = LocationStateEnum.FindSocket;
                stateService.SetState(update.CallbackQuery.Message.Chat.Id, state);
                await botClient.SendMessage(update.CallbackQuery.Message.Chat.Id, "Searching closest socket(s)", cancellationToken: cancellationToken);
            }
            else
            {
                stateService.ClearState(update.CallbackQuery.Message.Chat.Id);
                throw new ArgumentException("Invalid callback query data", nameof(update.CallbackQuery.Data));
            }

            return;
        }
    }
}
