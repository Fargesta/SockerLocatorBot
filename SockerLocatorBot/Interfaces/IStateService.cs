namespace SockerLocatorBot.Interfaces
{
    public enum UserState
    {
        None,
        LocationShared,
        WaitingForImage,
        FindSocket,
        WaitingForType
    }

    public interface IStateService
    {
        void SetState(long chatId, UserState state);
        UserState GetState(long chatId);
        void ClearState(long chatId);
    }
}
