using System.Threading.Tasks;
using HotelBooking.Models;
using HotelBooking.Services.DTOs;

namespace HotelBooking.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<User?> GetProfileAsync(int userId);
    }
}
