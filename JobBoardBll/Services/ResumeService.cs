using AutoMapper;
using BusinessModels;
using DomainData.Entities;
using DomainData.UoW;
using Microsoft.EntityFrameworkCore;

namespace JobBoardBll.Services
{
    public class ResumeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ResumeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void CreateResume(ResumeBusinessModel model)
        {
            var entity = _mapper.Map<Resume>(model);
            _unitOfWork.ResumeRepo.Create(entity);
            _unitOfWork.Save();
        }

        public void DeleteResume(int id)
        {
            var resume = _unitOfWork.ResumeRepo.GetById(id);

            if (resume != null)
            {
                var relatedApplications = _unitOfWork.ApplicationRepo.GetAll()
                .Where(a => a.ResumeId == id)
                .ToList();

                foreach (var app in relatedApplications)
                {
                    _unitOfWork.ApplicationRepo.Delete(app.Id);
                }
                _unitOfWork.ResumeRepo.Delete(id);
                _unitOfWork.Save();
            }
            else
            {
                throw new Exception("Resume not found");
            }
        }

        public List<ResumeBusinessModel> SearchResumes(string searchTerm)
        {
            var entities = _unitOfWork.ResumeRepo
                .GetAll()
                .ToList()
                .Where(r => r.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           r.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(r => _mapper.Map<ResumeBusinessModel>(r))
                .ToList();
            return entities;
        }

        public List<ResumeBusinessModel> GetAllResumes()
        {
            var resumes = _unitOfWork.ResumeRepo.GetAll();
            return _mapper.Map<List<ResumeBusinessModel>>(resumes);
        }

        public List<Application> GetResumeApplications(int resumeId)
        {
            return _unitOfWork.ApplicationRepo.GetAll()
        .Where(a => a.ResumeId == resumeId)
        .Include(a => a.Resume)
        .Include(a => a.Vacancy)
        .ToList();
        }
    }
}
