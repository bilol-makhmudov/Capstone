using System.ComponentModel.DataAnnotations;

namespace Capstone.Models;

public abstract class BaseEntity
{
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    [Required]
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}