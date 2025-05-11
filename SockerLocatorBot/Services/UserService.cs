using DbManager;
using DbManager.Models;
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
                    Id = GetFromId(update),
                    FirstName = GetFromName(update),
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

        private long GetFromId(Update update)
        {
            if (update is { CallbackQuery: { From: { Id: var idc } } })
            {
                return idc;
            }
            else if (update is { Message: { From: { Id: var idm } } })
            {
                return idm;
            }
            else
            {
                throw new ArgumentNullException("From.id not found in GetFromId");
            }
        }

        private string GetFromName(Update update)
        {
            if (update is { CallbackQuery: { From: { FirstName: var fnc } } })
            {
                return fnc;
            }
            else if (update is { Message: { From: { FirstName: var fnm } } })
            {
                return fnm;
            }
            else
            {
                throw new ArgumentNullException("From.FirstName not found in GetFromName");
            }
        }
    }
}
