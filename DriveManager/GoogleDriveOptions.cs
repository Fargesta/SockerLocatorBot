namespace DriveManager
{
    public sealed class GoogleDriveOptions
    {
        public string ApplicationName { get; set; } = "EUC.Socket.Locator";
        public string ServiceAccountJson { get; set; } = string.Empty;
        public string FolderId { get; set; } = string.Empty;
    }
}
