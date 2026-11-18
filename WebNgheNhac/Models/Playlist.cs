using System;
using System.Collections.Generic;

namespace WebNgheNhac.Models;

public partial class Playlist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

    public virtual User User { get; set; } = null!;
}
