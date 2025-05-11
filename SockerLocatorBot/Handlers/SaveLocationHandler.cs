using SockerLocatorBot.Dtos;
using SockerLocatorBot.Helpers;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class SaveLocationHandler(ILogger<SaveLocationHandler> logger,
        IStateService stateService,
        ITelegramBotClient botClient,
        IImageService imageService,
        ILocationService locationService) : IBotHandler
    {
        private LocationState? locationState { get; set; } = null;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            if (update.CallbackQuery is not null &&
                update.CallbackQuery?.Message is not null &&
                update.CallbackQuery.Data is "SAVE")
            {
                chatId = GetInfroFromUpdate.GetChatId(update);
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

            var location = await locationService.CreateLocationAsync(update, locationState, cancellationToken);
            if (location is null)
            {
                await botClient.SendMessage(chatId, "Error saving location. Please try again.", cancellationToken: cancellationToken);
                logger.LogError($"Location is null ChatId: {chatId}");
            }
            else
            {
                var imagesUploads = await imageService.CreateImageAsync(update, location, locationState, cancellationToken);
                if (imagesUploads.Count == 0)
                {
                    await botClient.SendMessage(chatId, "Error saving images. Please try again.", cancellationToken: cancellationToken);
                    logger.LogError($"Images are null ChatId: {chatId}");
                }
                else
                {
                    await botClient.SendMessage(chatId, "Location saved successfully!", cancellationToken: cancellationToken);
                    logger.LogInformation($"Location saved successfully. Chat Id {chatId}");
                }
            }

            stateService.ClearState(chatId);
        }
    }
}
