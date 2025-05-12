using DbManager.Models;
using SockerLocatorBot.Dtos;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Interfaces
{
    internal interface IImageService
    {
        Task<List<ImageModel>> CreateImageAsync(Update update, LocationModel location, LocationState locationState, CancellationToken cancellationToken);
        Task<List<byte[]>> DowloadImagesAsync(IList<ImageModel> images, CancellationToken cancellationToken);
    }
}