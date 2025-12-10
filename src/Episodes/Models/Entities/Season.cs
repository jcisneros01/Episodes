using System;
using System.Collections.Generic;

namespace Episodes.Models.Entities;

public partial class Season
{
    public int Id { get; set; }

    public int ShowId { get; set; }

    public int SeasonNumber { get; set; }

    public string? Name { get; set; }

    public string? Overview { get; set; }

    public DateOnly? AirDate { get; set; }

    public string? PosterImgLink { get; set; }

    public int ExternalId { get; set; }

    public int DataProviderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TvDataProvider DataProvider { get; set; } = null!;

    public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();

    public virtual Show Show { get; set; } = null!;
}
