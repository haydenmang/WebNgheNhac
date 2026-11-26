using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using WebNgheNhac.Models;

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // ĐƯờng dẫn: /api/singers
    public class SingersController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;

        // Chỉ cần bơm DbContext vào vì chỉ đọc dữ liệu từ SQL Server
        public SingersController(WebNgheNhacDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/singers (Lấy danh sách ca sĩ)
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
                    TotalSongs = s.Songs.Count, // Đếm số lượng bài hát của ca sĩ
                    TotalAlbums = s.Albums.Count // Đếm số lượng album của ca sĩ
                })
                .ToListAsync();
            return Ok(singers);
        }

        // 2. GET: api/singers/{id} (Lấy thông tin chi tiết của ca sĩ theo ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSinger(int id)
        {
            var singer = await _context.Singers
                .Include(s => s.Albums)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (singer == null)
            {
                return NotFound(new { message = "Không tìm thấy ca sĩ này" });
            }
            return Ok(singer);
        }

        // 3. GET: api/singers/{id}/songs (Lấy danh sách bài hát của ca sĩ theo ID)
        [HttpGet("{id}/songs")]
        public async Task<IActionResult> GetSongsBySinger(int id)
        {
            // Kiểm tra xem ca sĩ có tồn tại không
            var singerExists = await _context.Singers.AnyAsync(s => s.Id == id);
            if (!singerExists)
            {
                return NotFound(new { message = "Không tìm thấy ca sĩ này" });
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

        // 4. POST: api/singers (Thêm ca sĩ mới)
        [HttpPost]
        public async Task<IActionResult> CreateSinger([FromBody] CreateSingerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Tên ca sĩ không được để trống" });
            }

            var singer = new Singer
            {
                Name = request.Name,
                Avatar = string.IsNullOrWhiteSpace(request.Avatar) ? "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4" : request.Avatar,
                Bio = request.Bio
            };

            _context.Singers.Add(singer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm ca sĩ thành công", singer });
        }

        // 4b. PUT: api/singers/{id} (Cập nhật thông tin ca sĩ)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSinger(int id, [FromBody] CreateSingerRequest request)
        {
            var singer = await _context.Singers.FindAsync(id);
            if (singer == null)
            {
                return NotFound(new { message = "Không tìm thấy ca sĩ cần sửa" });
            }
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Tên ca sĩ không được để trống" });
            }

            singer.Name = request.Name;
            if (!string.IsNullOrWhiteSpace(request.Avatar)) singer.Avatar = request.Avatar;
            singer.Bio = request.Bio;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thông tin ca sĩ thành công", singer });
        }

        // 5. DELETE: api/singers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSinger(int id)
        {
            var singer = await _context.Singers
                .Include(s => s.Songs)
                .Include(s => s.Albums)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (singer == null)
            {
                return NotFound(new { message = "Không tìm thấy ca sĩ để xóa" });
            }

            if (singer.Songs.Any())
            {
                return BadRequest(new { message = $"Không thể xóa ca sĩ '{singer.Name}' vì đang có {singer.Songs.Count} bài hát thuộc ca sĩ này. Vui lòng xóa bài hát trước!" });
            }

            if (singer.Albums.Any())
            {
                _context.Albums.RemoveRange(singer.Albums);
            }
            _context.Singers.Remove(singer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa ca sĩ thành công" });
        }
    }

    public class CreateSingerRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
    }
}
