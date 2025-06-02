using DomainData.Entities;
using DomainData.UoW;
using BusinessModels;
using AutoMapper;

namespace JobBoardBll.Services
{
    public class ApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void CreateApplication(ApplicationBusinessModel model)
        {
            var resume = _unitOfWork.ResumeRepo.GetById(model.ResumeId);
            if (resume == null) throw new Exception("Resume not found");

            var vacancy = _unitOfWork.VacancyRepo.GetById(model.VacancyId);
            if (vacancy == null) throw new Exception("Vacancy not found");

            var application = new Application
            {
                ResumeId = resume.Id,
                VacancyId = vacancy.Id,
                AppliedDate = DateTime.UtcNow,
                Resume = resume,
                Vacancy = vacancy
            };

            _unitOfWork.ApplicationRepo.Create(application);
            _unitOfWork.Save();
        }

        public void DeleteApplication(int id)
        {
            var application = _unitOfWork.ApplicationRepo.GetById(id);
            if (application != null)
            {
                _unitOfWork.ApplicationRepo.Delete(id);
                _unitOfWork.Save();
            }
            else
            {
                throw new Exception("Application not found");
            }

        }
    }
}
