using System;
using System.Collections.Generic;

namespace Episodes.Models.Entities;

public partial class genre
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<show> shows { get; set; } = new List<show>();
}
