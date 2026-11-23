using System.ComponentModel.DataAnnotations;
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

        // 4. POST: api/auth/change-password (Đổi mật khẩu)
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
            {
                return BadRequest(new { message = "Mật khẩu cũ không chính xác" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }

        // 5. GET: api/auth/users (Admin xem danh sách user)
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Avatar,
                    u.Level,
                    u.CreatedAt,
                    TotalPlaylists = u.Playlists.Count
                })
                .ToListAsync();

            return Ok(users);
        }

        // 6. DELETE: api/auth/users/{id} (Admin xóa tài khoản)
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa tài khoản thành công" });
        }
    }

    // Các lớp định dạng dữ liệu gửi lên (DTOs)
    public class RegisterRequest
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string? FullName { get; set; }

        public string? Avatar { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }

    public class ChangePasswordRequest
    {
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Required]
        public string OldPassword { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}
