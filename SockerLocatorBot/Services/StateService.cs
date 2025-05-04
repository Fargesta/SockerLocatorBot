using Microsoft.Extensions.Caching.Memory;
using SockerLocatorBot.Interfaces;

namespace SockerLocatorBot.Services
{
    public class StateService(IMemoryCache memoryCache) : IStateService
    {
        public void ClearState(long chatId) => memoryCache.Remove(chatId);
        public UserState GetState(long chatId) => memoryCache.TryGetValue(chatId, out UserState state) ? state : UserState.None;
        public void SetState(long chatId, UserState state) => memoryCache.Set(chatId, state);
    }
}
