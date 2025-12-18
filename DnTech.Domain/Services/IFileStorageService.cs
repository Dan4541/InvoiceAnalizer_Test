namespace DnTech.Domain.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
        Task<Stream> DownloadFileAsync(string fileLocation, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string fileLocation, CancellationToken cancellationToken = default);
    }
}
