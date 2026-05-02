using Booking.Clients;
using Booking.Configurations;
using Booking.Constants;
using Booking.DTO;
using Booking.Enums;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Booking.Services
{
    public class ReservationService(
        IReservationRepository _reservationRepository,
        IRoomRepository _roomRepository,
        IAuthRepository _authRepository,
        IOptions<AuthSettings> authSettings,
        IEmailVerificationService _emailVerificationService,
        IBlobStorageService _blobStorageService,
        IEmailJobService _emailJobService) : IReservationService
    {
        private readonly AuthSettings _authSettings = authSettings.Value;

        public async Task<ReservationResponse> CreateReservationAsync(int hotelId, int staffUserId, CreateReservationRequest request)
        {
            if (request.ContractFile.ContentType != FileConstants.PdfContentType)
                throw new BadRequestException(Messages.ContractFileMustBePdf);

            if (request.InvoiceFile.ContentType != FileConstants.PdfContentType)
                throw new BadRequestException(Messages.InvoiceFileMustBePdf);

            var room = await _roomRepository.GetByIdAndHotelIdAsync(request.RoomId, hotelId)
                ?? throw new RoomNotFoundException(request.RoomId);

            if (request.CheckOutDate <= request.CheckInDate)
                throw new BadRequestException(Messages.InvalidCheckOutDate);

            if (await _reservationRepository.HasOverlappingReservationAsync(request.RoomId, request.CheckInDate, request.CheckOutDate))
                throw new RoomNotAvailableException();

            var customerId = await EnsureCustomerAsync(request);

            var contractPath = await _blobStorageService.UploadAsync(request.ContractFile);
            var invoicePath = await _blobStorageService.UploadAsync(request.InvoiceFile);

            var year = DateTime.UtcNow.Year;
            var count = await _reservationRepository.CountByYearAsync(year);
            var reservationNumber = $"RES-{year}-{(count + 1):D6}";

            var reservation = new Reservation
            {
                ReservationNumber = reservationNumber,
                HotelId = hotelId,
                RoomId = request.RoomId,
                CustomerId = customerId,
                Source = request.Source,
                Status = ReservationStatus.Pending,
                GuestFullName = request.GuestFullName,
                GuestEmail = request.GuestEmail,
                GuestPhone = request.GuestPhone,
                GuestIdNumber = request.GuestIdNumber,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfGuests = request.NumberOfGuests,
                NumberOfRooms = request.NumberOfRooms,
                ContractPath = contractPath,
                InvoicePath = invoicePath,
                SpecialRequests = request.SpecialRequests,
                Notes = request.Notes,
                CreatedById = staffUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            Reservation saved;
            try
            {
                saved = await _reservationRepository.CreateAsync(reservation);
            }
            catch (DbUpdateException)
            {
                var freshCount = await _reservationRepository.CountByYearAsync(year);
                reservation.ReservationNumber = $"RES-{year}-{(freshCount + 1):D6}";
                saved = await _reservationRepository.CreateAsync(reservation);
            }

            var contractUrl = _blobStorageService.GetBlobUrl(saved.ContractPath!);
            var invoiceUrl = _blobStorageService.GetBlobUrl(saved.InvoicePath!);

            await _emailJobService.EnqueueReservationConfirmationEmailAsync(
                saved.GuestEmail, saved.GuestFullName, saved, contractUrl, invoiceUrl);

            return new ReservationResponse(saved, contractUrl, invoiceUrl);
        }

        public async Task<ReservationResponse> GetReservationByIdAsync(int hotelId, int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAndHotelIdAsync(reservationId, hotelId)
                ?? throw new ReservationNotFoundException(reservationId);

            return BuildResponse(reservation);
        }

        public async Task<IEnumerable<ListReservationResponse>> GetReservationsByHotelIdAsync(int hotelId)
        {
            var reservations = await _reservationRepository.GetByHotelIdAsync(hotelId);
            return reservations.Select(r => new ListReservationResponse(r));
        }

        public async Task<ReservationResponse> UpdateReservationAsync(int hotelId, int reservationId, int staffUserId, UpdateReservationRequest request)
        {
            var reservation = await _reservationRepository.GetByIdAndHotelIdAsync(reservationId, hotelId)
                ?? throw new ReservationNotFoundException(reservationId);

            var newCheckIn = request.CheckInDate ?? reservation.CheckInDate;
            var newCheckOut = request.CheckOutDate ?? reservation.CheckOutDate;

            if (newCheckOut <= newCheckIn)
                throw new BadRequestException("Check-out date must be after check-in date.");

            var datesChanged = request.CheckInDate.HasValue || request.CheckOutDate.HasValue;
            if (datesChanged && await _reservationRepository.HasOverlappingReservationAsync(reservation.RoomId, newCheckIn, newCheckOut, reservationId))
                throw new RoomNotAvailableException();

            if (request.CustomerId.HasValue) reservation.CustomerId = request.CustomerId;
            if (request.Source.HasValue) reservation.Source = request.Source.Value;
            if (request.GuestFullName is not null) reservation.GuestFullName = request.GuestFullName;
            if (request.GuestEmail is not null) reservation.GuestEmail = request.GuestEmail;
            if (request.GuestPhone is not null) reservation.GuestPhone = request.GuestPhone;
            if (request.GuestIdNumber is not null) reservation.GuestIdNumber = request.GuestIdNumber;
            if (request.CheckInDate.HasValue) reservation.CheckInDate = request.CheckInDate.Value;
            if (request.CheckOutDate.HasValue) reservation.CheckOutDate = request.CheckOutDate.Value;
            if (request.NumberOfGuests.HasValue) reservation.NumberOfGuests = request.NumberOfGuests.Value;
            if (request.NumberOfRooms.HasValue) reservation.NumberOfRooms = request.NumberOfRooms.Value;
            if (request.SpecialRequests is not null) reservation.SpecialRequests = request.SpecialRequests;
            if (request.Notes is not null) reservation.Notes = request.Notes;

            reservation.UpdatedById = staffUserId;
            reservation.UpdatedAt = DateTime.UtcNow;

            var updated = await _reservationRepository.UpdateAsync(reservation);
            return BuildResponse(updated);
        }

        public async Task<ReservationResponse> UpdateReservationStatusAsync(int hotelId, int reservationId, ReservationStatus newStatus)
        {
            var reservation = await _reservationRepository.GetByIdAndHotelIdAsync(reservationId, hotelId)
                ?? throw new ReservationNotFoundException(reservationId);

            var allowedTransitions = new Dictionary<ReservationStatus, ReservationStatus[]>
            {
                [ReservationStatus.Pending] = [ReservationStatus.Confirmed, ReservationStatus.Cancelled],
                [ReservationStatus.Confirmed] = [ReservationStatus.CheckedIn, ReservationStatus.Cancelled],
                [ReservationStatus.CheckedIn] = [ReservationStatus.CheckedOut],
                [ReservationStatus.CheckedOut] = [],
                [ReservationStatus.Cancelled] = []
            };

            if (!allowedTransitions[reservation.Status].Contains(newStatus))
                throw new InvalidStatusTransitionException(reservation.Status.ToString(), newStatus.ToString());

            if (newStatus == ReservationStatus.Confirmed
                && await _reservationRepository.HasOverlappingReservationAsync(reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate, reservationId))
                throw new RoomNotAvailableException();

            reservation.Status = newStatus;
            reservation.UpdatedAt = DateTime.UtcNow;

            var updated = await _reservationRepository.UpdateAsync(reservation);
            return BuildResponse(updated);
        }

        private ReservationResponse BuildResponse(Reservation r)
        {
            var contractUrl = r.ContractPath is not null ? _blobStorageService.GetBlobUrl(r.ContractPath) : null;
            var invoiceUrl = r.InvoicePath is not null ? _blobStorageService.GetBlobUrl(r.InvoicePath) : null;
            return new ReservationResponse(r, contractUrl, invoiceUrl);
        }

        private async Task<int?> EnsureCustomerAsync(CreateReservationRequest request)
        {
            if (request.Source != ReservationSource.WalkIn && request.Source != ReservationSource.Phone)
                return request.CustomerId;

            var existing = await _authRepository.FindByEmailAsync(request.GuestEmail);
            if (existing is not null)
                return existing.Id;

            var nameParts = request.GuestFullName.Trim().Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var user = new ApplicationUser
            {
                UserName = request.GuestEmail,
                Email = request.GuestEmail,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = request.GuestPhone,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _authRepository.CreateUserAsync(user, _authSettings.DefaultPassword);
            if (!result.Succeeded)
                throw new BadRequestException("Failed to create customer account.");

            await _authRepository.AddToRoleAsync(user, Roles.Customer);

            var verifyLink = await _emailVerificationService.GenerateVerificationLinkAsync(user);
            await _emailJobService.EnqueueNewCustomerAccountEmailAsync(user, verifyLink, _authSettings.DefaultPassword);

            return user.Id;
        }
    }
}
