using DbManager;
using DbManager.Models;
using SockerLocatorBot.Helpers;
using SockerLocatorBot.Interfaces;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Services
{
    internal class LocationService(ILogger<LocationService> logger, PgContext pgContext, IStateService stateService, IUserService userService) : ILocationService
    {
        public async Task<LocationModel?> GetLocationAsync(long locationId, CancellationToken cancellationToken) =>
            await pgContext.Locations.FindAsync(locationId, cancellationToken);

        public async Task DeleteLocationAsync(long locationId, CancellationToken cancellationToken)
        {
            pgContext.Locations.Remove(new LocationModel { Id = locationId });
            await pgContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<LocationModel?> CreateLocationAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = GetInfroFromUpdate.GetChatId(update);
            var user = await userService.GetUserAsync(update, cancellationToken);

            if (user is null)
            {
                logger.LogError("User not found for chatId: {ChatId}", chatId);
                return null;
            }

            var state = stateService.GetState(chatId);
            if (state is null)
            {
                logger.LogError("State not found for chatId: {ChatId}", chatId);
                return null;
            }

            var locationModel = new LocationModel
            {
                CreatedBy = user,
                UpdatedBy = user,
                Location = state.Location,
                SocketType = state.SocketType,
                Description = state.Description
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
    }
}
