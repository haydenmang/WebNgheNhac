using System;
using System.Collections.Generic;

namespace WebNgheNhac.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Avatar { get; set; }

    public int Level { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
