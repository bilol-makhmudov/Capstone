using Newtonsoft.Json;

namespace Capstone.Models
{
    public class TemplateTag
    { 
        public Guid TemplateId { get; set; } 
        public Guid TagId { get; set; }
        [JsonIgnore]
        public Template Template { get; set; }
        public Tag Tag { get; set; }
    }
}