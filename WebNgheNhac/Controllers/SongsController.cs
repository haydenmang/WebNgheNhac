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

        // 1. GET: api/songs
        [HttpGet]
        public async Task<IActionResult> GetSongs()
        {
            var songs = await _context.Songs
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
            return Ok(songs);
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
    }
}
