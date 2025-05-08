using SockerLocatorBot.Dtos;

namespace SockerLocatorBot.Interfaces
{
    public interface IStateService
    {
        public LocationState? GetState(long chatId);
        public void CreateState(long chatId, LocationState state);
        public void SetState(long chatId, LocationState state);
        public void ClearState(long chatId);
    }
}
