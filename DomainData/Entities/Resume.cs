using System.Diagnostics;
using System.Text.Json.Serialization;

namespace DomainData.Entities
{
    public class Resume
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }

}
