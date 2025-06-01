using AutoMapper;
using BusinessModels;
using DomainData.Entities;
using DomainData.UoW;
using Microsoft.EntityFrameworkCore;

namespace JobBoardBll.Services
{
    public class VacancyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VacancyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void CreateVacancy(VacancyBusinessModel model)
        {
            var entity = _mapper.Map<Vacancy>(model);
            _unitOfWork.VacancyRepo.Create(entity);
            _unitOfWork.Save();
        }

        public void DeleteVacancy(int id)
        {
            var vacancy = _unitOfWork.VacancyRepo.GetById(id);
            if (vacancy != null)
            {
                var relatedApplications = _unitOfWork.ApplicationRepo.GetAll()
                .Where(a => a.VacancyId == id)
                .ToList();

                foreach (var app in relatedApplications)
                {
                    _unitOfWork.ApplicationRepo.Delete(app.Id);
                }
                _unitOfWork.VacancyRepo.Delete(id);
                _unitOfWork.Save();
            }
        }

        public List<VacancyBusinessModel> SearchVacancies(string searchTerm)
        {
            var entities = _unitOfWork.VacancyRepo
                .GetAll()
                .ToList()
                .Where(v => v.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           v.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(v => _mapper.Map<VacancyBusinessModel>(v))
                .ToList();
            return entities;
        }
        
        public List<VacancyBusinessModel> GetAllVacancy()
        {
            var vacancies = _unitOfWork.VacancyRepo.GetAll();
            return _mapper.Map<List<VacancyBusinessModel>>(vacancies);
        }

        public List<Application> GetVacancyApplications(int vacancyId)
        {
            return _unitOfWork.ApplicationRepo.GetAll()
        .Where(a => a.VacancyId == vacancyId)
        .Include(a => a.Resume)
        .Include(a => a.Vacancy)
        .ToList();
        }
    }
}
