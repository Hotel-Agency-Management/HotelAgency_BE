using Booking.Constants;
using Booking.Exceptions;

public static class TeamMemberUtils
{
    private static readonly HashSet<string> AllowedAgencyRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        Roles.PropertyManager,
        Roles.FrontDeskStaff,
        Roles.HousekeepingManager,
        Roles.HousekeepingEmployee,
        Roles.Accountant,
        Roles.CustomerSupport,
        Roles.Auditor
    };

    public static string NormalizeAndValidateRole(string role)
    {
        var normalized = role.Trim().ToUpperInvariant();
        if (!AllowedAgencyRoles.Contains(normalized))
            throw new InvalidTeamMemberRoleException(role);
        return normalized;
    }
}
