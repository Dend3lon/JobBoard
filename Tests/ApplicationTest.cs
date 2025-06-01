using DomainData.Entities;
using DomainData.Repository;
using DomainData.UoW;
using Moq;
using JobBoardBll.Services;
using AutoMapper;
using AutoMapperProfiles;
using Xunit;
using BusinessModels;

namespace Tests
{
    public class ApplicationTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Application>> _applicationRepoMock;
        private readonly Mock<IGenericRepository<Resume>> _resumeRepoMock;
        private readonly Mock<IGenericRepository<Vacancy>> _vacancyRepoMock;
        private readonly ApplicationService _applicationService;

        public ApplicationTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _applicationRepoMock = new Mock<IGenericRepository<Application>>();
            _unitOfWorkMock.Setup(uow => uow.ApplicationRepo).Returns(_applicationRepoMock.Object);
            _resumeRepoMock = new Mock<IGenericRepository<Resume>>();
            _unitOfWorkMock.Setup(uow => uow.ResumeRepo).Returns(_resumeRepoMock.Object);
            _vacancyRepoMock = new Mock<IGenericRepository<Vacancy>>();
            _unitOfWorkMock.Setup(uow => uow.VacancyRepo).Returns(_vacancyRepoMock.Object);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(AutoMapperProfile).Assembly);
            });
            var mapper = config.CreateMapper();

            _applicationService = new ApplicationService(_unitOfWorkMock.Object, mapper);
        }

        [Fact]
        public void CreateApplicationTest()
        {
            var vacancy = new Vacancy { Id = 1, Title = "Test Vacancy" };
            _vacancyRepoMock.Setup(x => x.GetById(vacancy.Id)).Returns(vacancy);    
            var resume = new Resume { Id = 1, Title = "Test Resume" };  
            _resumeRepoMock.Setup(x => x.GetById(resume.Id)).Returns(resume);

            var application = new ApplicationBusinessModel
            {
                ResumeId = resume.Id,
                VacancyId = vacancy.Id
            };

            _applicationService.CreateApplication(application);
            
            _applicationRepoMock.Verify(x => x.Create(It.Is<Application>( x =>
            x.ResumeId == application.ResumeId &&
            x.VacancyId == application.VacancyId
            )));
            _unitOfWorkMock.Verify(x => x.Save(), Times.Once);
        }

        [Fact]
        public void DeleteApplivationTest()
        {
            var application = new Application { Id = 1 };
            _applicationRepoMock.Setup(x => x.GetById(application.Id)).Returns(application);

            _applicationService.DeleteApplication(application.Id);
            _applicationRepoMock.Verify(x => x.Delete(application.Id), Times.Once);
            _unitOfWorkMock.Verify(x => x.Save(), Times.Once);
        }
    }
}