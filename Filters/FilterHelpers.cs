
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Filters
{
    internal static class FilterHelpers
    {
        internal static bool TryGetId(ActionExecutingContext context, string key, string errorMessage, out int id)
        {
            if (context.ActionArguments.TryGetValue(key, out var value) && value is int parsedId)
            {
                id = parsedId;
                return true;
            }

            context.Result = new BadRequestObjectResult(errorMessage);
            id = 0;
            return false;
        }

        internal static NotFoundObjectResult NotFound(string message) =>
            new(new { statusCode = 404, message });

        internal static ObjectResult Forbidden(string message) =>
            new(new { statusCode = 403, message }) { StatusCode = 403 };
    }
}
