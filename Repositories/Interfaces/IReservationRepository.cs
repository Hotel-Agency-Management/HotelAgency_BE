using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public record RoomTypeReservationCount(int RoomTypeId, string RoomTypeName, int Count);
    public record HotelOverviewCards(int TotalReservations, int TodayCheckIns, int TodayCheckOuts, int PendingReservations);

    public interface IReservationRepository
    {
        Task<Reservation> CreateAsync(Reservation reservation);
        Task<Reservation?> GetByIdAsync(int reservationId);
        Task<Reservation?> GetByIdAndHotelIdAsync(int reservationId, int hotelId);
        Task<IEnumerable<Reservation>> GetByHotelIdAsync(
            int hotelId, string? search, ReservationStatus? status,
            DateOnly? checkInFrom, DateOnly? checkInTo, int pageNumber, int pageSize);
        Task<int> CountByHotelIdAsync(
            int hotelId, string? search, ReservationStatus? status,
            DateOnly? checkInFrom, DateOnly? checkInTo);
        Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId);
        Task<Reservation?> GetByIdAndCustomerIdAsync(int reservationId, int customerId);
        Task<IEnumerable<Reservation>> GetPagedByCustomerIdAsync(int customerId, ReservationStatus? status, int pageNumber, int pageSize);
        Task<int> CountByCustomerIdAsync(int customerId, ReservationStatus? status);
        Task<IEnumerable<string>> GetUnavailableRoomNumbersAsync(IEnumerable<int> roomIds, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null);
        Task<int> CountByYearAsync(int year);
        Task<Reservation> UpdateAsync(Reservation reservation);

        // Agency overview stats
        Task<int> GetTotalCountByAgencyAsync(int agencyId);
        Task<int> GetPendingCountByAgencyAsync(int agencyId);
        Task<decimal> GetAverageValueByAgencyAsync(int agencyId);
        Task<IEnumerable<(ReservationSource Source, int Count)>> GetBookingSourceDistributionByAgencyAsync(int agencyId, DateTime from);
        Task<IEnumerable<(ReservationStatus Status, int Count)>> GetStatusDistributionByAgencyAsync(int agencyId, DateTime from);
        Task<IEnumerable<RoomTypeReservationCount>> GetReservationsByRoomTypeForAgencyAsync(int agencyId, DateTime from);
        Task<HotelOverviewCards> GetHotelOverviewCardsAsync(int hotelId);
        Task<IEnumerable<(ReservationStatus Status, int Count)>> GetStatusDistributionByHotelIdAsync(int hotelId);
        Task<IEnumerable<(ReservationSource Source, int Count)>> GetBookingTypeDistributionByHotelIdAsync(int hotelId);
        Task<IEnumerable<(int Year, int Month, decimal Revenue)>> GetMonthlyRevenueByHotelAsync(int hotelId, DateTime from, DateTime to);
        Task<IEnumerable<(int Year, int Month, int Day, decimal Revenue)>> GetDailyRevenueByHotelAsync(int hotelId, DateTime from, DateTime to);
        Task<IEnumerable<(int Year, int Month, int Day, decimal TotalInsurance, int TotalReservations)>> GetDailyInsuranceIncomeByHotelAsync(int hotelId, DateTime from, DateTime to);
        Task<IEnumerable<(int Year, int Month, decimal TotalInsurance, int TotalReservations)>> GetMonthlyInsuranceIncomeByHotelAsync(int hotelId, DateTime from, DateTime to);
    }
}
