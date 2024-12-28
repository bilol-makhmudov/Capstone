namespace Capstone.Models;

public class Topic : BaseEntity
{
    public required string TopicName { get; set; }
    
    public ICollection<Template> Templates { get; set; } = new List<Template>();
}