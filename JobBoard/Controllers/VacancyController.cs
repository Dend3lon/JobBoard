using AutoMapper;
using BusinessModels;
using DtoModels;
using JobBoardBll.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VacancyController : ControllerBase
{
    private readonly VacancyService _vacancyService;
    private readonly IMapper _mapper;

    public VacancyController(VacancyService vacancyService, IMapper mapper)
    {
        _vacancyService = vacancyService;
        _mapper = mapper;
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Employer, Admin")]
    public IActionResult CreateResume([FromBody] VacancyDto dto)
    {
        _vacancyService.CreateVacancy(_mapper.Map<VacancyBusinessModel>(dto));
        return Ok();
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public IActionResult Search([FromQuery] string query)
    {
        var results = _vacancyService.SearchVacancies(query);
        return Ok(results);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        _vacancyService.DeleteVacancy(id);
        return Ok();
    }

    [HttpGet("all")]
    [AllowAnonymous]
    public IActionResult GetAll()
    {
        var resumes = _vacancyService.GetAllVacancy();
        return Ok(resumes);
    }

    [HttpGet("GetVacancyApplications/{id}")]
    [Authorize(Roles = "Employer, Admin")]
    public IActionResult GetVacancyApplications(int id)
    {
        try
        {
            var applications = _vacancyService.GetVacancyApplications(id);
            return Ok(applications);
        }
        catch (Exception ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }
}
