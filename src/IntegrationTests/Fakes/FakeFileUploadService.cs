using Application.Comments.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Fakes;

public sealed class FakeFileUploadService : IFileUploadService
{
    public async Task<bool> DeleteAsync(string containerName, string fileName)
    {
        Task<bool> task = Task.Run(() =>
        {
            return true;
        });

        return await task;
    }

    public async Task<bool> UploadAsync(string containerName, string fileName, byte[] fileContent)
    {
        Task<bool> task = Task.Run(() =>
        {
            return true;
        });

        return await task;
    }
}
