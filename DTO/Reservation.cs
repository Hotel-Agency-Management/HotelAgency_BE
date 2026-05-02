using System.ComponentModel.DataAnnotations;
using Booking.Enums;
using Booking.Models;

namespace Booking.DTO
{
    public class CreateReservationRequest
    {
        [Required] public int RoomId { get; set; }
        public int? CustomerId { get; set; }
        [Required] public ReservationSource Source { get; set; }
        [Required] public string GuestFullName { get; set; } = string.Empty;
        [Required][EmailAddress] public string GuestEmail { get; set; } = string.Empty;
        [Required] public string GuestPhone { get; set; } = string.Empty;
        public string? GuestIdNumber { get; set; }
        [Required] public DateOnly CheckInDate { get; set; }
        [Required] public DateOnly CheckOutDate { get; set; }
        [Range(1, int.MaxValue)] public int NumberOfGuests { get; set; }
        [Range(1, int.MaxValue)] public int NumberOfRooms { get; set; }
        public string? SpecialRequests { get; set; }
        public string? Notes { get; set; }
        [Required] public IFormFile ContractFile { get; set; } = null!;
        [Required] public IFormFile InvoiceFile { get; set; } = null!;
    }

    public class UpdateReservationRequest
    {
        public int? CustomerId { get; set; }
        public ReservationSource? Source { get; set; }
        public string? GuestFullName { get; set; }
        [EmailAddress] public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }
        public string? GuestIdNumber { get; set; }
        public DateOnly? CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }
        [Range(1, int.MaxValue)] public int? NumberOfGuests { get; set; }
        [Range(1, int.MaxValue)] public int? NumberOfRooms { get; set; }
        public string? SpecialRequests { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateReservationStatusRequest
    {
        [Required] public ReservationStatus Status { get; set; }
    }

    public class ReservationResponse
    {
        public int Id { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public int HotelId { get; set; }
        public string? HotelName { get; set; }
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public int? CustomerId { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string GuestFullName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public string? GuestIdNumber { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }
        public string? ContractUrl { get; set; }
        public string? InvoiceUrl { get; set; }
        public string? SpecialRequests { get; set; }
        public string? Notes { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ReservationResponse(Reservation r, string? contractUrl = null, string? invoiceUrl = null)
        {
            Id = r.Id;
            ReservationNumber = r.ReservationNumber;
            HotelId = r.HotelId;
            HotelName = r.Hotel?.Name;
            RoomId = r.RoomId;
            RoomNumber = r.Room?.RoomNumber;
            CustomerId = r.CustomerId;
            Source = r.Source.ToString();
            Status = r.Status.ToString();
            GuestFullName = r.GuestFullName;
            GuestEmail = r.GuestEmail;
            GuestPhone = r.GuestPhone;
            GuestIdNumber = r.GuestIdNumber;
            CheckInDate = r.CheckInDate;
            CheckOutDate = r.CheckOutDate;
            NumberOfGuests = r.NumberOfGuests;
            NumberOfRooms = r.NumberOfRooms;
            ContractUrl = contractUrl;
            InvoiceUrl = invoiceUrl;
            SpecialRequests = r.SpecialRequests;
            Notes = r.Notes;
            CreatedById = r.CreatedById;
            UpdatedById = r.UpdatedById;
            CreatedAt = r.CreatedAt;
            UpdatedAt = r.UpdatedAt;
        }
    }

    public class ListReservationResponse
    {
        public int Id { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public string? RoomNumber { get; set; }
        public string GuestFullName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public ListReservationResponse(Reservation r)
        {
            Id = r.Id;
            ReservationNumber = r.ReservationNumber;
            RoomNumber = r.Room?.RoomNumber;
            GuestFullName = r.GuestFullName;
            Status = r.Status.ToString();
            CheckInDate = r.CheckInDate;
            CheckOutDate = r.CheckOutDate;
            CreatedAt = r.CreatedAt;
        }
    }
}
