using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Form : BaseEntity
    {
        public Guid FormId { get; set; }
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
        public DateTime FilledDate { get; set; } = DateTime.UtcNow;
        
        public Template Template { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}