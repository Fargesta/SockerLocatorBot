using SockerLocatorBot.Dtos;
using SockerLocatorBot.Helpers;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class SearchLocationHandler(ILogger<SearchLocationHandler> logger,
        IStateService stateService,
        ITelegramBotClient botClient,
        ILocationService locationService) : IBotHandler
    {
        private LocationState? locationState { get; set; } = null;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            chatId = GetInfroFromUpdate.GetChatId(update);
            var state = stateService.GetState(chatId);

            if (state is not null && state.State is LocationStateEnum.FindSocket)
            {
                locationState = state;
                return true;
            }

            return false;
        }

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            if (locationState is null || locationState.State is not LocationStateEnum.FindSocket     || update.CallbackQuery is null)
            {
                throw new ArgumentNullException(nameof(locationState), "CallbackQuery is null or wrong state");
            }

            logger.LogInformation($"Handling callback query: {update.CallbackQuery.Data}, Chat Id: {chatId}");
            await botClient.SendMessage(chatId, "Searching closest socket(s)", cancellationToken: cancellationToken);

            var locationsFound = await locationService.FindLocations(locationState.Location, 100, cancellationToken);
            if (locationsFound.Count == 0)
            {
                await botClient.SendMessage(chatId, "No sockets found nearby", cancellationToken: cancellationToken);
                logger.LogError($"No sockets found ChatId: {chatId}");
            }
            else
            {
                foreach (var location in locationsFound)
                {
                    await botClient.SendLocation(chatId, location.Location.X, location.Location.Y, cancellationToken: cancellationToken);
                }
            }
        }
    }
}
