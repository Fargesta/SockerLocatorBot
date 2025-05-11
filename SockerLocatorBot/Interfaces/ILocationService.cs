using DbManager.Models;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Interfaces
{
    internal interface ILocationService
    {
        Task<LocationModel?> CreateLocationAsync(Update update, CancellationToken cancellationToken);
        Task DeleteLocationAsync(long locationId, CancellationToken cancellationToken);
        Task<LocationModel?> GetLocationAsync(long locationId, CancellationToken cancellationToken);
    }
}