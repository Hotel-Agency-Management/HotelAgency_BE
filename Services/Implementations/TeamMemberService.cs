using Booking.Clients;
using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Booking.Utils;

namespace Booking.Services
{
    public class TeamMemberService(
        ITeamMemberRepository _teamMemberRepository,
        IHotelRepository _hotelRepository,
        IAuthRepository _authRepository,
        IEmailJobService _emailJobService,
        IEmailVerificationService _emailVerificationService) : ITeamMemberService
    {
        public async Task<PaginatedResponse<TeamMemberResponse>> GetAgencyTeamMembersAsync(
            int agencyId,
            int hotelId,
            int agencyOwnerId,
            TeamMemberListRequest request)
        {
            var totalCount = await _teamMemberRepository.CountByHotelAsync(agencyId, hotelId, agencyOwnerId);
            var users = await _teamMemberRepository.GetByHotelAsync(
                agencyId,
                hotelId,
                agencyOwnerId,
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
            int hotelId,
            CreateTeamMemberRequest request)
        {
            var role = TeamMemberUtils.NormalizeAndValidateRole(request.Role);
            var email = request.Email.Trim();

            if (await _teamMemberRepository.FindByEmailAsync(email) is not null)
                throw new EmailAlreadyExistsException(email);

            var hotel = await _hotelRepository.GetByIdAndAgencyIdAsync(hotelId, agencyId)
                ?? throw new HotelNotFoundException(hotelId);

            await _teamMemberRepository.EnsureHotelDoesNotHaveSingleAssigneeRoleAsync(hotelId, role);

            var user = new ApplicationUser
            {
                AgencyId = agencyId,
                HotelId = hotelId,
                UserName = email,
                Email = email,
                EmailConfirmed = false,
                PhoneNumber = request.PhoneNumber.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createResult = await _teamMemberRepository.CreateAsync(user, AuthConstant.DefaultPassword);
            if (!createResult.Succeeded)
                throw new TeamMemberCreationFailedException();

            var roleResult = await _teamMemberRepository.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                throw new TeamMemberCreationFailedException();

            var link = await _emailVerificationService.GenerateVerificationLinkAsync(user);
            await _emailJobService.EnqueueTeamMemberVerificationEmailAsync(user, hotel, link, AuthConstant.DefaultPassword);

            return new TeamMemberResponse(user, role);
        }

        public async Task<TeamMemberResponse> AssignAgencyTeamMemberRoleAsync(
            int agencyId,
            int hotelId,
            int teamMemberId,
            AssignTeamMemberRoleRequest request)
        {
            var role = TeamMemberUtils.NormalizeAndValidateRole(request.Role);

            var teamMember = await _teamMemberRepository.GetByIdAndAgencyAsync(teamMemberId, agencyId)
                ?? throw new TeamMemberNotFoundException(teamMemberId);

            if (teamMember.HotelId != hotelId)
                throw new TeamMemberNotFoundException(teamMemberId);

            await _teamMemberRepository.EnsureHotelDoesNotHaveSingleAssigneeRoleAsync(hotelId, role, teamMemberId);

            var result = await _teamMemberRepository.ReplaceRoleAsync(teamMember, role);
            if (!result.Succeeded)
                throw new TeamMemberCreationFailedException();

            teamMember.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateUserAsync(teamMember);

            return new TeamMemberResponse(teamMember, role);
        }

    }
}
