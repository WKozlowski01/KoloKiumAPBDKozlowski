namespace Kolokwium13_05_2025.DTOs;

public class InsertData
{
    public int bookingId { get; set; }
    public int guestId { get; set; }
    public string employee_number { get; set; }
    public List<AttractionDTO> attractions { get; set; }
    
}