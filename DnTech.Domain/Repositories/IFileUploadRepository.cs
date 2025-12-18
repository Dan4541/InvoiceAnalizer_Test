namespace DnTech.Domain.Repositories
{
    public interface IFileUploadRepository
    {
        Task<FileUpload?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<FileUpload>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<FileUpload>> GetAllAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);
        Task<FileUpload> AddAsync(FileUpload fileUpload, CancellationToken cancellationToken = default);
        Task UpdateAsync(FileUpload fileUpload, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
