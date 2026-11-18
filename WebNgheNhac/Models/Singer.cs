using System;
using System.Collections.Generic;

namespace WebNgheNhac.Models;

public partial class Singer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? Bio { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}
