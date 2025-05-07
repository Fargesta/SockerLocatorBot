using NetTopologySuite.Geometries;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Dtos
{
    internal class AddLocationDto
    {
        public string ChatId { get; set; } = default!;
        public Point Location { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public List<TGFile> Photos { get; set; } = new();
        public string? SocketType { get; set; } = default!;
        public string? Description { get; set; } = default!;
    }
}
