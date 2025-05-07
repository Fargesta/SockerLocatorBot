namespace DriveManager.Dtos
{
    public class UploadFileData
    {
        public string FileName { get; set; } = default!;
        public string Id { get; set; } = default!;
        public long? Size { get; set; } = 0;

        public bool IsValid => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(FileName) && Size > 0;
    }
}
