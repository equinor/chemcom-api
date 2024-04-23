
namespace Application.Comments.Services;

public interface IFileUploadService
{
    Task<bool> DeleteAsync(string containerName, string fileName);
    Task<bool> UploadAsync(string containerName, string fileName, byte[] fileContent);
}