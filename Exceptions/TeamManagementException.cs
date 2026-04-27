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

    public class TeamMemberHotelMismatchException : TeamManagementException
    {
        public TeamMemberHotelMismatchException(int userId, int hotelId)
            : base($"Team member with id '{userId}' does not belong to source hotel with id '{hotelId}'.", (int)HttpStatusCode.BadRequest) { }
    }

    public class TeamMemberTransferSameHotelException : TeamManagementException
    {
        public TeamMemberTransferSameHotelException()
            : base("Source hotel and destination hotel must be different.", (int)HttpStatusCode.BadRequest) { }
    }

    public class TeamMemberTransferAgencyMismatchException : TeamManagementException
    {
        public TeamMemberTransferAgencyMismatchException(int sourceHotelId, int destinationHotelId)
            : base($"Source hotel with id '{sourceHotelId}' and destination hotel with id '{destinationHotelId}' must belong to the same agency.", (int)HttpStatusCode.BadRequest) { }
    }

    public class HotelAlreadyHasRoleException : TeamManagementException
    {
        public HotelAlreadyHasRoleException(int hotelId, string role)
            : base($"Hotel with id '{hotelId}' already has a user assigned to role '{role}'.", (int)HttpStatusCode.Conflict) { }
    }
}
