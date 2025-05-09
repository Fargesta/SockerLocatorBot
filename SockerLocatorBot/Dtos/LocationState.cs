using NetTopologySuite.Geometries;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Dtos
{
    public enum LocationStateEnum
    {
        None,
        LocationShared,
        WaitingForImage,
        FindSocket,
        WaitingForType,
        WaitingForDescription
    }

    public class LocationState
    {
        public string ChatId { get; set; } = default!;
        public Point Location { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public List<TGFile> Photos { get; set; } = new();
        public string? SocketType { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public LocationStateEnum State { get; set; } = LocationStateEnum.None;
    }
}
