using DbManager;
using DbManager.Models;
using SockerLocatorBot.Helpers;
using SockerLocatorBot.Interfaces;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Services
{
    internal class UserService(ILogger<UserService> logger, PgContext pgContext) : IUserService
    {
        public async Task<UserModel?> GetUserAsync(Update update, CancellationToken cancellationToken) =>
            await pgContext.Users.FindAsync(GetInfroFromUpdate.GetFromId(update), cancellationToken);

        public async Task<UserModel> CreateUserAsync(Update update, CancellationToken cancellationToken)
        {
            try
            {
                var user = new UserModel
                {
                    Id = GetInfroFromUpdate.GetFromId(update),
                    FirstName = GetInfroFromUpdate.GetFromName(update),
                    LastName = update.Message?.From?.LastName,
                    UserName = update.Message?.From?.Username,
                    LanguageCode = update.Message?.From?.LanguageCode,
                    RoleId = 3, // Guest role for new users
                };
                await pgContext.Users.AddAsync(user, cancellationToken);
                await pgContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating user");
                throw;
            }
        }
    }
}
