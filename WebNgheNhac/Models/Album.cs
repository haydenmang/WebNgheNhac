using System;
using System.Collections.Generic;

namespace WebNgheNhac.Models;

public partial class Album
{
    public int Id { get; set; }

    public string AlbumName { get; set; } = null!;

    public string? CoverUrl { get; set; }

    public int SingerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Singer Singer { get; set; } = null!;

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}
