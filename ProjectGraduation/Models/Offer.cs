using System;
using System.Collections.Generic;

namespace ProjectGraduation.Models;

public partial class Offer
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Description { get; set; } = null!;

    public DateTime AddedAt { get; set; }

    public string? ClientId { get; set; } 

    public string? PostId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Post? Post { get; set; }
}
