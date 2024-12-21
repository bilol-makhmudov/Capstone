namespace Capstone.Models;

public class Template : BaseEntity
{
    public Guid TemplateId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public Guid TopicId { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? ImageUrl { get; set; }
    public Guid UserId { get; set; }
    
    public Topic Topic { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<TemplateTag> TemplateTags { get; set; } = new List<TemplateTag>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<Form> Forms { get; set; } = new List<Form>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<TemplateUser> TemplateUsers { get; set; } = new List<TemplateUser>();
}