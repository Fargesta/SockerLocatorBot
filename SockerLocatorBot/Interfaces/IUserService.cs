using DbManager.Models;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Interfaces
{
    internal interface IUserService
    {
        Task<UserModel> CreateUserAsync(Update update, CancellationToken cancellationToken);
        Task<UserModel?> GetUserAsync(Update update, CancellationToken cancellationToken);
    }
}