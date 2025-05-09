using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;
using SockerLocatorBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class DescriptionHandler(ILogger<DescriptionHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        public bool CanHandle(Update update)
        {
            if (update.CallbackQuery is not null && update.CallbackQuery.Message is not null)
            {
                var state = stateService.GetState(update.CallbackQuery.Message.Chat.Id);
                if (state is not null && state.State is LocationStateEnum.WaitingForDescription)
                {
                    return true;
                }
            }
            else if (update.Message is not null && update.Message.Type is Telegram.Bot.Types.Enums.MessageType.Text)
            {
                var state = stateService.GetState(update.Message.Chat.Id);
                if (state is not null && state.State is LocationStateEnum.WaitingForDescription)
                {
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
