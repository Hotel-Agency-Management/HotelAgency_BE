using System.ComponentModel.DataAnnotations;
using Booking.Enums;
using Booking.Models;

namespace Booking.DTO
{
    public class CreateReservationRequest
    {
        [Required][MinLength(1)] public List<string> RoomNumbers { get; set; } = [];
        public int? CustomerId { get; set; }
        [Required] public ReservationSource Source { get; set; }
        [Required] public string GuestFullName { get; set; } = string.Empty;
        [Required][EmailAddress] public string GuestEmail { get; set; } = string.Empty;
        [Required] public string GuestPhone { get; set; } = string.Empty;
        public string? GuestIdNumber { get; set; }
        [Required] public DateOnly CheckInDate { get; set; }
        [Required] public DateOnly CheckOutDate { get; set; }
        [Range(1, int.MaxValue)] public int NumberOfGuests { get; set; }
        [Required] public Decimal TotalAmount { get; set; }
        public string? SpecialRequests { get; set; }
        public string? Notes { get; set; }
        public bool HasInsurance { get; set; } = false;
        [Required] public IFormFile ContractFile { get; set; } = null!;
        [Required] public IFormFile InvoiceFile { get; set; } = null!;
    }

    public class UpdateReservationRequest
    {
        public ReservationSource? Source { get; set; }
        public string? GuestFullName { get; set; }
        public string? GuestPhone { get; set; }
        public string? GuestIdNumber { get; set; }
        public DateOnly? CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }
        [Range(1, int.MaxValue)] public int? NumberOfGuests { get; set; }
        public bool? HasInsurance { get; set; }
        public string? SpecialRequests { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateReservationStatusRequest
    {
        [Required] public ReservationStatus Status { get; set; }
    }

    public class ReservationListRequest
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }
        public ReservationStatus? Status { get; set; }
        public DateOnly? CheckInFrom { get; set; }
        public DateOnly? CheckInTo { get; set; }
    }

    public class ReservationResponse
    {
        public int Id { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public int HotelId { get; set; }
        public string? HotelName { get; set; }
        public List<string> RoomNumbers { get; set; } = [];
        public int? CustomerId { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string GuestFullName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public Decimal TotalAmount { get; set; }
        public bool HasInsurance { get; set; }
        public decimal InsuranceAmount { get; set; }
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

        public ReservationResponse(Reservation r)
        {
            Id = r.Id;
            ReservationNumber = r.ReservationNumber;
            HotelId = r.HotelId;
            HotelName = r.Hotel?.Name;
            RoomNumbers = r.ReservationRooms.Select(rr => rr.Room?.RoomNumber ?? string.Empty).ToList();
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
            ContractUrl = r.ContractPath;
            InvoiceUrl = r.InvoicePath;
            SpecialRequests = r.SpecialRequests;
            Notes = r.Notes;
            CreatedById = r.CreatedById;
            UpdatedById = r.UpdatedById;
            CreatedAt = r.CreatedAt;
            UpdatedAt = r.UpdatedAt;
            TotalAmount = r.TotalAmount;
            HasInsurance = r.HasInsurance;
            InsuranceAmount = r.InsuranceAmount;
        }
    }

    public class CancelReservationRequest
    {
        public string? CancellationReason { get; set; }
    }

    public class CancellationResponse
    {
        public int ReservationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal CancellationFee { get; set; }
        public bool IsFreeCancellation { get; set; }
        public DateTime CancelledAt { get; set; }
        public string Message { get; set; } = string.Empty;

        public CancellationResponse(Reservation r, string message)
        {
            ReservationId = r.Id;
            Status = r.Status.ToString();
            TotalAmount = r.TotalAmount;
            CancellationFee = r.CancellationFee;
            IsFreeCancellation = r.IsFreeCancellation;
            CancelledAt = r.CancelledAt!.Value;
            Message = message;
        }
    }

    public class ListReservationResponse
    {
        public int Id { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public List<string> RoomNumbers { get; set; } = [];
        public string GuestFullName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public Decimal TotalAmount { get; set; }
        public bool HasInsurance { get; set; }
        public decimal InsuranceAmount { get; set; }

        public ListReservationResponse(Reservation r)
        {
            Id = r.Id;
            ReservationNumber = r.ReservationNumber;
            RoomNumbers = r.ReservationRooms.Select(rr => rr.Room?.RoomNumber ?? string.Empty).ToList();
            GuestFullName = r.GuestFullName;
            Status = r.Status.ToString();
            CheckInDate = r.CheckInDate;
            CheckOutDate = r.CheckOutDate;
            CreatedAt = r.CreatedAt;
            TotalAmount = r.TotalAmount;
            HasInsurance = r.HasInsurance;
            InsuranceAmount = r.InsuranceAmount;
        }
    }
}
