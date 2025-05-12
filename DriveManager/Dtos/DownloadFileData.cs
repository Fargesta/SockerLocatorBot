namespace DriveManager.Dtos
{
    public class DownloadFileData
    {
        public string FileId { get; set; } = default!;
        public byte[] Bytes { get; set; } = default!;
    }
}
