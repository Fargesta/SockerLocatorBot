using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    public class WaitForImageHandler(ILogger<WaitForImageHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        public bool CanHandle(Update update)
            => update.Message?.Type == Telegram.Bot.Types.Enums.MessageType.Photo &&
                update.Message?.Photo?.Length > 0 &&
                stateService.GetState(update.Message.Chat.Id) == UserState.WaitingForImage;

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            if (update.Message?.Photo == null || update.Message.Photo.Length == 0)
            {
                throw new ArgumentNullException(nameof(update.Message.Photo), "Photo is null or empty");
            }

            logger.LogInformation($"Handling photo(s). Chat Id {update.Message.Chat.Id}");

            var images = update.Message.Photo;
            var fileId = images.Last().FileId;
            var file = await botClient.GetFile(fileId, cancellationToken: cancellationToken);

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File is null");
            }

            return;
        }
    }
}
