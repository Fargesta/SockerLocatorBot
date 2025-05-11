using DbManager.Models;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Interfaces
{
    internal interface IUserService
    {
        Task<UserModel> CreateUserAsync(Update update);
        Task<UserModel?> GetUserAsync(Update update);
    }
}