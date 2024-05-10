using Application.Comments.Services;
using Azure.Core;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BlobService;

public sealed class AzureFileUploadService : IFileUploadService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureFileUploadService> _logger;

    public AzureFileUploadService(IConfiguration configuration, ILogger<AzureFileUploadService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> UploadAsync(string containerName, string fileName, byte[] fileContent, CancellationToken cancellationToken = default)
    {
        try
        {
            BlobContainerClient containerClient = GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            var options = new BlobUploadOptions();
            await blobClient.UploadAsync(new BinaryData(fileContent), cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Attachment upload failed to the container ({containerName})", containerName);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string containerName, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            BlobContainerClient containerClient = GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Attachment delete failed in the container ({containerName})", containerName);
            throw;
        }
    }

    private BlobContainerClient GetBlobContainerClient(string containerName)
    {
        var storageSharedKeyCredentials = new StorageSharedKeyCredential(_configuration["AzureBlobAccount"], _configuration["AzureBlobKey"]);
        Uri uri = new Uri($"https://{_configuration["AzureBlobAccount"]}.blob.core.windows.net");
        BlobClientOptions blobClientOptions = new BlobClientOptions
        {
            Retry =
            {
                Delay = TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(5),
                MaxRetries = 4,
                Mode = RetryMode.Exponential,
                NetworkTimeout = TimeSpan.FromSeconds(100)
            }
        };

        var blobServiceClient = new BlobServiceClient(uri, storageSharedKeyCredentials, blobClientOptions);
        return blobServiceClient.GetBlobContainerClient(containerName);
    }
}
