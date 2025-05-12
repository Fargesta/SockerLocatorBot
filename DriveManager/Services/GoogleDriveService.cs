using System.Collections.Concurrent;
using DriveManager.Dtos;
using DriveManager.Interfaces;
using Google.Apis.Drive.v3;
using Microsoft.Extensions.Options;

namespace DriveManager.Services
{
    internal sealed class GoogleDriveService(ILogger<GoogleDriveService> logger, DriveService driveService, IOptions<GoogleDriveOptions> opts) : IGoogleDriveService
    {
        private string directoryId => opts.Value.DirectoryId;
        private int maxRetryCount => 3;
        private TimeSpan retryDelay => TimeSpan.FromSeconds(2);

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

                int attempt = 0;
                while (true)
                {
                    attempt++;
                    try
                    {

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
                            break;
                        }
                        else
                        {
                            throw file.Exception ?? new Exception($"Error uploading file: {file.Exception}");
                        }
                    }
                    catch (Exception ex) when (attempt < maxRetryCount)
                    {
                        // Log the exception and retry
                        logger($"Attempt {attempt} failed: {ex.Message}");
                        await Task.Delay(retryDelay, ct);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception and break
                        Console.WriteLine($"Failed to upload file after {attempt} attempts: {ex.Message}");
                        break;
                    }
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
