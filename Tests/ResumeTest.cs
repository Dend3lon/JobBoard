using AutoMapper;
using AutoMapperProfiles;
using BusinessModels;
using DomainData.Entities;
using DomainData.Repository;
using DomainData.UoW;
using JobBoardBll.Services;
using Moq;
using Xunit;

namespace Tests
{
    public class ResumeTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Resume>> _mockResumeRepo;
        private readonly Mock<IGenericRepository<Application>> _mockApplicationRepo;
        private readonly ResumeService _resumeService;

        public ResumeTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mockResumeRepo = new Mock<IGenericRepository<Resume>>();
            _unitOfWorkMock.Setup(uow => uow.ResumeRepo).Returns(_mockResumeRepo.Object);
            _mockApplicationRepo = new Mock<IGenericRepository<Application>>();
            _unitOfWorkMock.Setup(uow => uow.ApplicationRepo).Returns(_mockApplicationRepo.Object);


            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(AutoMapperProfile).Assembly);
            });
            var mapper = config.CreateMapper();

            _resumeService = new ResumeService(_unitOfWorkMock.Object, mapper);
        }

        [Fact]
        public void CreateResumeTest()
        {
            var resume = new ResumeBusinessModel
            {
                Title = "Test Resume",
                Description = "Test Description"
            };

            _resumeService.CreateResume(resume);

            _mockResumeRepo.Verify(x => x.Create(It.Is<Resume>(x =>
            resume.Title == x.Title &&
            resume.Description == x.Description
            )));
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void DeleteResumeTest()
        {
            var resume = new Resume
            {
                Id = 1,
                Title = "Test Vacancy"
            };
            var applications = new List<Application>
            {
                new Application { Id = 1, ResumeId = resume.Id },
                new Application { Id = 2, ResumeId = resume.Id }
            };

            _mockApplicationRepo.Setup(uow => uow.GetAll())
                .Returns(applications.AsQueryable());
            _mockResumeRepo.Setup(x => x.GetById(resume.Id)).Returns(new Resume { Id = resume.Id });

            _resumeService.DeleteResume(resume.Id);

            _mockResumeRepo.Verify(x => x.Delete(resume.Id), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void SearchResumesTest()
        {
            var resumes = new List<Resume>
            {
                new Resume { Id = 1, Title = "Test1", Description = "Test1" },
                new Resume { Id = 2, Title = "Test2", Description = "Test2" }
            };
            _mockResumeRepo.Setup(x => x.GetAll()).Returns(resumes.AsQueryable());
            var result = _resumeService.SearchResumes("Test1");

            Xunit.Assert.Equal("Test1", result[0].Title);
        }

        [Fact]
        public void GetAllResumesTest()
        {
            var resumes = new List<Resume>
            {
                new Resume { Id = 1, Title = "Resume1", Description = "Description1" },
                new Resume { Id = 2, Title = "Resume2", Description = "Description2" }
            };
            _mockResumeRepo.Setup(x => x.GetAll()).Returns(resumes.AsQueryable());
            var result = _resumeService.GetAllResumes();


            Xunit.Assert.Equal("Resume1", result[0].Title);
            Xunit.Assert.Equal("Resume2", result[1].Title);
        }

        [Fact]
        public void GetResumeApplicationsTest()
        {
            var resumeId = 1;
            var applications = new List<Application>
            {
                new Application { Id = 1, VacancyId = 1, ResumeId = resumeId },
                new Application { Id = 2, VacancyId = 2, ResumeId = resumeId }
            };
            _mockApplicationRepo.Setup(uow => uow.GetAll())
                .Returns(applications.AsQueryable());
            var result = _resumeService.GetResumeApplications(resumeId);

            Xunit.Assert.Equal(resumeId, result[0].ResumeId);
            Xunit.Assert.Equal(resumeId, result[1].ResumeId);
        }
    }
}
