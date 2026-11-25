using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using Type = WebNgheNhac.Models.Type;

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

        // 1. GET: api/types (Lấy danh sách tất cả thể loại)
        [HttpGet]
        public async Task<IActionResult> GetTypes()
        {
            var types = await _context.Types
                .Select(t => new
                {
                    t.Id,
                    t.TypeName,
                    t.ImageUrl,
                    TotalSongs = t.Songs.Count
                })
                .ToListAsync();
            return Ok(types);
        }

        // 2. GET: api/types/{id} (Lấy chi tiết 1 thể loại)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetType(int id)
        {
            var type = await _context.Types.FindAsync(id);
            if (type == null)
            {
                return NotFound(new { message = "Không tìm thấy thể loại này!" });
            }
            return Ok(type);
        }

        // 3. GET: api/types/{id}/songs (Lấy danh sách bài hát theo thể loại)
        [HttpGet("{id}/songs")]
        public async Task<IActionResult> GetSongsByType(int id)
        {
            var typeExists = await _context.Types.AnyAsync(t => t.Id == id);
            if (!typeExists)
            {
                return NotFound(new { message = "Không tìm thấy thể loại này!" });
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

        // 4. POST: api/types (Thêm thể loại mới - Admin)
        [HttpPost]
        public async Task<IActionResult> CreateType([FromBody] CreateTypeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TypeName))
            {
                return BadRequest(new { message = "Tên thể loại không được để trống!" });
            }

            var existingType = await _context.Types.AnyAsync(t => t.TypeName.ToLower() == request.TypeName.Trim().ToLower());
            if (existingType)
            {
                return BadRequest(new { message = "Thể loại này đã tồn tại trong hệ thống!" });
            }

            var type = new Type
            {
                TypeName = request.TypeName.Trim(),
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? "/images/default-type.png" : request.ImageUrl.Trim()
            };

            _context.Types.Add(type);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm thể loại thành công!", type });
        }

        // 5. PUT: api/types/{id} (Cập nhật thể loại - Admin)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateType(int id, [FromBody] CreateTypeRequest request)
        {
            var type = await _context.Types.FindAsync(id);
            if (type == null)
            {
                return NotFound(new { message = "Không tìm thấy thể loại để cập nhật!" });
            }

            if (string.IsNullOrWhiteSpace(request.TypeName))
            {
                return BadRequest(new { message = "Tên thể loại không được để trống!" });
            }

            type.TypeName = request.TypeName.Trim();
            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                type.ImageUrl = request.ImageUrl.Trim();
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thể loại thành công!", type });
        }

        // 6. DELETE: api/types/{id} (Xóa thể loại - Admin)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteType(int id)
        {
            var type = await _context.Types
                .Include(t => t.Songs)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (type == null)
            {
                return NotFound(new { message = "Không tìm thấy thể loại để xóa!" });
            }

            if (type.Songs.Any())
            {
                return BadRequest(new { message = $"Không thể xóa thể loại này vì đang có {type.Songs.Count} bài hát thuộc thể loại này!" });
            }

            _context.Types.Remove(type);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa thể loại thành công!" });
        }
    }

    public class CreateTypeRequest
    {
        public string TypeName { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
