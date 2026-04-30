using Booking.Clients;
using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Booking.Configurations;
using Microsoft.Extensions.Options;
namespace Booking.Services
{
    public class TeamManagementService(
        ITeamManagementRepository _teamMemberRepository,
        IAgencyRepository _agencyRepository,
        IAuthRepository _authRepository,
        IEmailJobService _emailJobService,
        IOptions<AuthSettings> authSettings,
        IEmailVerificationService _emailVerificationService) : ITeamManagementService
    {
        private readonly AuthSettings _authSettings = authSettings.Value;

        public async Task<PaginatedResponse<TeamMemberResponse>> GetAgencyTeamMembersAsync(
            int agencyId,
            int? excludedUserId,
            TeamMemberListRequest request)
        {
            var totalCount = await _teamMemberRepository.CountByAgencyAsync(agencyId, excludedUserId);
            var users = await _teamMemberRepository.GetByAgencyAsync(
                agencyId,
                excludedUserId,
                request.PageNumber,
                request.PageSize);

            var items = new List<TeamMemberResponse>();
            foreach (var user in users)
            {
                var role = await _teamMemberRepository.GetRoleAsync(user);
                items.Add(new TeamMemberResponse(user, role));
            }

            return new PaginatedResponse<TeamMemberResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<TeamMemberResponse> CreateAgencyTeamMemberAsync(
            int agencyId,
            CreateTeamMemberRequest request)
        {
            var role = TeamMemberUtils.NormalizeAndValidateRole(request.Role);
            var email = request.Email.Trim();

            if (await _teamMemberRepository.FindByEmailAsync(email) is not null)
                throw new EmailAlreadyExistsException(email);

            var agency = await _agencyRepository.GetByIdAsync(agencyId)
                ?? throw new AgencyNotFoundException(agencyId);

            var user = new ApplicationUser
            {
                AgencyId = agencyId,
                HotelId = null,
                UserName = email,
                Email = email,
                EmailConfirmed = false,
                PhoneNumber = request.PhoneNumber.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createResult = await _teamMemberRepository.CreateAsync(user, _authSettings.DefaultPassword);
            if (!createResult.Succeeded)
                throw new TeamMemberCreationFailedException();

            var roleResult = await _teamMemberRepository.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                throw new TeamMemberCreationFailedException();

            var link = await _emailVerificationService.GenerateVerificationLinkAsync(user);
            await _emailJobService.EnqueueTeamMemberVerificationEmailAsync(user, agency, link, _authSettings.DefaultPassword);

            return new TeamMemberResponse(user, role);
        }

        public async Task<TeamMemberResponse> AssignAgencyTeamMemberRoleAsync(
            int agencyId,
            int teamMemberId,
            AssignTeamMemberRoleRequest request)
        {
            var role = TeamMemberUtils.NormalizeAndValidateRole(request.Role);

            var teamMember = await _teamMemberRepository.GetByIdAndAgencyAsync(teamMemberId, agencyId)
                ?? throw new TeamMemberNotFoundException(teamMemberId);

            var result = await _teamMemberRepository.ReplaceRoleAsync(teamMember, role);
            if (!result.Succeeded)
                throw new TeamMemberCreationFailedException();

            teamMember.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateUserAsync(teamMember);

            return new TeamMemberResponse(teamMember, role);
        }
    }
}
