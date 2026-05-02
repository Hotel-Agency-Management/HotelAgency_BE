using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Booking.Clients
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration config)
        {
            _blobServiceClient = new BlobServiceClient(
                config["AzureBlobStorage:ConnectionString"]);
            _containerName = config["AzureBlobStorage:ContainerName"]!;
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

            return uniqueFileName;
        }

        public async Task DeleteAsync(string fileName)
        {
            var containerClient = _blobServiceClient
                .GetBlobContainerClient(_containerName);

            await containerClient.DeleteBlobIfExistsAsync(fileName);
        }

        public string GetBlobUrl(string blobName)
        {
            return _blobServiceClient
                .GetBlobContainerClient(_containerName)
                .GetBlobClient(blobName)
                .Uri.ToString();
        }
    }
}
