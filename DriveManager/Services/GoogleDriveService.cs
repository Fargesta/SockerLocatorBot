using System.Collections.Concurrent;
using DriveManager.Dtos;
using DriveManager.Interfaces;
using Google.Apis.Drive.v3;
using Microsoft.Extensions.Options;

namespace DriveManager.Services
{
    internal sealed class GoogleDriveService(DriveService driveService, IOptions<GoogleDriveOptions> opts) : IGoogleDriveService
    {
        private readonly string directoryId = opts.Value.DirectoryId;

        public async Task<IList<UploadFileData>> UploadImagesAsync(IEnumerable<Stream> imageStreams,
            string fileName,
            int maxParllelUploads = 4,
            CancellationToken cancellationToken = default)
        {
            var ids = new ConcurrentBag<UploadFileData>();

            await Parallel.ForEachAsync(imageStreams, new ParallelOptions
            {
                MaxDegreeOfParallelism = maxParllelUploads,
                CancellationToken = cancellationToken
            },
            async (stream, ct) =>
            {
                var meta = new Google.Apis.Drive.v3.Data.File
                {
                    Name = fileName,
                    MimeType = "image/jpeg",
                    Parents = new List<string> { directoryId }
                };

                var request = driveService.Files.Create(meta, stream, "image/jpeg");
                request.Fields = "id, name, size";
                var file = await request.UploadAsync(ct);
                if (file.Status == Google.Apis.Upload.UploadStatus.Completed)
                {
                    ids.Add(new UploadFileData
                    {
                        Size = request.ResponseBody.Size,
                        Id = request.ResponseBody.Id,
                        FileName = request.ResponseBody.Name
                    });
                }
                else
                {
                    throw new Exception($"Error uploading file: {file.Exception}");
                }
            });

            return ids.ToList();
        }

        public Task<IDictionary<string, byte[]>> GetImagesAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFilesAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
