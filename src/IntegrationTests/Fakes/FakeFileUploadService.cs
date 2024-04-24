using Application.Comments.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Fakes;

public sealed class FakeFileUploadService : IFileUploadService
{
    public async Task<bool> DeleteAsync(string containerName, string fileName, CancellationToken cancellationToken = default)
    {
        Task<bool> task = Task.Run(() =>
        {
            return true;
        }, cancellationToken);

        return await task;
    }

    public async Task<bool> UploadAsync(string containerName, string fileName, byte[] fileContent, CancellationToken cancellationToken = default)
    {
        Task<bool> task = Task.Run(() =>
        {
            return true;
        }, cancellationToken);

        return await task;
    }
}
