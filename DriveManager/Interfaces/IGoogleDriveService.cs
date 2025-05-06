namespace DriveManager.Interfaces
{
    public interface IGoogleDriveService
    {
        Task<IList<string>> UploadImagesAsync(IEnumerable<Stream> imageStreams, CancellationToken cancellationToken = default);

        Task<IDictionary<string, byte[]>> GetImagesAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default);

        Task DeleteFilesAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default);
    }
}
