using Kolokwium13_05_2025.DTOs;

namespace Kolokwium13_05_2025.Services;

public interface IDbService
{
    Task<BookingDTO> GetBookingsAsync(int bookingID);
    
    Task InsertBookingAsync(InsertData insertdata);
}