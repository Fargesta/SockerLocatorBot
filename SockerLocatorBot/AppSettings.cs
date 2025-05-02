namespace SockerLocatorBot
{
    internal class AppSettings
    {
        public BotOptions BotConfiguration { get; init; } = default!;
    }

    internal class BotOptions
    {
        public string BotToken { get; init; } = default!;
    }

}
