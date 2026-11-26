using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using Type = WebNgheNhac.Models.Type; // Đặt alisas Type để tránh xung đột với System.Type

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Đường dẫn: /api/types
    public class TypesController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;
        public TypesController(WebNgheNhacDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/types (Lấy danh sách thể loại)
        [HttpGet]
        public async Task<IActionResult> GetTypes()
        {
            var types = await _context.Types
                .Select(t => new
                {
                    t.Id,
                    t.TypeName,
                    t.ImageUrl,
                    TotalSongs = t.Songs.Count // Đếm số lượng bài hát của thể loại
                })
                .ToListAsync();
            return Ok(types);
        }

        // 2. GET: api/types/{id} (Lấy thông tin chi tiết của thể loại theo ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetType(int id)
        {
            var type = await _context.Types.FindAsync(id);
            if (type == null)
            {
                return NotFound(new { message = "Không tìm thấy thể loại này" });
            }
            return Ok(type);
        }

        // 3. GET: api/types/{id}/songs (Lấy danh sách bài hát của thể loại theo ID)
        [HttpGet("{id}/songs")]
        public async Task<IActionResult> GetSongsByType(int id)
        {
            var typeExists = await _context.Types.AnyAsync(t => t.Id == id);
            if (!typeExists)
            {
                return NotFound(new { message = "Không tìm thấy thể loại này" });
            }
            var songs = await _context.Songs
                .Where(s => s.TypeId == id)
                .Include(s => s.Singer)
                .Include(s => s.Album)
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.AudioUrl,
                    s.CoverUrl,
                    s.Duration,
                    s.Plays,
                    SingerName = s.Singer != null ? s.Singer.Name : "Chưa rõ",
                    AlbumName = s.Album != null ? s.Album.AlbumName : "Chưa rõ"
                })
                .ToListAsync();
            return Ok(songs);
        }

        // 4. POST: api/types (Thêm thể loại mới)
        [HttpPost]
        public async Task<IActionResult> CreateType([FromBody] CreateTypeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TypeName))
            {
                return BadRequest(new { message = "Tên thể loại không được để trống" });
            }

            var type = new Type
            {
                TypeName = request.TypeName,
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad" : request.ImageUrl
            };

            _context.Types.Add(type);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm thể loại thành công", type });
        }

        // 4b. PUT: api/types/{id} (Cập nhật thông tin thể loại)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateType(int id, [FromBody] CreateTypeRequest request)
        {
            var type = await _context.Types.FindAsync(id);
            if (type == null)
            {
                return NotFound(new { message = "Không tìm thấy thể loại cần sửa" });
            }
            if (string.IsNullOrWhiteSpace(request.TypeName))
            {
                return BadRequest(new { message = "Tên thể loại không được để trống" });
            }

            type.TypeName = request.TypeName;
            if (!string.IsNullOrWhiteSpace(request.ImageUrl)) type.ImageUrl = request.ImageUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thể loại thành công", type });
        }

        // 5. DELETE: api/types/{id} (Xóa thể loại)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteType(int id)
        {
            var type = await _context.Types
                .Include(t => t.Songs)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (type == null)
            {
                return NotFound(new { message = "Không tìm thấy thể loại để xóa" });
            }

            if (type.Songs.Any())
            {
                return BadRequest(new { message = $"Không thể xóa thể loại '{type.TypeName}' vì đang có {type.Songs.Count} bài hát thuộc thể loại này!" });
            }

            _context.Types.Remove(type);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa thể loại thành công" });
        }
    }

    public class CreateTypeRequest
    {
        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
