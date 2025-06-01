using AutoMapper;
using BusinessModels;
using DtoModels;
using JobBoardBll.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResumeController : ControllerBase
{
    private readonly ResumeService _resumeService;
    private readonly IMapper _mapper;

    public ResumeController(ResumeService resumeService, IMapper mapper)
    {
        _resumeService = resumeService;
        _mapper = mapper;
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Worker, Admin")]
    public IActionResult CreateResume([FromBody] ResumeDto dto)
    {
        _resumeService.CreateResume(_mapper.Map<ResumeBusinessModel>(dto));
        return Ok();
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public IActionResult Search([FromQuery] string query)
    {
        var results = _resumeService.SearchResumes(query);
        return Ok(results);
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        _resumeService.DeleteResume(id);
        return Ok();
    }

    [HttpGet("all")]
    [AllowAnonymous]
    public IActionResult GetAll()
    {
        var resumes = _resumeService.GetAllResumes();
        return Ok(resumes);
    }

    [HttpGet("GetResumeApplications/{id}")]
    [Authorize(Roles = "Worker, Admin")]
    public IActionResult GetResumeApplications(int id)
    {
        try
        {
            var applications = _resumeService.GetResumeApplications(id);
            return Ok(applications);
        }
        catch (Exception ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }
}