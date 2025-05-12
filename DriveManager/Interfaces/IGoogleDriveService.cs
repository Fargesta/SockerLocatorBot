using System.Collections.Concurrent;
using DriveManager.Dtos;

namespace DriveManager.Interfaces
{
    public interface IGoogleDriveService
    {
        Task<ConcurrentBag<UploadFileData>> UploadImagesAsync(IEnumerable<Stream> imageStreams, string fileName, int maxParllelUploads, CancellationToken cancellationToken = default);

        Task<ConcurrentBag<byte[]>> GetImagesAsync(IEnumerable<string> fileIds, int maxParallelDownloads, CancellationToken cancellationToken = default);

        Task DeleteFilesAsync(IEnumerable<string> fileIds, int maxParralelDeletes, CancellationToken cancellationToken = default);
    }
}
