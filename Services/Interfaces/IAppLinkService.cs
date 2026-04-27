namespace Booking.Interfaces.Services
{
    public interface IAppLinkService
    {
        string Build(string path);
        string BuildVerifyEmailLink(int userId, string token);
        string GetHelpLink();
        string GetSupportLink();
        string GetPrivacyLink();
    }
}
