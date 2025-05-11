using Telegram.Bot.Types;

namespace SockerLocatorBot.Helpers
{
    public static class GetInfroFromUpdate
    {
        public static long GetFromId(Update update)
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

        public static string GetFromName(Update update)
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

        public static long GetChatId(Update update)
        {
            if (update is { CallbackQuery: { Message: { Chat: { Id: var idc } } } })
            {
                return idc;
            }
            else if (update is { Message: { Chat: { Id: var idm } } })
            {
                return idm;
            }
            else
            {
                throw new ArgumentNullException("Chat.Id not found in GetChatId");
            }
        }
    }
}
