namespace SockerLocatorBot.Interfaces
{
    public enum UserState { None, WaitingForImage }

    public interface IStateService
    {
        void SetState(long chatId, UserState state);
        UserState GetState(long chatId);
        void ClearState(long chatId);
    }
}
