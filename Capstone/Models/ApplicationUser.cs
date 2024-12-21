using Microsoft.AspNetCore.Identity;

namespace Capstone.Models;
public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public ICollection<Form> Forms { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Template> Templates { get; set; }
    public ICollection<TemplateUser> TemplateUsers { get; set; }
}