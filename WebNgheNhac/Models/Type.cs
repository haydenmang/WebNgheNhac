using System;
using System.Collections.Generic;

namespace WebNgheNhac.Models;

public partial class Type
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}
