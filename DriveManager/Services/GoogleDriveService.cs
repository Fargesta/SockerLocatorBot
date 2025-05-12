using System.Collections.Concurrent;
using DriveManager.Dtos;
using DriveManager.Interfaces;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriveManager.Services
{
    internal sealed class GoogleDriveService(ILogger<GoogleDriveService> logger, DriveService driveService, IOptions<GoogleDriveOptions> opts) : IGoogleDriveService
    {
        private string directoryId => opts.Value.DirectoryId;
        private int maxRetryCount => 3;
        private TimeSpan retryDelay => TimeSpan.FromSeconds(2);

        public async Task<ConcurrentBag<UploadFileData>> UploadImagesAsync(IEnumerable<Stream> imageStreams,
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
                        logger.LogError($"Upload attempt {attempt} failed: {ex.Message}");
                        await Task.Delay(retryDelay, ct);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception and break
                        throw new Exception($"Error uploading file after {attempt} attempts: {ex.Message}", ex);
                    }
                }
            }).ConfigureAwait(false);

            return ids;
        }

        public async Task<ConcurrentBag<DownloadFileData>> GetImagesAsync(IEnumerable<string> fileIds, int maxParallelDownloads = 4, CancellationToken cancellationToken = default)
        {
            var result = new ConcurrentBag<DownloadFileData>();

            await Parallel.ForEachAsync(fileIds, new ParallelOptions
            {
                MaxDegreeOfParallelism = maxParallelDownloads,
                CancellationToken = cancellationToken
            },
            async (fileId, ct) =>
            {
                int attempt = 0;
                while (true)
                {
                    attempt++;
                    try
                    {
                        using var memoryStream = new MemoryStream();
                        var request = driveService.Files.Get(fileId);
                        var progress = await request.DownloadAsync(memoryStream, ct);

                        if (progress.Status == DownloadStatus.Completed)
                        {
                            var newDownload = new DownloadFileData
                            {
                                FileId = fileId,
                                Bytes = memoryStream.ToArray(),
                            };

                            result.Add(newDownload);
                            break;
                        }
                        else
                        {
                            throw progress.Exception ?? new Exception("Unknown download error");
                        }
                    }
                    catch (Exception ex) when (attempt < maxRetryCount)
                    {
                        logger.LogError($"Download attempt {attempt} failed: {ex.Message}");
                        await Task.Delay(retryDelay, ct);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error downloading file '{fileId}' after {attempt} attempts: {ex.Message}", ex);
                    }
                }
            }).ConfigureAwait(false);

            return result;
        }

        public async Task DeleteFilesAsync(IEnumerable<string> fileIds, int maxParralelDeletes = 4, CancellationToken cancellationToken = default)
        {
            await Parallel.ForEachAsync(fileIds, new ParallelOptions
            {
                MaxDegreeOfParallelism = maxParralelDeletes,
                CancellationToken = cancellationToken
            },
            async (fileId, ct) =>
            {
                int attempt = 0;
                while (true)
                {
                    attempt++;
                    try
                    {
                        var request = driveService.Files.Delete(fileId);
                        await request.ExecuteAsync(ct);
                        break;
                    }
                    catch (Exception ex) when (attempt < maxRetryCount)
                    {
                        logger.LogError($"Delete attempt {attempt} failed: {ex.Message}");
                        await Task.Delay(retryDelay, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error deleting file '{fileId}' after {attempt} attempts: {ex.Message}", ex);
                    }
                }
            }).ConfigureAwait(false);
        }
    }
}
