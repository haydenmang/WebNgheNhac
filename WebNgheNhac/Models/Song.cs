using System;
using System.Collections.Generic;

namespace WebNgheNhac.Models;

public partial class Song
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string AudioUrl { get; set; } = null!;

    public string? CoverUrl { get; set; }

    public int Duration { get; set; }

    public int Plays { get; set; }

    public int SingerId { get; set; }

    public int TypeId { get; set; }

    public int? AlbumId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Album? Album { get; set; }

    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

    public virtual Singer Singer { get; set; } = null!;

    public virtual Type Type { get; set; } = null!;
}
