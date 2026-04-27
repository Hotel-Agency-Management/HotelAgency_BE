using Booking.Configurations;
using Booking.Constants;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Booking.Services
{
    public class AppLinkService(IOptions<AppLinksOptions> appLinksOptions) : IAppLinkService
    {
        private readonly AppLinksOptions _appLinksOptions = appLinksOptions.Value;

        public string Build(string path)
        {
            var normalizedBaseUrl = (_appLinksOptions.BaseUrl ?? string.Empty).Trim().TrimEnd('/');
            var normalizedPath = (path ?? string.Empty).Trim().TrimStart('/');

            if (string.IsNullOrWhiteSpace(normalizedBaseUrl))
                throw new InvalidOperationException("AppLinks:BaseUrl is not configured.");

            if (string.IsNullOrWhiteSpace(normalizedPath))
                return normalizedBaseUrl;

            return $"{normalizedBaseUrl}/{normalizedPath}";
        }

        public string BuildVerifyEmailLink(int userId, string token)
        {
            var baseLink = Build(AppLinkPaths.VerifyEmail);
            return QueryHelpers.AddQueryString(baseLink, new Dictionary<string, string?>
            {
                ["userId"] = userId.ToString(),
                ["token"] = token
            });
        }

        public string GetHelpLink() => Build(AppLinkPaths.Help);

        public string GetSupportLink() => Build(AppLinkPaths.Support);

        public string GetPrivacyLink() => Build(AppLinkPaths.Privacy);
    }
}
