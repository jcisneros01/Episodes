using System;
using System.Collections.Generic;

namespace Episodes.Models.Entities;

public partial class user_show
{
    public int user_id { get; set; }

    public int show_id { get; set; }

    public string status { get; set; } = null!;

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual show show { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}
