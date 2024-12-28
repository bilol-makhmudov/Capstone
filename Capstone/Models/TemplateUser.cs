namespace Capstone.Models
{
    public class TemplateUser : BaseEntity
    {
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Template Template { get; set; }
    }
}