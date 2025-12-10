using System;
using System.Collections.Generic;

namespace Episodes.Models.Entities;

public partial class show
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public DateOnly? premiered_date { get; set; }

    public DateOnly? ended_date { get; set; }

    public string status { get; set; } = null!;

    public string? poster_img_link { get; set; }

    public int external_id { get; set; }

    public int data_provider_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual tv_data_provider data_provider { get; set; } = null!;

    public virtual ICollection<season> seasons { get; set; } = new List<season>();

    public virtual ICollection<user_show> user_shows { get; set; } = new List<user_show>();

    public virtual ICollection<genre> genres { get; set; } = new List<genre>();

    public virtual ICollection<tv_network> networks { get; set; } = new List<tv_network>();
}
