using EventParkingReservationSystem.API.DTOs.Customers;


namespace EventParkingReservationSystem.API.IServices;

public interface ICustomerService
{
    Task<CustomerResponseDto> RegisterAsync(RegisterCustomerDto dto);
    Task<CustomerResponseDto> GetAsync(int id);
    Task<CustomerResponseDto> UpdateAsync(int id, UpdateCustomerDto dto);
    Task<IReadOnlyList<CustomerResponseDto>> SearchAsync(string? search);
    Task DeactivateAsync(int id);
    Task ReactivateAsync(int id);
   
    
}
