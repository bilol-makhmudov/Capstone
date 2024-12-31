using System.ComponentModel.DataAnnotations;

namespace Capstone.Models;

public interface IBaseEntity
{
    Guid Id { get; set; } 
    DateTime? CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    Guid? UpdatedBy { get; set; }
    bool? IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    Guid? DeletedBy { get; set; }
}

public abstract class BaseEntity : IBaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid? UpdatedBy { get; set; }
    public bool? IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}