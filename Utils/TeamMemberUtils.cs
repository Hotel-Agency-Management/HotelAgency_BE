using Booking.Constants;
using Booking.Exceptions;
using System.Text;

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
        var normalized = NormalizeRole(role);
        if (!AllowedAgencyRoles.Contains(normalized))
            throw new InvalidTeamMemberRoleException(role);
        return normalized;
    }

    private static string NormalizeRole(string role)
    {
        var trimmed = role.Trim();
        var builder = new StringBuilder(trimmed.Length + 4);

        for (var i = 0; i < trimmed.Length; i++)
        {
            var current = trimmed[i];
            if (current is '-' or ' ')
            {
                builder.Append('_');
                continue;
            }

            if (i > 0 && char.IsUpper(current) && char.IsLower(trimmed[i - 1]))
                builder.Append('_');

            builder.Append(char.ToUpperInvariant(current));
        }

        return builder.ToString();
    }
}
