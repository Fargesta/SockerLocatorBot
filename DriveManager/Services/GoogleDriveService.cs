using DriveManager.Interfaces;
using Google.Apis.Drive.v3;
using static DriveManager.Interfaces.IGoogleDriveService;

namespace DriveManager.Services
{
    internal sealed class GoogleDriveService(DriveService driveService) : IGoogleDriveService
    {
        public Task DeleteFilesAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<string, byte[]>> GetImagesAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> UploadImagesAsync(IEnumerable<Stream> imageStreams, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
