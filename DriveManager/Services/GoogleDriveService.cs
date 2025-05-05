using DriveManager.Interfaces;
using Google.Apis.Drive.v3;
using static DriveManager.Interfaces.IGoogleDriveService;

namespace DriveManager.Services
{
    internal sealed class GoogleDriveService(DriveService driveService, GoogleDriveOptions driveOptions) : IGoogleDriveService
    {
        public ValueTask<IReadOnlyList<string>> UploadAsync(IEnumerable<UploadItem> items, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public ValueTask DownloadAsync(IEnumerable<string> fileIds, string targetFolder, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public ValueTask DeleteAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
