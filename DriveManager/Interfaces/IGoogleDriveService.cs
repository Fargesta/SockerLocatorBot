using DriveManager.Dtos;

namespace DriveManager.Interfaces
{
    public interface IGoogleDriveService
    {
        Task<IList<UploadFileData>> UploadImagesAsync(IEnumerable<Stream> imageStreams, string fileName, int maxParllelUploads, CancellationToken cancellationToken = default);

        Task<IDictionary<string, byte[]>> GetImagesAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default);

        Task DeleteFilesAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default);
    }
}
