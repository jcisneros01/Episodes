using System;
using System.Collections.Generic;

namespace Episodes.Models.Entities;

public partial class user
{
    public int id { get; set; }

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<user_show> user_shows { get; set; } = new List<user_show>();

    public virtual ICollection<watched_episode> watched_episodes { get; set; } = new List<watched_episode>();
}
