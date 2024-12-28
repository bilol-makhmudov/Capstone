namespace Capstone.Models
{
    public class Like : BaseEntity
    {
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
        
        public Template Template { get; set; }
        public ApplicationUser User { get; set; }
    }
}