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

        // 1. GET: api/songs?page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetSongs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 50) pageSize = 50;

            var total = await _context.Songs.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var songs = await _context.Songs
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Singer) // Bao gồm thông tin ca sĩ
                .Include(s => s.Type)   // Bao gồm thông tin thể loại
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

        // 4. POST: api/songs (Upload file MP3 và Ảnh lên Supabase rồi lưu vào SQL)
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateSong(
            [FromForm] string title,
            [FromForm] int singerId,
            [FromForm] int typeId,
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

            string audioUrl = "";
            string coverUrl = "";

            // A. Đẩy file nhạc lên bucket 'songs' của Supabase
            using (var memoryStreamAudio = new MemoryStream())
            {
                await fileAudio.CopyToAsync(memoryStreamAudio);
                var audioFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileAudio.FileName);
                await _supabase.Storage.From("songs").Upload(memoryStreamAudio.ToArray(), audioFileName);
                audioUrl = _supabase.Storage.From("songs").GetPublicUrl(audioFileName);
            }

            // B. Đẩy file ảnh bìa lên bucket 'covers' của Supabase (nếu người dùng có chọn ảnh)
            if (fileCover != null && fileCover.Length > 0)
            {
                using (var memoryStreamCover = new MemoryStream())
                {
                    await fileCover.CopyToAsync(memoryStreamCover);
                    var coverFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileCover.FileName);
                    await _supabase.Storage.From("covers").Upload(memoryStreamCover.ToArray(), coverFileName);
                    coverUrl = _supabase.Storage.From("covers").GetPublicUrl(coverFileName);
                }
            }

            // C. Tạo bản ghi Song mới và kluwu vào SQL Server
            var song = new Song
            {
                Title = title,
                SingerId = singerId,
                TypeId = typeId,
                AlbumId = ablumId,
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

        // 5. DELETE: api/songs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound(new { message = "Không tìm thấy bài hát để xóa" });
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
}
