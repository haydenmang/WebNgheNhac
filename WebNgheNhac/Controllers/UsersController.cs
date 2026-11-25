using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Đường dẫn: /api/users
    public class UsersController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;

        public UsersController(WebNgheNhacDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/users (Lấy danh sách tất cả người dùng trong hệ thống - Admin)
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Avatar,
                    u.Level, // 0: User, 1: Admin
                    u.CreatedAt,
                    TotalPlaylists = u.Playlists.Count
                })
                .ToListAsync();

            return Ok(users);
        }

        // 2. GET: api/users/{id} (Lấy chi tiết 1 người dùng)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Playlists)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng này!" });
            }

            return Ok(new
            {
                user.Id,
                user.Username,
                user.FullName,
                user.Avatar,
                user.Level,
                user.CreatedAt,
                TotalPlaylists = user.Playlists.Count
            });
        }

        // 3. PUT: api/users/{id}/level (Thay đổi quyền tài khoản: User <-> Admin)
        [HttpPut("{id}/level")]
        public async Task<IActionResult> ChangeUserLevel(int id, [FromBody] ChangeLevelRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng!" });
            }

            // Chỉ cho phép 0 (User) hoặc 1 (Admin)
            if (request.Level != 0 && request.Level != 1)
            {
                return BadRequest(new { message = "Level chỉ chấp nhận giá trị 0 (User) hoặc 1 (Admin)!" });
            }

            user.Level = request.Level;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã cập nhật cấp bậc của tài khoản {user.Username} thành {(user.Level == 1 ? "Admin" : "User")}!" });
        }

        // 4. DELETE: api/users/{id} (Xóa tài khoản người dùng - Admin)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Playlists)
                    .ThenInclude(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng để xóa!" });
            }

            // Xóa toàn bộ liên kết PlaylistSongs và Playlists của user trước
            foreach (var playlist in user.Playlists)
            {
                _context.PlaylistSongs.RemoveRange(playlist.PlaylistSongs);
            }
            _context.Playlists.RemoveRange(user.Playlists);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã xóa tài khoản {user.Username} thành công!" });
        }
    }

    public class ChangeLevelRequest
    {
        public int Level { get; set; }
    }
}
