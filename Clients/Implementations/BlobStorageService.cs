using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Booking.Clients
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IConfiguration config, ILogger<BlobStorageService> logger)
        {
            _blobServiceClient = new BlobServiceClient(
                config["AzureBlobStorage:ConnectionString"]);
            _containerName = config["AzureBlobStorage:ContainerName"]!;
            _logger = logger;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient
                .GetBlobContainerClient(_containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

            _logger.LogInformation("Blob {FileName} uploaded to container {Container}", uniqueFileName, _containerName);
            return uniqueFileName;
        }

        public async Task DeleteAsync(string fileName)
        {
            var containerClient = _blobServiceClient
                .GetBlobContainerClient(_containerName);

            await containerClient.DeleteBlobIfExistsAsync(fileName);
            _logger.LogInformation("Blob {FileName} deleted from container {Container}", fileName, _containerName);
        }

        public string GetBlobUrl(string blobName)
        {
            _logger.LogDebug("Resolving blob URL for {BlobName}", blobName);
            return _blobServiceClient
                .GetBlobContainerClient(_containerName)
                .GetBlobClient(blobName)
                .Uri.ToString();
        }
    }
}
