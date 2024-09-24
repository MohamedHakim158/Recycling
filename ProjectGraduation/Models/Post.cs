using System;
using System.Collections.Generic;

namespace ProjectGraduation.Models;

public partial class Post
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public byte[] Image { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CategoryName { get; set; }
    public bool isAvailable { get; set;} = true;
    public string ClientId { get; set; } = null!;
    public string ProfileId { get; set; }
    public virtual Client Client { get; set; } = null!;
    public virtual Profile Profile { get; set; }

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
