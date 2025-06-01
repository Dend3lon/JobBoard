using JobBoardBll.Services;
using Microsoft.AspNetCore.Mvc;
using BusinessModels;
using DtoModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace JobBoard.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationService _applicationService;
        private readonly IMapper _mapper;

        public ApplicationsController(ApplicationService applicationService, IMapper mapper)
        {
            _applicationService = applicationService;
            _mapper = mapper;
        }

        [HttpPost("ApplyForVacancy/Resume")]
        [Authorize(Roles = "Employer, Worker, Admin")]
        public IActionResult Create([FromBody] ApplicationDto dto)
        {
            try
            {
                var applications = _mapper.Map<ApplicationBusinessModel>(dto);
                _applicationService.CreateApplication(applications);
                return Ok(new { Message = "Application submitted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _applicationService.DeleteApplication(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }
    }
}