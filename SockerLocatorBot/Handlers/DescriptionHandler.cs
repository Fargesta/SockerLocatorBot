using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using SockerLocatorBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class DescriptionHandler(ILogger<DescriptionHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        private LocationState? locationState { get; set; } = null;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            if (update.CallbackQuery is not null && update.CallbackQuery.Message is not null)
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
                var state = stateService.GetState(chatId);
                if (state is not null && state.State is LocationStateEnum.WaitingForDescription)
                {
                    locationState = state;
                    return true;
                }
            }
            else if (update.Message is not null && update.Message.Type is Telegram.Bot.Types.Enums.MessageType.Text)
            {
                chatId = update.Message.Chat.Id;
                var state = stateService.GetState(chatId);
                if (state is not null && state.State is LocationStateEnum.WaitingForDescription)
                {
                    locationState = state;
                    return true;
                }
            }
            return false;
        }

        public Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
