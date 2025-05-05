using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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

            var inlineMarkup = new InlineKeyboardMarkup()
                .AddNewRow()
                    .AddButton(InlineKeyboardButton.WithCallbackData("220V 2pin", "2PIN"))
                    .AddButton(InlineKeyboardButton.WithCallbackData("380V 4pin", "4PIN"))
                .AddNewRow()
                    .AddButton(InlineKeyboardButton.WithCallbackData("380V 4PIN", "5PIN"))
                    .AddButton(InlineKeyboardButton.WithCallbackData("Unknown", "UNKN"));
            await botClient.SendMessage(update.Message.Chat, "Please select the socket type", replyMarkup: inlineMarkup, cancellationToken: cancellationToken);
            stateService.SetState(update.Message.Chat.Id, UserState.WaitingForType);

            return;
        }
    }
}
