using System;
using System.Collections.Generic;

namespace Episodes.Models.Entities;

public partial class UserShow
{
    public int UserId { get; set; }

    public int ShowId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Show Show { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
