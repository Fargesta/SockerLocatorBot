namespace DriveManager.Interfaces
{
    public interface IGoogleDriveService
    {
        public sealed record UploadItem(Stream Content, string Name, string MimeType, string? ParentFolderId = null);

        ValueTask<IReadOnlyList<string>> UploadAsync(IEnumerable<UploadItem> items, CancellationToken cancellationToken = default);
        ValueTask DownloadAsync(IEnumerable<string> fileIds, string targetFolder, CancellationToken cancellationToken = default);
        ValueTask DeleteAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default);
    }
}
