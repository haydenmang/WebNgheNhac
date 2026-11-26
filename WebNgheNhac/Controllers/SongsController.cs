using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using WebNgheNhac.Models;
using Supabase;

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //Đường dẫn API cho controller này sẽ là /api/songs

    public class SongsController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;
        private readonly Supabase.Client _supabase;

        // Bơm DbContext (nói chuyện với SQL) và Supabase Client (upload file) vào controller 
        public SongsController(WebNgheNhacDbContext context, Supabase.Client supabase)
        {
            _context = context;
            _supabase = supabase;
        }

        // 1. GET: api/songs?page=1&pageSize=50
        [HttpGet]
        public async Task<IActionResult> GetSongs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 200) pageSize = 200;

            var total = await _context.Songs.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var songs = await _context.Songs
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Singer) // Bao gồm thông tin ca sĩ
                .Include(s => s.Type)   // Bao gồm thông tin thể loại
                .Include(s => s.Album)  // Bao gồm thông tin album
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.AudioUrl,
                    s.CoverUrl,
                    s.Duration,
                    s.Plays,
                    s.SingerId,
                    s.TypeId,
                    s.AlbumId,
                    SingerName = s.Singer != null ? s.Singer.Name : "Chưa rõ",
                    TypeName = s.Type != null ? s.Type.TypeName : "Chưa rõ",
                    AlbumName = s.Album != null ? s.Album.AlbumName : null
                })
                .ToListAsync();

            return Ok(new
            {
                data = songs,
                total,
                page,
                pageSize,
                totalPages
            });
        }
        
        // 2. GET: api/songs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSong(int id)
        {
            var song = await _context.Songs
                .Include(s => s.Singer)
                .Include(s => s.Type)
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
            {
                return NotFound(new { message = "Không tìm thấy bài hát yêu cầu!" });
            }
            return Ok(song);
        }

        // 3. POST: api/songs/{id}/play
        [HttpPost("{id}/play")]
        public async Task<IActionResult> PlaySong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song ==  null)
            {
                return NotFound();
            }

            // Tự động tăng lượt nghe lên 1
            song.Plays += 1;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã tăng lượt nghe thành công!", currentPlays = song.Plays });
        }

        // 4. POST: api/songs (Upload file MP3 và Ảnh lên Supabase hoặc lưu cục bộ rồi lưu vào SQL)
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateSong(
            [FromForm] string title,
            [FromForm] int singerId,
            [FromForm] int typeId,
            [FromForm] int? albumId,
            [FromForm] int? ablumId,
            [FromForm] int duration,
            IFormFile fileAudio,
            IFormFile? fileCover)
        {
            // Kiểm tra bắt buộc phải có file nhạc
            if (fileAudio == null || fileAudio.Length == 0)
            {
                return BadRequest(new { message = "File nhạc không được để trống!" });
            }

            int? resolvedAlbumId = albumId ?? ablumId;
            string audioUrl = "";
            string coverUrl = "";
            var audioFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileAudio.FileName);

            // A. Đẩy file nhạc lên bucket 'songs' của Supabase (fallback lưu vào wwwroot/uploads/songs nếu Supabase chặn quyền ghi)
            try
            {
                using var memoryStreamAudio = new MemoryStream();
                await fileAudio.CopyToAsync(memoryStreamAudio);
                await _supabase.Storage.From("songs").Upload(memoryStreamAudio.ToArray(), audioFileName);
                audioUrl = _supabase.Storage.From("songs").GetPublicUrl(audioFileName);
            }
            catch
            {
                var localSongsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "songs");
                Directory.CreateDirectory(localSongsDir);
                var localAudioPath = Path.Combine(localSongsDir, audioFileName);
                using var fs = new FileStream(localAudioPath, FileMode.Create);
                await fileAudio.CopyToAsync(fs);
                audioUrl = $"/uploads/songs/{audioFileName}";
            }

            // B. Đẩy file ảnh bìa lên bucket 'covers' của Supabase (fallback lưu vào wwwroot/uploads/covers)
            if (fileCover != null && fileCover.Length > 0)
            {
                var coverFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileCover.FileName);
                try
                {
                    using var memoryStreamCover = new MemoryStream();
                    await fileCover.CopyToAsync(memoryStreamCover);
                    await _supabase.Storage.From("covers").Upload(memoryStreamCover.ToArray(), coverFileName);
                    coverUrl = _supabase.Storage.From("covers").GetPublicUrl(coverFileName);
                }
                catch
                {
                    var localCoversDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "covers");
                    Directory.CreateDirectory(localCoversDir);
                    var localCoverPath = Path.Combine(localCoversDir, coverFileName);
                    using var fs = new FileStream(localCoverPath, FileMode.Create);
                    await fileCover.CopyToAsync(fs);
                    coverUrl = $"/uploads/covers/{coverFileName}";
                }
            }

            // C. Tạo bản ghi Song mới và lưu vào SQL Server
            var song = new Song
            {
                Title = title,
                SingerId = singerId,
                TypeId = typeId,
                AlbumId = resolvedAlbumId,
                Duration = duration,
                AudioUrl = audioUrl,
                CoverUrl = string.IsNullOrEmpty(coverUrl) ? "/images/default-cover.png" : coverUrl,
                Plays = 0,
                CreatedAt = DateTime.Now
            };
            
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm bài hát thành công", song });
        }

        // 4b. POST: api/songs (Tạo bài hát qua JSON DTO)
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSongFromJson([FromBody] CreateSongRequest request)
        {
            var song = new Song
            {
                Title = request.Title,
                SingerId = request.SingerId,
                TypeId = request.TypeId,
                AlbumId = request.AlbumId,
                Duration = request.Duration,
                AudioUrl = request.AudioUrl,
                CoverUrl = request.CoverUrl,
                Plays = 0,
                CreatedAt = DateTime.Now
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm bài hát thành công", song });
        }

        // 4c. PUT: api/songs/{id} (Cập nhật thông tin bài hát)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(int id, [FromBody] UpdateSongRequest request)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound(new { message = "Không tìm thấy bài hát cần sửa" });
            }

            if (!string.IsNullOrWhiteSpace(request.Title)) song.Title = request.Title;
            if (request.SingerId > 0) song.SingerId = request.SingerId;
            if (request.TypeId > 0) song.TypeId = request.TypeId;
            song.AlbumId = (request.AlbumId.HasValue && request.AlbumId.Value > 0) ? request.AlbumId : null;
            if (request.Duration > 0) song.Duration = request.Duration;
            if (!string.IsNullOrWhiteSpace(request.AudioUrl)) song.AudioUrl = request.AudioUrl;
            if (!string.IsNullOrWhiteSpace(request.CoverUrl)) song.CoverUrl = request.CoverUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật bài hát thành công", song });
        }

        // 5. DELETE: api/songs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs
                .Include(s => s.PlaylistSongs)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (song == null)
            {
                return NotFound(new { message = "Không tìm thấy bài hát để xóa" });
            }

            if (song.PlaylistSongs.Any())
            {
                _context.PlaylistSongs.RemoveRange(song.PlaylistSongs);
            }
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa bài hát thành công" });
        }
    }

    public class CreateSongRequest
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string AudioUrl { get; set; } = null!;

        [Required]
        public string CoverUrl { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int SingerId { get; set; }

        [Range(1, int.MaxValue)]
        public int TypeId { get; set; }

        [Range(1, int.MaxValue)]
        public int? AlbumId { get; set; }

        public int Duration { get; set; }
    }

    public class UpdateSongRequest
    {
        [Required]
        public string Title { get; set; } = null!;
        public int SingerId { get; set; }
        public int TypeId { get; set; }
        public int? AlbumId { get; set; }
        public int Duration { get; set; }
        public string? AudioUrl { get; set; }
        public string? CoverUrl { get; set; }
    }
}
