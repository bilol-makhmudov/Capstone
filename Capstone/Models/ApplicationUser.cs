using Microsoft.AspNetCore.Identity;

namespace Capstone.Models;
public class ApplicationUser : IdentityUser<Guid>, IBaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Template> Templates { get; set; }
    public ICollection<TemplateUser> TemplateUsers { get; set; }

}