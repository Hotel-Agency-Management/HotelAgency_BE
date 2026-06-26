using Booking.Clients;
using Booking.Constants;
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
        IEmailJobService _emailJobService,
        IPaymentLogRepository _paymentLogRepository,
        IHotelRepository _hotelRepository,
        ILogger<ReservationService> _logger) : IReservationService
    {
        public async Task<ReservationResponse> CreateReservationAsync(int hotelId, int staffUserId, CreateReservationRequest request)
        {
            ValidateReservationDates(request.CheckInDate, request.CheckOutDate);

            var rooms = await ValidateAndFetchRoomsAsync(request.RoomNumbers, hotelId, request.CheckInDate, request.CheckOutDate);
            var insuranceAmount = request.HasInsurance ? rooms.Sum(r => r.InsurancePerReservation ?? 0m) : 0m;
            var days = request.CheckOutDate.DayNumber - request.CheckInDate.DayNumber;
            var totalAmount = rooms.Sum(r => r.DailyPrice) * days + insuranceAmount;

            var customerId = await _customerAccountService.EnsureCustomerAsync(
                request.Source, request.CustomerId, request.GuestEmail, request.GuestFullName, request.GuestPhone);

            var (contractPath, invoicePath) = await ValidateAndUploadFilesAsync(request.ContractFile, request.InvoiceFile);

            var saved = await SaveReservationAsync(new Reservation
            {
                HotelId = hotelId,
                CustomerId = customerId,
                Source = request.Source,
                Status = ReservationStatus.Confirmed,
                GuestFullName = request.GuestFullName,
                GuestEmail = request.GuestEmail,
                GuestPhone = request.GuestPhone,
                GuestIdNumber = request.GuestIdNumber ?? string.Empty,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfGuests = request.NumberOfGuests,
                NumberOfRooms = rooms.Count,
                ContractPath = contractPath,
                TotalAmount = totalAmount,
                HasInsurance = request.HasInsurance,
                InsuranceAmount = insuranceAmount,
                InvoicePath = invoicePath,
                SpecialRequests = request.SpecialRequests,
                Notes = request.Notes,
                CreatedById = staffUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ReservationRooms = rooms.Select(r => new ReservationRoom { RoomId = r.Id }).ToList()
            });

            var agencyId = (await _hotelRepository.GetByIdAsync(saved.HotelId))?.AgencyId;

            await _paymentLogRepository.CreateAsync(new PaymentLog
            {
                ReservationId = saved.Id,
                Amount = saved.TotalAmount,
                Type = PaymentType.Booking,
                Reason = PaymentReason.Booking,
                From = saved.CustomerId,
                To = null,
                HotelId = saved.HotelId,
                AgencyId = agencyId
            });

            if (saved.HasInsurance && saved.InsuranceAmount > 0)
                await _paymentLogRepository.CreateAsync(new PaymentLog
                {
                    ReservationId = saved.Id,
                    Amount = saved.InsuranceAmount,
                    Type = PaymentType.ReservationInsurance,
                    Reason = PaymentReason.ReservationInsurance,
                    From = saved.CustomerId,
                    To = null,
                    HotelId = saved.HotelId,
                    AgencyId = agencyId
                });

            await SendConfirmationEmailAsync(saved);
            _logger.LogInformation("Reservation {ReservationNumber} created for hotel {HotelId} by staff {StaffUserId}", saved.ReservationNumber, hotelId, staffUserId);
            return new ReservationResponse(saved);
        }

        public async Task<ReservationResponse> CreateMyReservationAsync(
            int hotelId, ApplicationUser user, CustomerCreateReservationRequest request)
        {
            ValidateReservationDates(request.CheckInDate, request.CheckOutDate);

            var rooms = await ValidateAndFetchRoomsAsync(request.RoomNumbers, hotelId, request.CheckInDate, request.CheckOutDate);
            var insuranceAmount = request.HasInsurance ? rooms.Sum(r => r.InsurancePerReservation ?? 0m) : 0m;
            var days = request.CheckOutDate.DayNumber - request.CheckInDate.DayNumber;
            var totalAmount = rooms.Sum(r => r.DailyPrice) * days + insuranceAmount;

            var (contractPath, invoicePath) = await ValidateAndUploadFilesAsync(request.ContractFile, request.InvoiceFile);

            var saved = await SaveReservationAsync(new Reservation
            {
                HotelId = hotelId,
                CustomerId = user.Id,
                Source = ReservationSource.Website,
                Status = ReservationStatus.Confirmed,
                GuestFullName = $"{user.FirstName} {user.LastName}".Trim(),
                GuestEmail = user.Email!,
                GuestPhone = user.PhoneNumber ?? string.Empty,
                GuestIdNumber = request.GuestIdNumber ?? string.Empty,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfGuests = request.NumberOfGuests,
                NumberOfRooms = rooms.Count,
                ContractPath = contractPath,
                TotalAmount = totalAmount,
                HasInsurance = request.HasInsurance,
                InsuranceAmount = insuranceAmount,
                InvoicePath = invoicePath,
                SpecialRequests = request.SpecialRequests,
                Notes = request.Notes,
                CreatedById = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ReservationRooms = rooms.Select(r => new ReservationRoom { RoomId = r.Id }).ToList()
            });

            var agencyId = (await _hotelRepository.GetByIdAsync(saved.HotelId))?.AgencyId;

            await _paymentLogRepository.CreateAsync(new PaymentLog
            {
                ReservationId = saved.Id,
                Amount = saved.TotalAmount,
                Type = PaymentType.Booking,
                Reason = PaymentReason.Booking,
                From = saved.CustomerId,
                To = null,
                HotelId = saved.HotelId,
                AgencyId = agencyId
            });

            if (saved.HasInsurance && saved.InsuranceAmount > 0)
                await _paymentLogRepository.CreateAsync(new PaymentLog
                {
                    ReservationId = saved.Id,
                    Amount = saved.InsuranceAmount,
                    Type = PaymentType.ReservationInsurance,
                    Reason = PaymentReason.ReservationInsurance,
                    From = saved.CustomerId,
                    To = null,
                    HotelId = saved.HotelId,
                    AgencyId = agencyId
                });

            await SendConfirmationEmailAsync(saved);
            _logger.LogInformation("Reservation {ReservationNumber} created by customer {CustomerId} for hotel {HotelId}", saved.ReservationNumber, user.Id, hotelId);
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
                Items = [.. reservations.Select(r => new ListReservationResponse(r))],
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<ReservationResponse> UpdateReservationAsync(
            int hotelId, int reservationId, int staffUserId, UpdateReservationRequest request)
        {
            var reservation = await _reservationRepository.GetByIdAndHotelIdAsync(reservationId, hotelId)
                ?? throw new ReservationNotFoundException(reservationId);

            if (reservation.Status == ReservationStatus.Cancelled)
                throw new BadRequestException(Messages.ReservationNotUpdatable);

            if (request.Source.HasValue) reservation.Source = request.Source.Value;
            if (request.GuestFullName is not null) reservation.GuestFullName = request.GuestFullName;

            return await ApplyUpdateAsync(reservation, staffUserId, request);
        }

        public async Task<CancellationResponse> CancelReservationAsync(
            int hotelId, int reservationId, CancelReservationRequest request)
        {
            var reservation = await _reservationRepository.GetByIdAndHotelIdAsync(reservationId, hotelId)
                ?? throw new ReservationNotFoundException(reservationId);

            return await ApplyCancellationAsync(reservation, request);
        }

        public async Task<ReservationResponse> UpdateReservationStatusAsync(
            int hotelId, int reservationId, UpdateReservationStatusRequest request)
        {
            var reservation = await _reservationRepository.GetByIdAndHotelIdAsync(reservationId, hotelId)
                ?? throw new ReservationNotFoundException(reservationId);

            var isValidTransition =
                (reservation.Status == ReservationStatus.Confirmed && request.Status == ReservationStatus.CheckedIn) ||
                (reservation.Status == ReservationStatus.CheckedIn && request.Status == ReservationStatus.CheckedOut);

            if (!isValidTransition)
                throw new BadRequestException(Messages.InvalidReservationStatusTransition);

            reservation.Status = request.Status;
            reservation.UpdatedAt = DateTime.UtcNow;

            var updated = await _reservationRepository.UpdateAsync(reservation);
            _logger.LogInformation("Reservation {ReservationId} status changed to {Status} for hotel {HotelId}", reservationId, request.Status, hotelId);

            return new ReservationResponse(updated);
        }

        public async Task<PaginatedResponse<ListReservationResponse>> GetMyReservationsAsync(
            int customerId, ReservationListRequest request)
        {
            var totalCount = await _reservationRepository.CountByCustomerIdAsync(customerId, request.Status);
            var reservations = await _reservationRepository.GetPagedByCustomerIdAsync(
                customerId, request.Status, request.PageNumber, request.PageSize);

            return new PaginatedResponse<ListReservationResponse>
            {
                Items = [.. reservations.Select(r => new ListReservationResponse(r))],
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<ReservationResponse> GetMyReservationByIdAsync(int reservationId, int customerId)
        {
            var reservation = await _reservationRepository.GetByIdAndCustomerIdAsync(reservationId, customerId)
                ?? throw new ReservationNotFoundException(reservationId);

            return new ReservationResponse(reservation);
        }

        public async Task<ReservationResponse> UpdateMyReservationAsync(
            int reservationId, int customerId, UpdateReservationRequest request)
        {
            var reservation = await _reservationRepository.GetByIdAndCustomerIdAsync(reservationId, customerId)
                ?? throw new ReservationNotFoundException(reservationId);

            if (reservation.Status != ReservationStatus.Pending &&
                reservation.Status != ReservationStatus.Confirmed)
                throw new BadRequestException(Messages.ReservationNotUpdatable);

            return await ApplyUpdateAsync(reservation, customerId, request);
        }

        public async Task<CancellationResponse> CancelMyReservationAsync(
            int reservationId, int customerId, CancelReservationRequest request)
        {
            var reservation = await _reservationRepository.GetByIdAndCustomerIdAsync(reservationId, customerId)
                ?? throw new ReservationNotFoundException(reservationId);

            return await ApplyCancellationAsync(reservation, request);
        }

        private async Task<ReservationResponse> ApplyUpdateAsync(
            Reservation reservation, int updatedById, UpdateReservationRequest request)
        {
            var newCheckIn = request.CheckInDate ?? reservation.CheckInDate;
            var newCheckOut = request.CheckOutDate ?? reservation.CheckOutDate;

            if (newCheckOut <= newCheckIn)
                throw new BadRequestException(Messages.InvalidCheckOutDate);

            var datesChanged = request.CheckInDate.HasValue || request.CheckOutDate.HasValue;
            if (datesChanged)
            {
                var roomIds = reservation.ReservationRooms.Select(rr => rr.RoomId);
                var unavailable = (await _reservationRepository.GetUnavailableRoomNumbersAsync(
                    roomIds, newCheckIn, newCheckOut, reservation.Id)).ToList();
                if (unavailable.Any())
                    throw new RoomsNotAvailableException(unavailable);
            }

            if (request.GuestPhone is not null) reservation.GuestPhone = request.GuestPhone;
            if (request.GuestIdNumber is not null) reservation.GuestIdNumber = request.GuestIdNumber;
            if (request.CheckInDate.HasValue) reservation.CheckInDate = request.CheckInDate.Value;
            var extraCharge = 0m;
            if (request.CheckOutDate.HasValue)
            {
                if (request.CheckOutDate.Value > reservation.CheckOutDate)
                {
                    var extraDays = request.CheckOutDate.Value.DayNumber - reservation.CheckOutDate.DayNumber;
                    extraCharge = reservation.ReservationRooms.Sum(rr => rr.Room?.ExtendPrice ?? 0m) * extraDays;
                    reservation.TotalAmount += extraCharge;
                }
                reservation.CheckOutDate = request.CheckOutDate.Value;
            }
            if (request.NumberOfGuests.HasValue) reservation.NumberOfGuests = request.NumberOfGuests.Value;
            if (request.SpecialRequests is not null) reservation.SpecialRequests = request.SpecialRequests;
            if (request.Notes is not null) reservation.Notes = request.Notes;
            if (request.HasInsurance.HasValue)
            {
                reservation.HasInsurance = request.HasInsurance.Value;
                reservation.InsuranceAmount = request.HasInsurance.Value
                    ? reservation.ReservationRooms.Sum(rr => rr.Room?.InsurancePerReservation ?? 0m)
                    : 0m;
            }

            reservation.UpdatedById = updatedById;
            reservation.UpdatedAt = DateTime.UtcNow;

            var updated = await _reservationRepository.UpdateAsync(reservation);

            if (extraCharge > 0m)
            {
                var extendAgencyId = (await _hotelRepository.GetByIdAsync(updated.HotelId))?.AgencyId;
                await _paymentLogRepository.CreateAsync(new PaymentLog
                {
                    ReservationId = updated.Id,
                    Amount = extraCharge,
                    Type = PaymentType.Extend,
                    Reason = PaymentReason.Extend,
                    From = updated.CustomerId,
                    To = null,
                    HotelId = updated.HotelId,
                    AgencyId = extendAgencyId
                });
            }

            return new ReservationResponse(updated);
        }

        private async Task<CancellationResponse> ApplyCancellationAsync(
            Reservation reservation, CancelReservationRequest request)
        {
            if (reservation.Status == ReservationStatus.Cancelled)
                throw new BadRequestException(Messages.ReservationAlreadyCancelled);

            if (reservation.Status == ReservationStatus.CheckedIn ||
                reservation.Status == ReservationStatus.CheckedOut)
                throw new InvalidStatusTransitionException(
                    reservation.Status.ToString(), ReservationStatus.Cancelled.ToString());

            if (!EnsureCancellable(reservation))
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var errorMessage = today > reservation.CheckOutDate
                    ? Messages.ReservationCannotBeCancelledAfterCheckOut
                    : Messages.ReservationCannotBeCancelledAfterCheckIn;
                throw new BadRequestException(errorMessage);
            }

            var isFree = DateTime.UtcNow.Date <
                reservation.CheckInDate.ToDateTime(TimeOnly.MinValue).AddDays(-3).Date;

            var fee = isFree
                ? 0m
                : reservation.TotalAmount * (reservation.Hotel!.CancellationFeePercentage / 100m);

            var originalTotal = reservation.TotalAmount;

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancelledAt = DateTime.UtcNow;
            reservation.CancellationFee = fee;
            reservation.TotalAmount = fee;
            reservation.IsFreeCancellation = isFree;
            reservation.CancellationReason = request.CancellationReason;
            reservation.UpdatedAt = DateTime.UtcNow;

            var updated = await _reservationRepository.UpdateAsync(reservation);

            var cancelAgencyId = reservation.Hotel?.AgencyId;

            if (fee > 0m)
            {
                await _paymentLogRepository.CreateAsync(new PaymentLog
                {
                    ReservationId = updated.Id,
                    Amount = fee,
                    Type = PaymentType.Cancellation,
                    Reason = PaymentReason.Cancellation,
                    From = updated.CustomerId,
                    To = null,
                    HotelId = updated.HotelId,
                    AgencyId = cancelAgencyId
                });
                await _paymentLogRepository.CreateAsync(new PaymentLog
                {
                    ReservationId = updated.Id,
                    Amount = originalTotal - fee,
                    Type = PaymentType.Refund,
                    Reason = PaymentReason.Refund,
                    From = null,
                    To = updated.CustomerId,
                    HotelId = updated.HotelId,
                    AgencyId = cancelAgencyId
                });
            }
            else
            {
                await _paymentLogRepository.CreateAsync(new PaymentLog
                {
                    ReservationId = updated.Id,
                    Amount = originalTotal,
                    Type = PaymentType.Refund,
                    Reason = PaymentReason.Refund,
                    From = null,
                    To = updated.CustomerId,
                    HotelId = updated.HotelId,
                    AgencyId = cancelAgencyId
                });
            }

            var message = fee == 0m ? Messages.FreeCancellationMessage : Messages.PaidCancellationMessage;
            return new CancellationResponse(updated, message);
        }

        private async Task<(string contractPath, string invoicePath)> ValidateAndUploadFilesAsync(
            IFormFile contractFile, IFormFile invoiceFile)
        {
            if (contractFile.ContentType != FileConstants.PdfContentType)
                throw new BadRequestException(Messages.ContractFileMustBePdf);

            if (invoiceFile.ContentType != FileConstants.PdfContentType)
                throw new BadRequestException(Messages.InvoiceFileMustBePdf);

            var contractPath = await _blobStorageService.UploadAsync(contractFile);
            var invoicePath = await _blobStorageService.UploadAsync(invoiceFile);

            return (contractPath, invoicePath);
        }

        private async Task SendConfirmationEmailAsync(Reservation saved)
        {
            var contractUrl = _blobStorageService.GetBlobUrl(saved.ContractPath!);
            var invoiceUrl = _blobStorageService.GetBlobUrl(saved.InvoicePath!);
            await _emailJobService.EnqueueReservationConfirmationEmailAsync(
                saved.GuestEmail, saved.GuestFullName, saved, contractUrl, invoiceUrl);
        }

        private static void ValidateReservationDates(DateOnly checkInDate, DateOnly checkOutDate)
        {
            if (checkInDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new BadRequestException(Messages.CheckInDateInThePast);

            if (checkOutDate <= checkInDate)
                throw new BadRequestException(Messages.InvalidCheckOutDate);
        }

        private async Task<List<Room>> ValidateAndFetchRoomsAsync(
            List<string> roomNumbers, int hotelId, DateOnly checkIn, DateOnly checkOut)
        {
            var rooms = (await _roomRepository.GetByRoomNumbersAndHotelIdAsync(roomNumbers, hotelId)).ToList();

            var foundNumbers = rooms.Select(r => r.RoomNumber).ToHashSet();
            var notFound = roomNumbers.Where(n => !foundNumbers.Contains(n)).ToList();
            if (notFound.Any())
                throw new BadRequestException($"The following room numbers were not found in this hotel: {string.Join(", ", notFound)}.");

            var notAvailable = rooms.Where(r => r.Status != RoomStatus.Available)
                                    .Select(r => r.RoomNumber).ToList();
            if (notAvailable.Any())
                throw new BadRequestException(
                    string.Format(Messages.RoomsNotAvailableStatus, string.Join(", ", notAvailable)));

            var unavailable = (await _reservationRepository.GetUnavailableRoomNumbersAsync(
                rooms.Select(r => r.Id), checkIn, checkOut)).ToList();
            if (unavailable.Any())
                throw new RoomsNotAvailableException(unavailable);

            return rooms;
        }

        private async Task<Reservation> SaveReservationAsync(Reservation reservation)
        {
            var year = DateTime.UtcNow.Year;
            var count = await _reservationRepository.CountByYearAsync(year);
            reservation.ReservationNumber = $"RES-{year}-{(count + 1):D6}";

            try
            {
                return await _reservationRepository.CreateAsync(reservation);
            }
            catch (DbUpdateException)
            {
                _logger.LogWarning("Reservation number collision for year {Year}, retrying with fresh count", year);
                var freshCount = await _reservationRepository.CountByYearAsync(year);
                reservation.ReservationNumber = $"RES-{year}-{(freshCount + 1):D6}";
                return await _reservationRepository.CreateAsync(reservation);
            }
        }

        public async Task<IReadOnlyList<BookingTypeDistributionItem>> GetAgencyBookingTypeDistributionAsync(int agencyId)
        {
            var from = DateTime.UtcNow.AddMonths(DashboardConstants.AgencyTrendMonthsBack);
            var raw = (await _reservationRepository.GetBookingSourceDistributionByAgencyAsync(agencyId, from))
                        .ToDictionary(x => x.Source, x => x.Count);

            var allSources = new[]
            {
                (ReservationSource.Website, BookingSourceLabels.Online),
                (ReservationSource.OTA,     BookingSourceLabels.OTA),
                (ReservationSource.Phone,   BookingSourceLabels.Phone),
                (ReservationSource.WalkIn,  BookingSourceLabels.WalkIn),
            };

            var total = raw.Values.Sum();

            return allSources.Select(s =>
            {
                raw.TryGetValue(s.Item1, out var count);
                return new BookingTypeDistributionItem
                {
                    Type = s.Item2,
                    Count = count,
                    Percentage = total == 0 ? 0 : Math.Round(count / (decimal)total * DashboardConstants.PercentageMultiplier, DashboardConstants.DecimalPlaces)
                };
            }).ToList();
        }

        public async Task<IReadOnlyList<ReservationStatusDistributionItem>> GetAgencyStatusDistributionAsync(int agencyId)
        {
            var from = DateTime.UtcNow.AddMonths(DashboardConstants.AgencyTrendMonthsBack);
            var raw = (await _reservationRepository.GetStatusDistributionByAgencyAsync(agencyId, from))
                        .ToDictionary(x => x.Status, x => x.Count);

            var allStatuses = new[]
            {
                ReservationStatus.Pending,
                ReservationStatus.Confirmed,
                ReservationStatus.CheckedIn,
                ReservationStatus.CheckedOut,
                ReservationStatus.Cancelled,
            };

            var total = raw.Values.Sum();

            return allStatuses.Select(s =>
            {
                raw.TryGetValue(s, out var count);
                return new ReservationStatusDistributionItem
                {
                    Status = s.ToString(),
                    Count = count,
                    Percentage = total == 0 ? 0 : Math.Round(count / (decimal)total * 100, 2)
                };
            }).ToList();
        }

        public async Task<AgencyReservationStats> GetAgencyStatsAsync(int agencyId)
        {
            var total = await _reservationRepository.GetTotalCountByAgencyAsync(agencyId);
            var pending = await _reservationRepository.GetPendingCountByAgencyAsync(agencyId);
            var avgValue = await _reservationRepository.GetAverageValueByAgencyAsync(agencyId);

            return new AgencyReservationStats
            {
                TotalBookings = total,
                PendingCount = pending,
                AverageBookingValue = avgValue
            };
        }

        public async Task<IReadOnlyList<RoomTypeReservationsItem>> GetAgencyReservationsByRoomTypeAsync(int agencyId)
        {
            var from = DateTime.UtcNow.AddMonths(DashboardConstants.AgencyTrendMonthsBack);
            var rows = await _reservationRepository.GetReservationsByRoomTypeForAgencyAsync(agencyId, from);

            return rows.Select(r => new RoomTypeReservationsItem
            {
                RoomTypeId = r.RoomTypeId,
                RoomTypeName = r.RoomTypeName,
                ReservationsCount = r.Count
            }).ToList();
        }

        public async Task<PropertyManagerOverviewCardsResponse> GetHotelOverviewCardsAsync(int hotelId)
        {
            var cards = await _reservationRepository.GetHotelOverviewCardsAsync(hotelId);
            return new PropertyManagerOverviewCardsResponse
            {
                TotalReservations = cards.TotalReservations,
                TodayCheckIns = cards.TodayCheckIns,
                TodayCheckOuts = cards.TodayCheckOuts,
                PendingReservations = cards.PendingReservations
            };
        }

        public async Task<HotelRevenueTrendResponse> GetHotelRevenueTrendAsync(int hotelId, string groupBy)
        {
            if (groupBy == DashboardConstants.GroupByDaily)
            {
                var from = DateTime.UtcNow.AddDays(DashboardConstants.DailyTrendDaysBack).Date;
                var to = DateTime.UtcNow.AddDays(DashboardConstants.DailyTrendDaysForward).Date;

                var raw = (await _reservationRepository.GetDailyRevenueByHotelAsync(hotelId, from, to))
                            .ToDictionary(x => new DateOnly(x.Year, x.Month, x.Day), x => x.Revenue);

                var items = Enumerable.Range(0, DashboardConstants.DailyTrendDaysCount).Select(i =>
                {
                    var date = DateOnly.FromDateTime(from).AddDays(i);
                    raw.TryGetValue(date, out var revenue);
                    return new RevenueTrendItem { Label = date.ToString(DashboardConstants.DailyLabelFormat), Revenue = revenue };
                }).ToList();

                return new HotelRevenueTrendResponse { GroupBy = DashboardConstants.GroupByDaily, Items = items };
            }
            else
            {
                var from = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, RevenueReportConstants.FirstDayOfMonth).AddMonths(RevenueReportConstants.MonthsBack);
                var to = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, RevenueReportConstants.FirstDayOfMonth).AddMonths(RevenueReportConstants.MonthsForward);

                var raw = (await _reservationRepository.GetMonthlyRevenueByHotelAsync(hotelId, from, to))
                            .ToDictionary(x => (x.Year, x.Month), x => x.Revenue);

                var items = Enumerable.Range(0, RevenueReportConstants.PeriodLengthMonths).Select(i =>
                {
                    var date = from.AddMonths(i);
                    raw.TryGetValue((date.Year, date.Month), out var revenue);
                    return new RevenueTrendItem { Label = date.ToString(RevenueReportConstants.MonthFormat), Revenue = revenue };
                }).ToList();

                return new HotelRevenueTrendResponse { GroupBy = DashboardConstants.GroupByMonthly, Items = items };
            }
        }

        public async Task<HotelBookingTypeDistributionResponse> GetHotelBookingTypeDistributionAsync(int hotelId)
        {
            var raw = (await _reservationRepository.GetBookingTypeDistributionByHotelIdAsync(hotelId))
                        .ToDictionary(x => x.Source, x => x.Count);

            var allSources = new[]
            {
                (ReservationSource.Website, BookingSourceLabels.Online),
                (ReservationSource.OTA,     BookingSourceLabels.OTA),
                (ReservationSource.Phone,   BookingSourceLabels.Phone),
                (ReservationSource.WalkIn,  BookingSourceLabels.WalkIn),
            };

            var total = raw.Values.Sum();

            var items = allSources.Select(s =>
            {
                raw.TryGetValue(s.Item1, out var count);
                return new BookingTypeDistributionItem
                {
                    Type = s.Item2,
                    Count = count,
                    Percentage = total == 0 ? 0 : Math.Round(count / (decimal)total * DashboardConstants.PercentageMultiplier, DashboardConstants.DecimalPlaces)
                };
            }).ToList();

            return new HotelBookingTypeDistributionResponse
            {
                TotalReservations = total,
                Items = items
            };
        }

        public async Task<HotelReservationStatusDistributionResponse> GetHotelReservationStatusDistributionAsync(int hotelId)
        {
            var raw = (await _reservationRepository.GetStatusDistributionByHotelIdAsync(hotelId))
                        .ToDictionary(x => x.Status, x => x.Count);

            var allStatuses = new[]
            {
                ReservationStatus.Pending,
                ReservationStatus.Confirmed,
                ReservationStatus.CheckedIn,
                ReservationStatus.CheckedOut,
                ReservationStatus.Cancelled,
            };

            var total = raw.Values.Sum();

            var items = allStatuses.Select(s =>
            {
                raw.TryGetValue(s, out var count);
                return new ReservationStatusDistributionItem
                {
                    Status = s.ToString(),
                    Count = count,
                    Percentage = total == 0 ? 0 : Math.Round(count / (decimal)total * DashboardConstants.PercentageMultiplier, DashboardConstants.DecimalPlaces)
                };
            }).ToList();

            return new HotelReservationStatusDistributionResponse
            {
                TotalReservations = total,
                Items = items
            };
        }

        public async Task<IReadOnlyList<InsuranceIncomePerBookingTrendItem>> GetHotelInsuranceIncomePerBookingTrendAsync(int hotelId)
        {
            var from = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, RevenueReportConstants.FirstDayOfMonth).AddMonths(RevenueReportConstants.MonthsBack);
            var to = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, RevenueReportConstants.FirstDayOfMonth).AddMonths(RevenueReportConstants.MonthsForward);

            var raw = (await _reservationRepository.GetMonthlyInsuranceIncomeByHotelAsync(hotelId, from, to))
                        .ToDictionary(x => (x.Year, x.Month), x => (x.TotalInsurance, x.TotalReservations));

            return Enumerable.Range(0, RevenueReportConstants.PeriodLengthMonths).Select(i =>
            {
                var date = from.AddMonths(i);
                raw.TryGetValue((date.Year, date.Month), out var entry);
                var value = entry.TotalReservations > 0
                    ? Math.Round(entry.TotalInsurance / entry.TotalReservations, DashboardConstants.DecimalPlaces)
                    : 0m;
                return new InsuranceIncomePerBookingTrendItem
                {
                    Month = date.ToString(DashboardConstants.MonthKeyFormat),
                    Value = value
                };
            }).ToList();
        }

        private static bool EnsureCancellable(Reservation reservation)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (today > reservation.CheckOutDate) return false;
            if (today >= reservation.CheckInDate) return false;
            return true;
        }
    }
}
