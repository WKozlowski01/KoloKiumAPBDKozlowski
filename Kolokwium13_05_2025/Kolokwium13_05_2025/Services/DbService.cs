using System.Data.Common;
using Kolokwium13_05_2025.DTOs;
using Kolokwium13_05_2025.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders.Physical;

namespace Kolokwium13_05_2025.Services;

public class DbService:IDbService

{
    private readonly string _connectionString;
    private readonly IConfiguration _configuration;
    
    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }




    public async Task<BookingDTO> GetBookingsAsync(int bookingID)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var com = new SqlCommand();
        com.Connection = con;

        await con.OpenAsync();
        DbTransaction transaction = await con.BeginTransactionAsync();
        com.Transaction = transaction as SqlTransaction;

        try
        {

            com.CommandText = @"SELECT guest_id,employee_id, date FROM Booking WHERE booking_id = @bookingId";
            com.Parameters.AddWithValue("@bookingId", bookingID);

            var booking = new BookingDTO();
            SqlDataReader reader = await com.ExecuteReaderAsync();
            try
            {

                while (await reader.ReadAsync())
                {
                    int booking_id = (int)reader["booking_id"];
                    int employee_id = (int)reader["employee_id"];
                    int guest_id = (int)reader["guest_id"];
                    DateTime date = (DateTime)reader["date"];

                    booking.booking_id = booking_id;
                    booking.employee_id = employee_id;
                    booking.guest_id = guest_id;
                    booking.date = date;

                }
            }
            catch (Exception ex)
            {
                throw new NotFoundException("Booking with id: " + bookingID + " not found");
            }

            com.Parameters.Clear();
            await reader.CloseAsync();




            com.CommandText = @"SELECT * FROM Employee WHERE employee_id = @employeeId";
            com.Parameters.AddWithValue("@employeeId", booking.employee_id);

            var employee = new employeeDTO();
            reader = await com.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int employee_id = (int)reader["employee_id"];
                string firstName = reader["first_name"].ToString();
                string lastName = reader["last_name"].ToString();
                string employeeNumber = reader["employee_number"].ToString();

                employee.employee_number = employeeNumber;
                employee.firstName = firstName;
                employee.lastName = lastName;
                employee.employee_id = employee_id;
            }

            com.Parameters.Clear();
            await reader.CloseAsync();

            com.CommandText = @"SELECT attraction_id, amount, name,price FROM Booking_Attractions
                                         JOIN Attraction ON Booking_Attractions.attraction_id = Attraction.attraction_id
                                         WHERE booking_id = @bookingId";
            com.Parameters.AddWithValue("@employeeId", booking.employee_id);

            List<AttractionDTO> attractions = new List<AttractionDTO>();
            reader = await com.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int attraction_id = (int)reader["attraction_id"];
                string name = reader["name"].ToString();
                decimal price = (decimal)reader["price"];

                var attr = new AttractionDTO()
                {
                    attraction_id = attraction_id,
                    name = name,
                    price = price

                };

                attractions.Add(attr);
            }

            com.Parameters.Clear();
            await reader.CloseAsync();


            var guest = new GuestDataDTO();
            com.CommandText = @"SELECT frist_name, last_name, date_of_birth FROM Guest WHERE guest_id = @GuestId";
            com.Parameters.AddWithValue("@GuestId", booking.guest_id);
            reader = await com.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string firstName = reader["first_name"].ToString();
                string lastName = reader["last_name"].ToString();
                DateTime dateofBitrh = (DateTime)reader["date_of_birth"];

                guest.firstName = firstName;
                guest.lastName = lastName;
                guest.dateOfBirth = dateofBitrh;
            }

            booking.attractions = attractions;
            booking.employee = employee;

            await transaction.CommitAsync();
            return booking;
            
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }


    }

    public async Task InsertBookingAsync(InsertData insertdata)
    {
      
        await using var con = new SqlConnection(_connectionString);
        await using var com = new SqlCommand();
        com.Connection = con;
        
        await con.OpenAsync();
        DbTransaction transaction = await con.BeginTransactionAsync();
        com.Transaction = transaction as SqlTransaction;

        try
        {
            com.CommandText = @"INSERT INTO Booking VALUES(@bookingId, @guestId, @employeeNumber, @date) ";
            com.Parameters.AddWithValue("@bookingID", insertdata.bookingId);
            com.Parameters.AddWithValue("@guestId", insertdata.guestId);
            com.Parameters.AddWithValue("employeeNumber", insertdata.employee_number);
            com.Parameters.AddWithValue("@date", DateTime.Now);

            try
            {
                await com.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new ConflictException("Booking with same ID exists");
            }

            foreach (var attraction in insertdata.attractions)
            {
                com.Parameters.Clear();
                com.CommandText = @"SELECT 1 FROM Guest WHERE guest_id = @guestId";
                com.Parameters.AddWithValue("@guestId", insertdata.guestId);
                var ifGuestExists = await com.ExecuteScalarAsync();
                if (ifGuestExists is null)
                {
                    throw new NotFoundException("Guest with id: " + insertdata.guestId + " not found");
                }

                com.Parameters.Clear();
                com.CommandText = @"SELECT 1 FROM Employee WHERE employee_number = @employeeID";
                com.Parameters.AddWithValue("@employeeID", insertdata.employee_number);
                var ifEmployeeExists = await com.ExecuteScalarAsync();
                if (ifGuestExists is null)
                {
                    throw new NotFoundException("employee with id: " + insertdata.employee_number + " not found");
                }

                com.Parameters.Clear();
                com.CommandText = @"INSERT INTO Attraction VALUES(@name, @price) ";
                com.Parameters.AddWithValue("@name", attraction.name);
                com.Parameters.AddWithValue("@price", attraction.price);


            }


        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }




    }
}
