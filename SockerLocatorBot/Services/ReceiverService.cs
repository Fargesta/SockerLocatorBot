using SockerLocatorBot.Abstract;
using SockerLocatorBot.Handlers;
using Telegram.Bot;
namespace SockerLocatorBot.Services
{
    public class ReceiverService(ITelegramBotClient botClient, UpdateHandler updateDispatcher, ILogger<ReceiverServiceBase<UpdateHandler>> logger)
        : ReceiverServiceBase<UpdateHandler>(botClient, updateDispatcher, logger);
}
