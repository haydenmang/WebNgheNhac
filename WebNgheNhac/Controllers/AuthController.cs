using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using WebNgheNhac.Models;

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Dường dẫn: /api/auth
    public class AuthController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;
        public AuthController(WebNgheNhacDbContext context)
        {
            _context = context;
        }

        // 1. POST: api/auth/register (Đăng ký tài khoản mới)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Kiểm tra tên đăng nhập đã có ai dùng chưa
            var existingUser = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (existingUser)
            {
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác" });
            }

            // Mã háo mật khẩu bằng thuật toán BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Tạo người dùng mới
            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                FullName = string.IsNullOrWhiteSpace(request.FullName) ? request.Username : request.FullName,
                Avatar = string.IsNullOrWhiteSpace(request.Avatar) ? "/images/default-avatar.png" : request.Avatar,
                Level = 0, // 0: Thành viên thường, 1: Quản trị viên Admin
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đăng ký tài khoản thành công",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Avatar,
                    user.Level
                }
            });
        }

        // 2. POST: api/auth/login (đăng nhập hệ thống)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Tìm tài khoản theo UserName
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác" });
            }

            // Dùng BCrypt để kiểm tra xem mật khẩu người dùng gõ có khớp với chuỗi băm trong DB không
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "tên đăng nhập hoặc mật khẩu không chính xác" });
            }

            // Đăng nhập thành công -> Trả về thông tin người dùng
            return Ok(new
            {
                StatusCode = 200,
                message = "Đăng nhập thành công",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Avatar,
                    user.Level
                }
            });
        }

        // 3. GET: api/auth/profile/{id} (Lấy thông tin tài khoản & danh sách Playlist)
        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var user = await _context.Users
                .Include(u => u.Playlists)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng này" });
            }

            return Ok(new
            {
                user.Id,
                user.Username,
                user.FullName,
                user.Avatar,
                user.Level,
                user.CreatedAt,
                Playlists = user.Playlists.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.CreatedAt,
                    TotalSongs = p.PlaylistSongs.Count
                })
            });
        }
    }

    // Các lớp định dạng dữ liệu gửi lên (DTOs)
    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
    }
    public class LoginRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
