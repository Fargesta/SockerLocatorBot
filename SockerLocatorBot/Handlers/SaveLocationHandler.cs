using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class SaveLocationHandler(ILogger<SaveLocationHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        private LocationState? locationState { get; set; } = null;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            if (update.CallbackQuery is not null &&
                update.CallbackQuery?.Message is not null &&
                update.CallbackQuery.Data is "SAVE")
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
                var state = stateService.GetState(chatId);

                if (state is not null && state.State is LocationStateEnum.ReadyToSave)
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
            logger.LogInformation($"Handling save location. Chat Id {chatId}");

            await botClient.SendMessage(chatId, "Saving location...", cancellationToken: cancellationToken);
        }
    }
}
