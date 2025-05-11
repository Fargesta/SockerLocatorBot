using DbManager;
using DbManager.Models;
using SockerLocatorBot.Helpers;
using SockerLocatorBot.Interfaces;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Services
{
    internal class UserService(ILogger<UserService> logger, PgContext pgContext) : IUserService
    {
        public async Task<UserModel?> GetUserAsync(Update update) => await pgContext.Users.FindAsync(GetFromId(update));

        public async Task<UserModel> CreateUserAsync(Update update)
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
                await pgContext.Users.AddAsync(user);
                await pgContext.SaveChangesAsync();
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
