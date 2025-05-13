namespace Kolokwium13_05_2025.DTOs;

public class GuestDataDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public DateTime dateOfBirth { get; set; }
}


public class classemployeeDTO
{ 
public int employee_id { get; set; }
public string firstName { get; set; }
public string lastName { get; set; }
public string employee_number { get; set; }
    
    
}

public class AttractionDTO
{
    public int? attraction_id { get; set; }
    public string name { get; set; }
    public decimal price { get; set; }
    
}


public class BookingDTO
{
    public int booking_id { get; set; }
    public int guest_id { get; set; }
    public int employee_id { get; set; }
    public DateTime date { get; set; }
    
    public employeeDTO employee { get; set; }
    
    public List<AttractionDTO> attractions { get; set; }

}

public class employeeDTO
{
    public int employee_id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string employee_number { get; set; }
}

