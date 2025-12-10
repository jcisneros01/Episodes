using System;
using System.Collections.Generic;

namespace Episodes.Models.Entities;

public partial class episode
{
    public int id { get; set; }

    public int season_id { get; set; }

    public int episode_number { get; set; }

    public string name { get; set; } = null!;

    public string? overview { get; set; }

    public DateOnly? air_date { get; set; }

    public int external_id { get; set; }

    public int data_provider_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual tv_data_provider data_provider { get; set; } = null!;

    public virtual season season { get; set; } = null!;

    public virtual ICollection<watched_episode> watched_episodes { get; set; } = new List<watched_episode>();
}
