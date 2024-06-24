

namespace Application.Comments.Services;

public interface IFileUploadService
{
    Task<bool> DeleteAsync(string containerName, string fileName, CancellationToken cancellationToken = default);
    Task<Stream> GetAsync(string containerName, string fileName, CancellationToken cancellationToken = default);
    Task<bool> UploadAsync(string containerName, string fileName, byte[] fileContent, CancellationToken cancellationToken = default);
}