using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using WebNgheNhac.Models;

namespace WebNgheNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //Đường dẫn: /api/playlist
    public class PlaylistsController : ControllerBase
    {
        private readonly WebNgheNhacDbContext _context;
        public PlaylistsController(WebNgheNhacDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/playlists/user/1 (Lấy tất cả playlist của 1 người dùng kèm danh sách ID bài hát)
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPlaylists(int userId)
        {
            var playlists = await _context.Playlists
                .Where(p => p.UserId == userId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.UserId,
                    p.CreatedAt,
                    TotalSongs = p.PlaylistSongs.Count,
                    SongIds = p.PlaylistSongs.Select(ps => ps.SongId).ToList()
                })
                .ToListAsync();

            return Ok(playlists);
        }

        // 2. GET: api/playlists/5 (lấy chi tiết playlist kèm toàn bộ bài hát bên trong)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylistDetails(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Singer)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return NotFound(new { message = "Không tìm thấy danh sách phát này" });
            }

            return Ok(new
            {
                playlist.Id,
                playlist.Name,
                playlist.CreatedAt,
                OwnerName = playlist.User != null ? playlist.User.FullName : "Ẩn danh",
                Songs = playlist.PlaylistSongs.Select(ps => new
                {
                    ps.Song.Id,
                    ps.Song.Title,
                    ps.Song.AudioUrl,
                    ps.Song.CoverUrl,
                    ps.Song.Duration,
                    ps.Song.Plays,
                    SingerName = ps.Song.Singer != null ? ps.Song.Singer.Name : "Chưa rõ",
                    AddedAt = ps.AddedAt
                })
            });
        }

        // 3. POST: api/playlists (Tạo một playlist mới)
        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Tên danh sách phát không được để trống" });
            }

            // Kiểm tra người dùng có tồn tại không 
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                return BadRequest(new { message = "Người dùng không tồn tại" });
            }

            var playlist = new Playlist
            {
                Name = request.Name,
                UserId = request.UserId,
                CreatedAt = DateTime.Now
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo danh sách phát thành công", playlist });
        }

        // 4. POST: api/playlists/5/songs (Thêm bài hát vào playlist - Bảng trung gian Playlist_Songs)
        [HttpPost("{playlistId}/songs")]
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, [FromBody] AddSongRequest request)
        {
            // Kiểm tra Playlist có tồn tại không
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null)
            {
                return NotFound(new { message = "Danh sách phát không tồn tại" });
            }

            // Kiểm tra bài hát có tồn tại không
            var song = await _context.Songs.FindAsync(request.SongId);
            if (song == null)
            {
                return NotFound(new { message = "Bài hát không tồn tại"});
            }

            // Kiểm tra bài hát đã có trong playlist chưa
            var alreadyExists = await _context.PlaylistSongs
                .AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == request.SongId);
            if (alreadyExists)
            {
                return BadRequest(new { message = "Bài hát này đã có trong danh sách playlist rồi" });
            }

            // Thêm vào bảng trung gian
            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = request.SongId,
                AddedAt = DateTime.Now
            };

            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã thêm bài hát vào danh sách phát thành công" });
        }

        // 5. DELETE: api/playlist/5/songs/3 (Gỡ bài ra khỏi playlist) 
        [HttpDelete("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var item = await _context.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (item == null)
            {
                return NotFound(new { message = "Bài hát không nằm trong danh sách này" });
            }

            _context.PlaylistSongs.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã gỡ bài hát khỏi danh sách phát" });
        }

        // 6. DELETE: api/playlists/5 (Xóa luôn cả playlist)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (playlist == null)
            {
                return NotFound(new { message = "Không tìm thấy danh sách để xóa" });
            }

            if (playlist.PlaylistSongs.Any())
            {
                _context.PlaylistSongs.RemoveRange(playlist.PlaylistSongs);
            }
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa danh sách thành công" });
        }
    }

    // Các DTOs nhập dữ liệu
    public class CreatePlaylistRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int UserId { get; set; }
    }

    public class AddSongRequest
    {
        [Range(1, int.MaxValue)]
        public int SongId { get; set; }
    }
}
