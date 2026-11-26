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

        private const string SeedPlaceholderHash = "$2a$11$qRzS6W7kG.9bL8l0kM5cWeu.Zf9B7sW1vN5f1kO7mH8kZ1o7rG9mK";

        private static bool VerifyPasswordSafe(string inputPassword, string storedHash, out bool needsUpgrade)
        {
            needsUpgrade = false;
            if (string.IsNullOrEmpty(storedHash)) return false;

            // Tương thích chuỗi hash mẫu trong Insert_Hiphop_Data.sql (mật khẩu mặc định 123456)
            if (storedHash == SeedPlaceholderHash && inputPassword == "123456")
            {
                needsUpgrade = true;
                return true;
            }

            try
            {
                if (storedHash.StartsWith("$2"))
                {
                    return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
                }
            }
            catch
            {
                // Fallback nếu chuỗi lưu trong DB không phải định dạng salt BCrypt hợp lệ
            }

            if (storedHash == inputPassword)
            {
                needsUpgrade = true;
                return true;
            }

            return false;
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

            // Dùng BCrypt kiểm tra mật khẩu (tự động nâng cấp hash chuẩn cho dữ liệu seed SQL)
            bool isPasswordValid = VerifyPasswordSafe(request.Password, user.PasswordHash, out bool needsUpgrade);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác" });
            }

            if (needsUpgrade)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                await _context.SaveChangesAsync();
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

        // 3. GET: api/auth/profile/{id} (Lấy thông tin tài khoản & danh sách Playlist kèm đúng số bài hát)
        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var user = await _context.Users
                .Include(u => u.Playlists)
                    .ThenInclude(p => p.PlaylistSongs)
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
                    TotalSongs = p.PlaylistSongs.Count,
                    SongIds = p.PlaylistSongs.Select(ps => ps.SongId).ToList()
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

            bool isOldPasswordValid = VerifyPasswordSafe(request.OldPassword, user.PasswordHash, out _);
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
            var user = await _context.Users
                .Include(u => u.Playlists)
                    .ThenInclude(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }
            if (user.Username.ToLower() == "admin" || user.Level == 1)
            {
                return BadRequest(new { message = "Không thể xóa tài khoản Quản trị viên hệ thống" });
            }

            foreach (var pl in user.Playlists)
            {
                _context.PlaylistSongs.RemoveRange(pl.PlaylistSongs);
            }
            _context.Playlists.RemoveRange(user.Playlists);
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
