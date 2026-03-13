using Episodes.Enums;

namespace Episodes.Data;

public class UserShow
{
    public int UserId { get; set; }

    public int ShowId { get; set; }

    public UserShowStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Show Show { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}