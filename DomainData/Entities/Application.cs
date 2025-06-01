
namespace DomainData.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public int VacancyId { get; set; }
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public Resume Resume { get; set; }
        public Vacancy Vacancy { get; set; }
    }
}
