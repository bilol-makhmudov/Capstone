using System;
using System.ComponentModel.DataAnnotations;

namespace Capstone.Models
{
    public class Comment : BaseEntity
    {
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public Template Template { get; set; }
        public ApplicationUser User { get; set; }
    }
}