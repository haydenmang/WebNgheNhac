using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using WebNgheNhac.Models;

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Đường dẫn: /api/singers
    public class SingersController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;

        public SingersController(WebNgheNhacDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/singers (Lấy danh sách tất cả ca sĩ)
        [HttpGet]
        public async Task<IActionResult> GetSingers()
        {
            var singers = await _context.Singers
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Avatar,
                    s.Bio,
                    TotalSongs = s.Songs.Count,
                    TotalAlbums = s.Albums.Count
                })
                .ToListAsync();
            return Ok(singers);
        }

        // 2. GET: api/singers/{id} (Lấy chi tiết 1 ca sĩ)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSinger(int id)
        {
            var singer = await _context.Singers
                .Include(s => s.Albums)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (singer == null)
            {
                return NotFound(new { message = "Không tìm thấy ca sĩ này!" });
            }
            return Ok(singer);
        }

        // 3. GET: api/singers/{id}/songs (Lấy toàn bộ bài hát của ca sĩ)
        [HttpGet("{id}/songs")]
        public async Task<IActionResult> GetSongsBySinger(int id)
        {
            var singerExists = await _context.Singers.AnyAsync(s => s.Id == id);
            if (!singerExists)
            {
                return NotFound(new { message = "Không tìm thấy ca sĩ này!" });
            }

            var songs = await _context.Songs
                .Where(s => s.SingerId == id)
                .Include(s => s.Type)
                .Include(s => s.Album)
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.Duration,
                    s.AudioUrl,
                    s.CoverUrl,
                    s.Plays,
                    TypeName = s.Type != null ? s.Type.TypeName : null,
                    AlbumName = s.Album != null ? s.Album.AlbumName : null
                })
                .ToListAsync();

            return Ok(songs);
        }

        // 4. POST: api/singers (Tạo mới hồ sơ ca sĩ - Admin)
        [HttpPost]
        public async Task<IActionResult> CreateSinger([FromBody] CreateSingerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Tên ca sĩ không được để trống!" });
            }

            var singer = new Singer
            {
                Name = request.Name.Trim(),
                Avatar = string.IsNullOrWhiteSpace(request.Avatar) ? "/images/default-avatar.png" : request.Avatar.Trim(),
                Bio = request.Bio
            };

            _context.Singers.Add(singer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm ca sĩ thành công!", singer });
        }

        // 5. PUT: api/singers/{id} (Cập nhật thông tin ca sĩ - Admin)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSinger(int id, [FromBody] CreateSingerRequest request)
        {
            var singer = await _context.Singers.FindAsync(id);
            if (singer == null)
            {
                return NotFound(new { message = "Không tìm thấy ca sĩ để cập nhật!" });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Tên ca sĩ không được để trống!" });
            }

            singer.Name = request.Name.Trim();
            if (!string.IsNullOrWhiteSpace(request.Avatar))
            {
                singer.Avatar = request.Avatar.Trim();
            }
            singer.Bio = request.Bio;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật ca sĩ thành công!", singer });
        }

        // 6. DELETE: api/singers/{id} (Xóa ca sĩ khỏi hệ thống - Admin)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSinger(int id)
        {
            var singer = await _context.Singers
                .Include(s => s.Songs)
                .Include(s => s.Albums)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (singer == null)
            {
                return NotFound(new { message = "Không tìm thấy ca sĩ để xóa!" });
            }

            // Kiểm tra ràng buộc: Nếu ca sĩ đã có bài hát hoặc album thì cảnh báo
            if (singer.Songs.Any() || singer.Albums.Any())
            {
                return BadRequest(new { message = $"Không thể xóa ca sĩ này vì đang có {singer.Songs.Count} bài hát và {singer.Albums.Count} album liên kết! Hãy xóa hoặc chuyển bài hát/album trước." });
            }

            _context.Singers.Remove(singer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa ca sĩ thành công!" });
        }
    }

    public class CreateSingerRequest
    {
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
    }
}
