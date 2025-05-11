using DbManager.Models;
using NetTopologySuite.Geometries;
using SockerLocatorBot.Dtos;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Interfaces
{
    internal interface ILocationService
    {
        Task<LocationModel?> CreateLocationAsync(Update update, LocationState locationState, CancellationToken cancellationToken);
        Task DeleteLocationAsync(long locationId, CancellationToken cancellationToken);
        Task<List<LocationModel>> FindLocations(Point point, int km, CancellationToken cancellationToken);
        Task<LocationModel?> GetLocationAsync(long locationId, CancellationToken cancellationToken);
    }
}