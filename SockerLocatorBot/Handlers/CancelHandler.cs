using SockerLocatorBot.Dtos;
using SockerLocatorBot.Helpers;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class CancelHandler(ILogger<CancelHandler> logger, IStateService stateService, ITelegramBotClient botClient) : IBotHandler
    {
        private LocationState? locationState { get; set; } = null;
        private long chatId { get; set; }

        public bool CanHandle(Update update)
        {
            if (update.CallbackQuery is not null &&
                update.CallbackQuery.Data is "CANCEL")
            {
                chatId = GetInfroFromUpdate.GetChatId(update);
                var state = stateService.GetState(chatId);

                if (state is not null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            if (update.CallbackQuery is null)
            {
                throw new ArgumentNullException(nameof(locationState), "CallbackQuery is null");
            }
        }
    }
}
