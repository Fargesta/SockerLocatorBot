using Microsoft.Extensions.Caching.Memory;
using SockerLocatorBot.Dtos;
using SockerLocatorBot.Interfaces;

namespace SockerLocatorBot.Services
{
    public class StateService(IMemoryCache memoryCache) : IStateService
    {
        public LocationState? GetState(long chatId) => memoryCache.TryGetValue(chatId, out LocationState? state) ? state : null;
        public void CreateState(long chatId, LocationState state) => memoryCache.Set(chatId, state, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });
        public void SetState(long chatId, LocationState state) => memoryCache.Set(chatId, state);
        public void ClearState(long chatId) => memoryCache.Remove(chatId);
    }
}
