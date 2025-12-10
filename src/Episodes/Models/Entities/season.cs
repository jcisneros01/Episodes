using System;
using System.Collections.Generic;

namespace Episodes.Models.Entities;

public partial class season
{
    public int id { get; set; }

    public int show_id { get; set; }

    public int season_number { get; set; }

    public string? name { get; set; }

    public string? overview { get; set; }

    public DateOnly? air_date { get; set; }

    public string? poster_img_link { get; set; }

    public int external_id { get; set; }

    public int data_provider_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual tv_data_provider data_provider { get; set; } = null!;

    public virtual ICollection<episode> episodes { get; set; } = new List<episode>();

    public virtual show show { get; set; } = null!;
}
