using System;
using System.Collections.Generic;

namespace ProjectGraduation.Models;

public partial class Profile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string? HintAboutMe { get; set; }

    public byte[]? ForegroundImage { get; set; }

    public byte[]? BackgroundImage { get; set; }
    public string Profession { get; set; }
    public int? Age { get; set; }

    public string? Address { get; set; }
    public string ClientId { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;
    public virtual List<Post> Posts { get; set; }
}
