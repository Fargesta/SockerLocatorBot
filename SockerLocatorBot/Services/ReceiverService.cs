using SockerLocatorBot.Abstract;
using SockerLocatorBot.Handlers;
using Telegram.Bot;
namespace SockerLocatorBot.Services
{
    public class ReceiverService(ITelegramBotClient botClient, UpdateDispatcher updateDispatcher, ILogger<ReceiverServiceBase<UpdateDispatcher>> logger)
        : ReceiverServiceBase<UpdateDispatcher>(botClient, updateDispatcher, logger);
}
