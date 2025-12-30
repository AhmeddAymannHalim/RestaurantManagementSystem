using System.ComponentModel.DataAnnotations.Schema;
using RestaurantManageSystem.Domain._Common;

namespace RestaurantManageSystem.Domain.Entities;

public class Setting : BaseEntity
{
    [Column("Key")]
    public string Key { get; set; } = string.Empty;

    [Column("Value")]
    public string Value { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}