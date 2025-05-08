using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SockerLocatorBot.Handlers
{
    public class WaitForImageHandler(ILogger<WaitForImageHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        public bool CanHandle(Update update)
        {
            if (update.Message is not null && 
                update.Message.Type is Telegram.Bot.Types.Enums.MessageType.Photo && 
                update.Message.Photo?.Length > 0)
            {
                var state = stateService.GetState(update.Message.Chat.Id);
                if (state is not null && state.State is LocationStateEnum.WaitingForImage)
                {
                    return true;
                }
            }
            return false;
        }

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
                    .AddButton(InlineKeyboardButton.WithCallbackData("Skip", "UNKN"))
                .AddNewRow()
                    .AddButton(InlineKeyboardButton.WithCallbackData("Cancel", "CANCEL"));

            await botClient.SendMessage(update.Message.Chat, "Please select the socket type", replyMarkup: inlineMarkup, cancellationToken: cancellationToken);

            var state = stateService.GetState(update.Message.Chat.Id);

            if(state is null)
            {
                throw new ArgumentNullException(nameof(state), "State is null");
            }

            state.Photos.Add(file);
            state.State = LocationStateEnum.WaitingForType;
            stateService.SetState(update.Message.Chat.Id, state);

            return;
        }
    }
}
