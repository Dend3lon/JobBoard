using DomainData.Entities;
using DomainData.Repository;

namespace DomainData.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly JobBoardContext _context;

        public IGenericRepository<Resume> _resumeRepo { get; }
        public IGenericRepository<Vacancy> _vacancieRepo { get; }
        public IGenericRepository<Application> _applicationRepo { get; }

        public UnitOfWork(JobBoardContext context)
        {
            _context = context;
        }
        public IGenericRepository<Resume> ResumeRepo => _resumeRepo ?? new GenericRepository<Resume>(_context);
        public IGenericRepository<Vacancy> VacancyRepo => _vacancieRepo ?? new GenericRepository<Vacancy>(_context);
        public IGenericRepository<Application> ApplicationRepo => _applicationRepo ?? new GenericRepository<Application>(_context);
        public void Save()
        {
            _context.SaveChanges();
        }
        public void Dispose() => _context.Dispose();
    }

}
