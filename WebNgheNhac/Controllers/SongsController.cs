using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using WebNgheNhac.Models;
using Supabase;

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Đường dẫn: /api/songs
    public class SongsController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;
        private readonly Supabase.Client _supabase;

        public SongsController(WebNgheNhacDbContext context, Supabase.Client supabase)
        {
            _context = context;
            _supabase = supabase;
        }

        // 1. GET: api/songs (Lấy danh sách tất cả bài hát)
        [HttpGet]
        public async Task<IActionResult> GetSongs()
        {
            var songs = await _context.Songs
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
                    s.SingerId,
                    s.TypeId,
                    s.AlbumId,
                    SingerName = s.Singer != null ? s.Singer.Name : "Chưa rõ",
                    TypeName = s.Type != null ? s.Type.TypeName : "Chưa rõ"
                })
                .ToListAsync();
            return Ok(songs);
        }

        // 2. GET: api/songs/{id} (Lấy chi tiết 1 bài hát)
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

        // 3. POST: api/songs/{id}/play (Ghi nhận lượt nghe khi phát nhạc)
        [HttpPost("{id}/play")]
        public async Task<IActionResult> PlaySong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound(new { message = "Không tìm thấy bài hát!" });
            }

            song.Plays += 1;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã tăng lượt nghe thành công!", currentPlays = song.Plays });
        }

        // 4. POST: api/songs (Upload MP3 và Ảnh lên Supabase rồi lưu vào SQL)
        [HttpPost]
        public async Task<IActionResult> CreateSong(
            [FromForm] string title,
            [FromForm] int singerId,
            [FromForm] int typeId,
            [FromForm] int? albumId,
            [FromForm] int duration,
            IFormFile fileAudio,
            IFormFile? fileCover)
        {
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

            // B. Đẩy file ảnh bìa lên bucket 'covers' của Supabase (nếu có chọn)
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

            // C. Tạo bản ghi Song mới và lưu vào SQL Server
            var song = new Song
            {
                Title = title,
                SingerId = singerId,
                TypeId = typeId,
                AlbumId = albumId,
                Duration = duration,
                AudioUrl = audioUrl,
                CoverUrl = string.IsNullOrEmpty(coverUrl) ? "/images/default-cover.png" : coverUrl,
                Plays = 0,
                CreatedAt = DateTime.Now
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm bài hát thành công!", song });
        }

        // 5. PUT: api/songs/{id} (Cập nhật thông tin bài hát - Admin)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(
            int id,
            [FromForm] string title,
            [FromForm] int singerId,
            [FromForm] int typeId,
            [FromForm] int? albumId,
            [FromForm] int duration,
            IFormFile? fileAudio,
            IFormFile? fileCover)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound(new { message = "Không tìm thấy bài hát để cập nhật!" });
            }

            // Nếu người dùng tải file nhạc mới -> upload và cập nhật AudioUrl
            if (fileAudio != null && fileAudio.Length > 0)
            {
                using var memoryStreamAudio = new MemoryStream();
                await fileAudio.CopyToAsync(memoryStreamAudio);
                var audioFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileAudio.FileName);
                await _supabase.Storage.From("songs").Upload(memoryStreamAudio.ToArray(), audioFileName);
                song.AudioUrl = _supabase.Storage.From("songs").GetPublicUrl(audioFileName);
            }

            // Nếu người dùng tải ảnh bìa mới -> upload và cập nhật CoverUrl
            if (fileCover != null && fileCover.Length > 0)
            {
                using var memoryStreamCover = new MemoryStream();
                await fileCover.CopyToAsync(memoryStreamCover);
                var coverFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileCover.FileName);
                await _supabase.Storage.From("covers").Upload(memoryStreamCover.ToArray(), coverFileName);
                song.CoverUrl = _supabase.Storage.From("covers").GetPublicUrl(coverFileName);
            }

            song.Title = title;
            song.SingerId = singerId;
            song.TypeId = typeId;
            song.AlbumId = albumId;
            song.Duration = duration;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật bài hát thành công!", song });
        }

        // 6. DELETE: api/songs/{id} (Xóa bài hát khỏi hệ thống - Admin)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound(new { message = "Không tìm thấy bài hát để xóa!" });
            }

            // Xóa liên kết bài hát trong các Playlist trước để tránh lỗi khóa ngoại
            var playlistSongs = await _context.PlaylistSongs.Where(ps => ps.SongId == id).ToListAsync();
            _context.PlaylistSongs.RemoveRange(playlistSongs);

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa bài hát khỏi hệ thống thành công!" });
        }
    }
}
