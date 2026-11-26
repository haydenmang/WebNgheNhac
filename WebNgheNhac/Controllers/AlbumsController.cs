using System.ComponentModel.DataAnnotations;
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

        // 1. GET: api/albums (Lấy danh sách album)
        [HttpGet]
        public async Task<IActionResult> GetAlbums()
        {
            var albums = await _context.Albums
                .Include(a => a.Singer) // Bao gồm thông tin ca sĩ
                .Select(a => new
                {
                    a.Id,
                    a.AlbumName,
                    a.CoverUrl,
                    a.SingerId,
                    a.CreatedAt,
                    SingerName = a.Singer != null ? a.Singer.Name : "Chưa rõ",
                    TotalSongs = a.Songs.Count // Đếm số lượng bài hát trong album
                })
                .ToListAsync();

            return Ok(albums);
        }

        // 2. GET: api/albums/{id} (Lấy thông tin chi tiết của album theo ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbum(int id)
        {
            var album = await _context.Albums
                .Include(a => a.Singer) // Bao gồm thông tin ca sĩ
                .FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
            {
                return NotFound(new { message = "Không tìm thấy album này" });
            }

            return Ok(album);
        }

        // 3. GET: api/albums/{id}/songs (Lấy danh sách bài hát trong album theo ID)
        [HttpGet("{id}/songs")]
        public async Task<IActionResult> GetSongsByAlbum(int id)
        {
            var albumExists = await _context.Albums.AnyAsync(a => a.Id == id);
            if (!albumExists)
            {
                return NotFound(new { message = "Không tìm thấy album này" });
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

        // 4. POST: api/albums (Thêm album mới)
        [HttpPost]
        public async Task<IActionResult> CreateAlbum([FromBody] CreateAlbumRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AlbumName))
            {
                return BadRequest(new { message = "Tên album không được để trống" });
            }

            var album = new Album
            {
                AlbumName = request.AlbumName,
                CoverUrl = string.IsNullOrWhiteSpace(request.CoverUrl) ? "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4" : request.CoverUrl,
                SingerId = request.SingerId,
                CreatedAt = DateTime.Now
            };

            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm album thành công", album });
        }

        // 4b. PUT: api/albums/{id} (Cập nhật thông tin album)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum(int id, [FromBody] CreateAlbumRequest request)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound(new { message = "Không tìm thấy album cần sửa" });
            }
            if (string.IsNullOrWhiteSpace(request.AlbumName))
            {
                return BadRequest(new { message = "Tên album không được để trống" });
            }

            album.AlbumName = request.AlbumName;
            if (request.SingerId > 0) album.SingerId = request.SingerId;
            if (!string.IsNullOrWhiteSpace(request.CoverUrl)) album.CoverUrl = request.CoverUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật album thành công", album });
        }

        // 5. DELETE: api/albums/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            var album = await _context.Albums
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
            {
                return NotFound(new { message = "Không tìm thấy album để xóa" });
            }

            foreach (var song in album.Songs)
            {
                song.AlbumId = null;
            }
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa album thành công" });
        }
    }

    public class CreateAlbumRequest
    {
        [Required]
        public string AlbumName { get; set; } = null!;

        [Required]
        public string CoverUrl { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int SingerId { get; set; }
    }
}
