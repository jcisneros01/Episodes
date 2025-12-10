using System;
using System.Collections.Generic;

namespace Episodes.Models.Entities;

public partial class watched_episode
{
    public int user_id { get; set; }

    public int episode_id { get; set; }

    public DateTime watched_at { get; set; }

    public virtual episode episode { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}
