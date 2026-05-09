using Booking.Clients;
using Booking.DTO;
using Booking.Enums;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Services
{
    public class ReservationService(
        IReservationRepository _reservationRepository,
        IRoomRepository _roomRepository,
        ICustomerAccountService _customerAccountService,
        IBlobStorageService _blobStorageService,
        IEmailJobService _emailJobService) : IReservationService
    {
        public async Task<ReservationResponse> CreateReservationAsync(int hotelId, int staffUserId, CreateReservationRequest request)
        {
            if (request.ContractFile.ContentType != FileConstants.PdfContentType)
                throw new BadRequestException(Messages.ContractFileMustBePdf);

            if (request.InvoiceFile.ContentType != FileConstants.PdfContentType)
                throw new BadRequestException(Messages.InvoiceFileMustBePdf);

            var rooms = (await _roomRepository.GetByRoomNumbersAndHotelIdAsync(request.RoomNumbers, hotelId)).ToList();

            var foundNumbers = rooms.Select(r => r.RoomNumber).ToHashSet();
            var notFound = request.RoomNumbers.Where(n => !foundNumbers.Contains(n)).ToList();
            if (notFound.Any())
                throw new BadRequestException($"The following room numbers were not found in this hotel: {string.Join(", ", notFound)}.");

            if (request.CheckOutDate <= request.CheckInDate)
                throw new BadRequestException(Messages.InvalidCheckOutDate);

            var unavailable = (await _reservationRepository.GetUnavailableRoomNumbersAsync(
                rooms.Select(r => r.Id), request.CheckInDate, request.CheckOutDate)).ToList();
            if (unavailable.Any())
                throw new RoomsNotAvailableException(unavailable);

            var customerId = await _customerAccountService.EnsureCustomerAsync(
                request.Source, request.CustomerId, request.GuestEmail, request.GuestFullName, request.GuestPhone);

            var contractPath = await _blobStorageService.UploadAsync(request.ContractFile);
            var invoicePath = await _blobStorageService.UploadAsync(request.InvoiceFile);

            var year = DateTime.UtcNow.Year;
            var count = await _reservationRepository.CountByYearAsync(year);

            var reservation = new Reservation
            {
                ReservationNumber = $"RES-{year}-{(count + 1):D6}",
                HotelId = hotelId,
                CustomerId = customerId,
                Source = request.Source,
                Status = ReservationStatus.Confirmed,
                GuestFullName = request.GuestFullName,
                GuestEmail = request.GuestEmail,
                GuestPhone = request.GuestPhone,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfGuests = request.NumberOfGuests,
                NumberOfRooms = rooms.Count,
                ContractPath = contractPath,
                TotalAmount = request.TotalAmount,
                InvoicePath = invoicePath,
                SpecialRequests = request.SpecialRequests,
                Notes = request.Notes,
                CreatedById = staffUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ReservationRooms = rooms.Select(r => new ReservationRoom { RoomId = r.Id }).ToList()
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

            return new ReservationResponse(saved);
        }

        public async Task<ReservationResponse> GetReservationByIdAsync(int hotelId, int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAndHotelIdAsync(reservationId, hotelId)
                ?? throw new ReservationNotFoundException(reservationId);

            return new ReservationResponse(reservation);
        }

        public async Task<PaginatedResponse<ListReservationResponse>> GetReservationsByHotelIdAsync(
            int hotelId, ReservationListRequest request)
        {
            var totalCount = await _reservationRepository.CountByHotelIdAsync(
                hotelId, request.Search, request.Status, request.CheckInFrom, request.CheckInTo);

            var reservations = await _reservationRepository.GetByHotelIdAsync(
                hotelId, request.Search, request.Status, request.CheckInFrom, request.CheckInTo,
                request.PageNumber, request.PageSize);

            return new PaginatedResponse<ListReservationResponse>
            {
                Items = [..reservations.Select(r => new ListReservationResponse(r))],
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
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
            if (datesChanged)
            {
                var roomIds = reservation.ReservationRooms.Select(rr => rr.RoomId);
                var unavailable = (await _reservationRepository.GetUnavailableRoomNumbersAsync(
                    roomIds, newCheckIn, newCheckOut, reservationId)).ToList();
                if (unavailable.Any())
                    throw new RoomsNotAvailableException(unavailable);
            }

            if (request.Source.HasValue) reservation.Source = request.Source.Value;
            if (request.GuestFullName is not null) reservation.GuestFullName = request.GuestFullName;
            if (request.GuestPhone is not null) reservation.GuestPhone = request.GuestPhone;
            if (request.GuestIdNumber is not null) reservation.GuestIdNumber = request.GuestIdNumber;
            if (request.CheckInDate.HasValue) reservation.CheckInDate = request.CheckInDate.Value;
            if (request.CheckOutDate.HasValue) reservation.CheckOutDate = request.CheckOutDate.Value;
            if (request.NumberOfGuests.HasValue) reservation.NumberOfGuests = request.NumberOfGuests.Value;
            if (request.SpecialRequests is not null) reservation.SpecialRequests = request.SpecialRequests;
            if (request.Notes is not null) reservation.Notes = request.Notes;

            reservation.UpdatedById = staffUserId;
            reservation.UpdatedAt = DateTime.UtcNow;

            var updated = await _reservationRepository.UpdateAsync(reservation);
            return new ReservationResponse(updated);
        }

    }
}
