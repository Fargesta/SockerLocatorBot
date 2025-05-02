using SockerLocatorBot.Abstract;

namespace SockerLocatorBot.Services
{
    public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
        : PollingServiceBase<ReceiverService>(serviceProvider, logger);
}
