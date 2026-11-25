using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using WebNgheNhac.Models;

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Đường dẫn: /api/albums
    public class AlbumsController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;

        public AlbumsController(WebNgheNhacDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/albums (Lấy danh sách tất cả Album)
        [HttpGet]
        public async Task<IActionResult> GetAlbums()
        {
            var albums = await _context.Albums
                .Include(a => a.Singer)
                .Select(a => new
                {
                    a.Id,
                    a.AlbumName,
                    a.CoverUrl,
                    a.CreatedAt,
                    a.SingerId,
                    SingerName = a.Singer != null ? a.Singer.Name : "Chưa rõ",
                    TotalSongs = a.Songs.Count
                })
                .ToListAsync();

            return Ok(albums);
        }

        // 2. GET: api/albums/{id} (Lấy chi tiết 1 Album)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbum(int id)
        {
            var album = await _context.Albums
                .Include(a => a.Singer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound(new { message = "Không tìm thấy album này!" });
            }

            return Ok(album);
        }

        // 3. GET: api/albums/{id}/songs (Lấy toàn bộ bài hát trong Album)
        [HttpGet("{id}/songs")]
        public async Task<IActionResult> GetSongsByAlbum(int id)
        {
            var albumExists = await _context.Albums.AnyAsync(a => a.Id == id);
            if (!albumExists)
            {
                return NotFound(new { message = "Không tìm thấy album này!" });
            }

            var songs = await _context.Songs
                .Where(s => s.AlbumId == id)
                .Include(s => s.Singer)
                .Include(s => s.Type)
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
                .ToListAsync();

            return Ok(songs);
        }

        // 4. POST: api/albums (Tạo mới Album - Admin)
        [HttpPost]
        public async Task<IActionResult> CreateAlbum([FromBody] CreateAlbumRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AlbumName))
            {
                return BadRequest(new { message = "Tên album không được để trống!" });
            }

            var singerExists = await _context.Singers.AnyAsync(s => s.Id == request.SingerId);
            if (!singerExists)
            {
                return BadRequest(new { message = "Ca sĩ sở hữu album không tồn tại!" });
            }

            var album = new Album
            {
                AlbumName = request.AlbumName.Trim(),
                CoverUrl = string.IsNullOrWhiteSpace(request.CoverUrl) ? "/images/default-album.png" : request.CoverUrl.Trim(),
                SingerId = request.SingerId,
                CreatedAt = DateTime.Now
            };

            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo album thành công!", album });
        }

        // 5. PUT: api/albums/{id} (Cập nhật thông tin Album - Admin)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum(int id, [FromBody] CreateAlbumRequest request)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound(new { message = "Không tìm thấy album để cập nhật!" });
            }

            if (string.IsNullOrWhiteSpace(request.AlbumName))
            {
                return BadRequest(new { message = "Tên album không được để trống!" });
            }

            var singerExists = await _context.Singers.AnyAsync(s => s.Id == request.SingerId);
            if (!singerExists)
            {
                return BadRequest(new { message = "Ca sĩ sở hữu album không tồn tại!" });
            }

            album.AlbumName = request.AlbumName.Trim();
            if (!string.IsNullOrWhiteSpace(request.CoverUrl))
            {
                album.CoverUrl = request.CoverUrl.Trim();
            }
            album.SingerId = request.SingerId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật album thành công!", album });
        }

        // 6. DELETE: api/albums/{id} (Xóa Album - Admin)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound(new { message = "Không tìm thấy album để xóa!" });
            }

            // Gỡ liên kết bài hát: bài hát trong album này chuyển thành Single (AlbumId = null)
            var songsInAlbum = await _context.Songs.Where(s => s.AlbumId == id).ToListAsync();
            foreach (var song in songsInAlbum)
            {
                song.AlbumId = null;
            }

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa album thành công (các bài hát thuộc album này đã được chuyển thành đĩa đơn Single)!" });
        }
    }

    public class CreateAlbumRequest
    {
        public string AlbumName { get; set; } = null!;
        public string? CoverUrl { get; set; }
        public int SingerId { get; set; }
    }
}
