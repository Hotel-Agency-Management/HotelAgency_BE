using System.Net;

namespace Booking.Exceptions
{
    public class TeamManagementException : AppException
    {
        public TeamManagementException(string message, int statusCode)
            : base(message, statusCode) { }
    }

    public class InvalidTeamMemberRoleException : TeamManagementException
    {
        public InvalidTeamMemberRoleException(string role)
            : base($"Role '{role}' is not allowed for agency team members.", (int)HttpStatusCode.BadRequest) { }
    }

    public class TeamMemberNotFoundException : TeamManagementException
    {
        public TeamMemberNotFoundException(int userId)
            : base($"Team member with id '{userId}' was not found.", (int)HttpStatusCode.NotFound) { }
    }

    public class TeamMemberCreationFailedException : TeamManagementException
    {
        public TeamMemberCreationFailedException()
            : base("Something went Wrong ", (int)HttpStatusCode.BadRequest) { }
    }

    public class TeamMemberUpdateFailedException : TeamManagementException
    {
        public TeamMemberUpdateFailedException()
            : base("Failed to update team member.", (int)HttpStatusCode.BadRequest) { }
    }
}
