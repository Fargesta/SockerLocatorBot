using DriveManager.Interfaces;
using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class SaveLocationHandler(ILogger<SaveLocationHandler> logger,
        IStateService stateService,
        ITelegramBotClient botClient,
        IGoogleDriveService googleDrive) : IBotHandler
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

            var filePath = locationState.Photos.Last().FilePath;
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path is null");
            }

            var fileName = $"{locationState.SocketType}_{locationState.Location.X}_{locationState.Location.Y}_{DateTime.UtcNow}"
                .Replace(' ', '_')
                .Replace(':', '-')
                .Replace('/', '-');

            try
            {
                using var stream = new MemoryStream();
                await botClient.DownloadFile(filePath, stream, cancellationToken: cancellationToken);
                stream.Position = 0;

                var upload = await googleDrive.UploadImagesAsync(new[] { stream }, fileName, 4, cancellationToken);

                if (upload is null || upload.Count == 0)
                {
                    throw new ArgumentNullException(nameof(upload), "Upload is null or empty");
                }

                await botClient.SendMessage(chatId, "Location saved", cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading file");
                await botClient.SendMessage(chatId, "Error uploading file", cancellationToken: cancellationToken);
                return;
            }
            finally
            {
                stateService.ClearState(chatId);
            }
        }
    }
}
