using DriveManager;

namespace SockerLocatorBot
{
    internal class AppSettings
    {
        public BotOptions BotConfiguration { get; init; } = default!;
        public GoogleDriveOptions GoogleDrive { get; init; } = default!;
    }

    internal class BotOptions
    {
        public string BotToken { get; init; } = default!;
    }

    internal class GoogleDrive
    {
        public string ApplicationName { get; set; } = default!;
        public string CredentialsPath { get; set; } = default!;
        public string DirectoryId { get; set; } = default!;
    }
}
