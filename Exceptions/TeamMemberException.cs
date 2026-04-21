using System.Net;

namespace Booking.Exceptions
{
    public class TeamMemberException : AppException
    {
        public TeamMemberException(string message, int statusCode)
            : base(message, statusCode) { }
    }

    public class InvalidTeamMemberRoleException : TeamMemberException
    {
        public InvalidTeamMemberRoleException(string role)
            : base($"Role '{role}' is not allowed for agency team members.", (int)HttpStatusCode.BadRequest) { }
    }

    public class TeamMemberNotFoundException : TeamMemberException
    {
        public TeamMemberNotFoundException(int userId)
            : base($"Team member with id '{userId}' was not found.", (int)HttpStatusCode.NotFound) { }
    }

    public class TeamMemberCreationFailedException : TeamMemberException
    {
        public TeamMemberCreationFailedException()
            : base("Something went Wrong ", (int)HttpStatusCode.BadRequest) { }
    }

    public class HotelAlreadyHasRoleException : TeamMemberException
    {
        public HotelAlreadyHasRoleException(int hotelId, string role)
            : base($"Hotel with id '{hotelId}' already has a user assigned to role '{role}'.", (int)HttpStatusCode.Conflict) { }
    }
}
