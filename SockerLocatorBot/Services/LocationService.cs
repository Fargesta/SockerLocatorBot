using DbManager;
using DbManager.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SockerLocatorBot.Dtos;
using SockerLocatorBot.Helpers;
using SockerLocatorBot.Interfaces;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Services
{
    internal class LocationService(ILogger<LocationService> logger, PgContext pgContext, IUserService userService) : ILocationService
    {
        public async Task<LocationModel?> GetLocationAsync(long locationId, CancellationToken cancellationToken) =>
            await pgContext.Locations.FindAsync(locationId, cancellationToken);

        public async Task DeleteLocationAsync(long locationId, CancellationToken cancellationToken)
        {
            pgContext.Locations.Remove(new LocationModel { Id = locationId });
            await pgContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<LocationModel?> CreateLocationAsync(Update update, LocationState locationState, CancellationToken cancellationToken)
        {
            var chatId = GetInfroFromUpdate.GetChatId(update);
            var user = await userService.GetUserAsync(update, cancellationToken);

            if (user is null)
            {
                logger.LogError("User not found for chatId: {ChatId}", chatId);
                return null;
            }

            var locationModel = new LocationModel
            {
                CreatedById = user.Id,
                UpdatedById = user.Id,
                Location = locationState.Location,
                SocketType = locationState.SocketType,
                Description = locationState.Description
            };

            try
            {
                await pgContext.Locations.AddAsync(locationModel, cancellationToken);
                await pgContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                logger.LogInformation("Location created: {Location}", locationModel);
                return locationModel;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating location");
                throw;
            }
        }

        public async Task<List<LocationModel>> FindLocations(Point point, int km, CancellationToken cancellationToken)
        {
            var radius = RadiusHelper.KmToDegrees(km);

            var locations = await pgContext.Locations
                .Where(l => l.Location.IsWithinDistance(point, radius))
                .OrderBy(l => l.Location.Distance(point))
                .Take(3)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return locations;
        }
    }
}
