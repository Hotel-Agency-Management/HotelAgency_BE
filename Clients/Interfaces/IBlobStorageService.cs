namespace Booking.Clients
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteAsync(string blobUrl);
    }
}
