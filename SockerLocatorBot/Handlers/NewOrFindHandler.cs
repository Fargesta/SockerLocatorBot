using SockerLocatorBot.Dtos;
using SockerLocatorBot.Helpers;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class NewOrFindHandler(ILogger<NewOrFindHandler> logger,
        IStateService stateService,
        ITelegramBotClient botClient,
        ILocationService locationService,
        IImageService imageService) : IBotHandler
    {
        private LocationState? locationState { get; set; } = null;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            chatId = GetInfroFromUpdate.GetChatId(update);
            var state = stateService.GetState(chatId);

            if(state is not null && state.State is LocationStateEnum.LocationShared && (update.CallbackQuery?.Data is "ADDNEW" || update.CallbackQuery?.Data is "FINDNEAR"))
            {
                locationState = state;
                return true;
            }
            
            return false;
        }

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {

            if (locationState is null || locationState.State is not LocationStateEnum.LocationShared || update.CallbackQuery is null || update.CallbackQuery.Message is null)
            {
                throw new ArgumentNullException(nameof(locationState), "CallbackQuery is null or wrong state");
            }

            logger.LogInformation($"Handling callback query: {update.CallbackQuery.Data}, Chat Id: {chatId}");

            if (update.CallbackQuery.Data is "ADDNEW")
            {
                locationState.State = LocationStateEnum.WaitingForImage;
                stateService.SetState(chatId, locationState);
                await botClient.SendMessage(chatId, "Please send me a photo of the socket", cancellationToken: cancellationToken);
            }
            else if (update.CallbackQuery.Data is "FINDNEAR")
            {
                locationState.State = LocationStateEnum.FindSocket;
                stateService.SetState(chatId, locationState);

                logger.LogInformation($"Handling callback query: {update.CallbackQuery.Data}, Chat Id: {chatId}");
                await botClient.SendMessage(chatId, "Searching closest socket(s)", cancellationToken: cancellationToken);

                var locationsFound = await locationService.FindLocations(locationState.Location, 10, 1, cancellationToken);
                if (locationsFound.Count == 0)
                {
                    await botClient.SendMessage(chatId, "No sockets found nearby", cancellationToken: cancellationToken);
                    logger.LogError($"No sockets found ChatId: {chatId}");
                }
                else
                {
                    foreach (var location in locationsFound)
                    {
                        await botClient.SendLocation(chatId, location.Location.Y, location.Location.X, cancellationToken: cancellationToken);

                        if (!string.IsNullOrEmpty(location.Description))
                            await botClient.SendMessage(chatId, location.Description, cancellationToken: cancellationToken);

                        var loadImages = await imageService.DowloadImagesAsync(location.Images, cancellationToken);
                        await Task.Delay(200, cancellationToken);
                        foreach (var image in loadImages)
                        {
                            var caption = location.Images.FirstOrDefault(x => x.DriveFileId == image.FileId)?.Description;
                            if(caption is not null)
                            {
                                await botClient.SendPhoto(chatId, new InputFileStream(new MemoryStream(image.Bytes)), caption: caption, cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await botClient.SendPhoto(chatId, new InputFileStream(new MemoryStream(image.Bytes)), cancellationToken: cancellationToken);
                            }
                            logger.LogInformation($"Image sent: {image.FileId}, Chat Id: {chatId}");
                            await Task.Delay(200, cancellationToken);
                        }
                    }
                }
                stateService.ClearState(chatId);
            }
            else
            {
                stateService.ClearState(chatId);
                throw new ArgumentException("Invalid callback query data", nameof(update.CallbackQuery.Data));
            }
        }
    }
}
