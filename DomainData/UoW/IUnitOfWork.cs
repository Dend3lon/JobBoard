using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainData.Entities;
using DomainData.Repository;

namespace DomainData.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Resume> ResumeRepo { get; }
        IGenericRepository<Vacancy> VacancyRepo { get; }
        IGenericRepository<Application> ApplicationRepo { get; }
        public void Save();
    }
}
