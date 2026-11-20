using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;

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
    }
}
