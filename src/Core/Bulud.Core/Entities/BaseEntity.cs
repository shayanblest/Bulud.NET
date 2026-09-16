using System.Text.Json.Serialization;

namespace Bulud.Core.Entities;

public class BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    [JsonIgnore]
    public DateTime? DeletedAt { get; set; }

}
