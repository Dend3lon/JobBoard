using DomainData.Entities;
using DomainData.Repository;
using DomainData.UoW;
using JobBoardBll.Services;
using Moq;
using AutoMapperProfiles;
using Xunit;
using BusinessModels;
using AutoMapper;

namespace Tests
{
    public class VacancyTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Vacancy>> _vacancyRepoMock;
        private readonly Mock<IGenericRepository<Application>> _applicationRepoMock;
        private readonly VacancyService _vacancyService;

        public VacancyTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _vacancyRepoMock = new Mock<IGenericRepository<Vacancy>>();
            _unitOfWorkMock.Setup(uow => uow.VacancyRepo).Returns(_vacancyRepoMock.Object);
            _applicationRepoMock = new Mock<IGenericRepository<Application>>();
            _unitOfWorkMock.Setup(uow => uow.ApplicationRepo).Returns(_applicationRepoMock.Object);


            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(AutoMapperProfile).Assembly);
            });
            var mapper = config.CreateMapper();

            _vacancyService = new VacancyService(_unitOfWorkMock.Object, mapper); 
        }

        [Fact]
        public void CreateVacancyTest()
        {
            var vacancy = new VacancyBusinessModel
            {
                Title = "Test Vacancy",
                Description = "Test Description"
            };

            _vacancyService.CreateVacancy(vacancy);

            _vacancyRepoMock.Verify(x => x.Create(It.Is<Vacancy>(x=>
            vacancy.Title == x.Title &&
            vacancy.Description == x.Description
            )));
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void DeleteVacancyTest()
        {
            var vacancy = new Vacancy
            {
                Id = 1,
                Title = "Test Vacancy"
            };  
            var applications = new List<Application>
            {
                new Application { Id = 1, VacancyId = vacancy.Id },
                new Application { Id = 2, VacancyId = vacancy.Id }
            };

            _applicationRepoMock.Setup(uow => uow.GetAll())
                .Returns(applications.AsQueryable());
            _vacancyRepoMock.Setup(x => x.GetById(vacancy.Id)).Returns(new Vacancy { Id = vacancy.Id });

            _vacancyService.DeleteVacancy(vacancy.Id);

            _vacancyRepoMock.Verify(x => x.Delete(vacancy.Id), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void SearchVacanciesTest()
        {
            var vacancies = new List<Vacancy>
            {
                new Vacancy { Id = 1, Title = "Test1", Description = "Test1" },
                new Vacancy { Id = 2, Title = "Test2", Description = "Test2" }
            };
            _vacancyRepoMock.Setup(x => x.GetAll()).Returns(vacancies.AsQueryable());
            var result = _vacancyService.SearchVacancies("Test1");
            
            Xunit.Assert.Equal("Test1", result[0].Title);
        }

        [Fact]
        public void GetAllVacanciesTest()
        {
            var vacancies = new List<Vacancy>
            {
                new Vacancy { Id = 1, Title = "Vacancy1", Description = "Description1" },
                new Vacancy { Id = 2, Title = "Vacancy2", Description = "Description2" }
            };
            _vacancyRepoMock.Setup(x => x.GetAll()).Returns(vacancies.AsQueryable());
            var result = _vacancyService.GetAllVacancy();


            Xunit.Assert.Equal("Vacancy1", result[0].Title);
            Xunit.Assert.Equal("Vacancy2", result[1].Title);
        }

        [Fact]
        public void GetVacancyApplicationsTest()
        {
            var vacancyId = 1;
            var applications = new List<Application>
            {
                new Application { Id = 1, VacancyId = vacancyId, ResumeId = 1 },
                new Application { Id = 2, VacancyId = vacancyId, ResumeId = 2 }
            };
            _applicationRepoMock.Setup(uow => uow.GetAll())
                .Returns(applications.AsQueryable());
            var result = _vacancyService.GetVacancyApplications(vacancyId);
            
            Xunit.Assert.Equal(vacancyId, result[0].VacancyId);
            Xunit.Assert.Equal(vacancyId, result[1].VacancyId);
        }

    }
}
