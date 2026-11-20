using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Đường dẫn; /api/search
    public class SearchController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;
        public SearchController(WebNgheNhacDbContext context)
        {
            _context = context;
        }

        // GET: api/search?keyword=mck
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? keyword)
        {
            // Nếu người dùng không nhập gì hoặc chỉ nhập khoảng trắng
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Ok(new
                {
                    songs = new List<object>(),
                    singers = new List<object>(),
                    albums = new List<object>()
                });
            }

            var query = keyword.Trim().ToLower();

            // 1. Tìm kiếm bài hát theo tên bài HOẶC tên ca sĩ
            var songs = await _context.Songs
                .Include(s => s.Singer)
                .Include(s => s.Type)
                .Where(s => s.Title.ToLower().Contains(query) || (s.Singer != null && s.Singer.Name.ToLower().Contains(query)))
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.AudioUrl,
                    s.CoverUrl,
                    s.Duration,
                    s.Plays,
                    SingerName = s.Singer != null ? s.Singer.Name : "Chưa rõ",
                    TypeName = s.Type != null ? s.Type.TypeName : "Chưa rõ"
                })
                .Take(10) //Lấy tối đa 10 kết quả phù hợp
                .ToListAsync();
            // 2. Tìm kiếm ca sĩ theo tên
            var singers = await _context.Singers
                .Where(s => s.Name.ToLower().Contains(query))
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Avatar,
                    TotalSongs = s.Songs.Count
                })
                .Take(5)
                .ToListAsync();

            // 3. Tìm kiếm album theo tên album
            var albums = await _context.Albums
                .Include(a => a.Singer)
                .Where(a => a.AlbumName.ToLower().Contains(query))
                .Select(a => new
                {
                    a.Id,
                    a.AlbumName,
                    a.CoverUrl,
                    SingerName = a.Singer != null ? a.Singer.Name : "Chưa rõ"
                })
                .Take(5)
                .ToListAsync();

            // Trả về gói kết quả gộp cả 3 mục
            return Ok(new
            {
                keyword,
                songs,
                singers,
                albums
            });
        }
    }
}
