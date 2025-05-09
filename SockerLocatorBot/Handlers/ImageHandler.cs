using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SockerLocatorBot.Handlers
{
    public class ImageHandler(ILogger<ImageHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        private LocationState? locationState { get; set; } = null;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            if (update.Message is not null && 
                update.Message.Type is Telegram.Bot.Types.Enums.MessageType.Photo &&
                update.Message.Photo is not null &&
                update.Message.Photo?.Length > 0)
            {
                chatId = update.Message.Chat.Id;
                var state = stateService.GetState(chatId);
                if (state is not null && state.State is LocationStateEnum.WaitingForImage)
                {
                    locationState = state;
                    return true;
                }
            }
            return false;
        }

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            if (locationState is null)
            {
                throw new ArgumentNullException(nameof(update.Message.Photo), "State is null");
            }

            logger.LogInformation($"Handling photo(s). Chat Id {chatId}");

            var images = update.Message!.Photo;
            var fileId = images!.Last().FileId;
            var file = await botClient.GetFile(fileId, cancellationToken: cancellationToken);

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File is null");
            }

            var inlineMarkup = new InlineKeyboardMarkup()
                .AddNewRow()
                    .AddButton(InlineKeyboardButton.WithCallbackData("220V 2pin", "2PIN"))
                    .AddButton(InlineKeyboardButton.WithCallbackData("380V 4pin", "4PIN"))
                .AddNewRow()
                    .AddButton(InlineKeyboardButton.WithCallbackData("380V 5PIN", "5PIN"))
                    .AddButton(InlineKeyboardButton.WithCallbackData("Skip", "UNKN"))
                .AddNewRow()
                    .AddButton(InlineKeyboardButton.WithCallbackData("Cancel", "CANCEL"));

            await botClient.SendMessage(update.Message.Chat, "Please select the socket type", replyMarkup: inlineMarkup, cancellationToken: cancellationToken);

            locationState.Photos.Add(file);
            locationState.State = LocationStateEnum.WaitingForType;
            stateService.SetState(chatId, locationState);
        }
    }
}
