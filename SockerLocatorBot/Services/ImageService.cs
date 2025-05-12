using DbManager;
using DbManager.Models;
using DriveManager.Dtos;
using DriveManager.Interfaces;
using Microsoft.Extensions.Options;
using SockerLocatorBot.Dtos;
using SockerLocatorBot.Helpers;
using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Services
{
    internal class ImageService(ILogger<ImageService> logger,
        IGoogleDriveService googleDrive,
        ITelegramBotClient botClient,
        PgContext pgContext,
        IOptionsSnapshot<AppSettings> appSettings) : IImageService
    {
        public async Task<List<ImageModel>> CreateImageAsync(Update update, LocationModel location, LocationState locationState, CancellationToken cancellationToken)
        {
            ChatId chatId = GetInfroFromUpdate.GetChatId(update);

            var result = new List<ImageModel>();

            var filePath = locationState.Photos.Last().FilePath;
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path is null");
            }

            var fileName = $"{locationState.SocketType}_{locationState.Location.X}_{locationState.Location.Y}_{DateTime.UtcNow}"
                .Replace(' ', '_')
                .Replace(':', '-')
                .Replace('/', '-');

            try
            {
                using var stream = new MemoryStream();
                await botClient.DownloadFile(filePath, stream, cancellationToken: cancellationToken);
                stream.Position = 0;

                var uploads = await googleDrive.UploadImagesAsync(new[] { stream }, fileName, 4, cancellationToken);

                if (uploads is null || uploads.Count == 0)
                {
                    throw new ArgumentNullException(nameof(uploads), "Upload is null or empty");
                }

                foreach (var upload in uploads)
                {
                    var newImage = new ImageModel
                    {
                        DriveFileId = upload.Id,
                        FileName = upload.FileName,
                        FileSize = upload.Size,
                        Description = locationState.ImageCaption,
                        LocationId = location.Id,
                        CreatedById = location.CreatedById,
                        UpdatedById = location.UpdatedById,
                        IsSaved = true,
                        Url = "https://drive.google.com/drive/folders/" + appSettings.Value.GoogleDrive.DirectoryId
                    };
                    pgContext.Images.Add(newImage);
                    result.Add(newImage);
                }
                await pgContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading file");
                throw;
            }
        }

        public async Task<List<DownloadFileData>> DowloadImagesAsync(IList<ImageModel> images, CancellationToken cancellationToken)
        {
            var ids = images.Select(x => x.DriveFileId).ToList();
            var downloads = await googleDrive.GetImagesAsync(ids, 4, cancellationToken);
            return downloads.ToList();  
        }
    }
}
