using AutoMapper;
using IncentivePayTracker.API.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace IncentivePayTracker.API.Controllers;

[ApiController]
[Route("/API/[controller]")]
public class EmploymentDateController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmploymentDateController(
        AppDbContext dbContext,
        IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(dbContext);
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public IActionResult GetEmploymentDates(int id)
    {
        var dates = _unitOfWork.EmploymentDates.GetAll(ed => ed.EmployeeId == id);

        var mapping = _mapper.Map<IEnumerable<DTO.EmploymentDate>>(dates);

        return Ok(mapping);
    }

    [HttpGet("Current/{id}")]
    public IActionResult GetCurrentEmploymentDate(int id)
    {
        var date = _unitOfWork.EmploymentDates.GetAll(ed => ed.EmployeeId == id)?.MaxBy(ed => ed.YearHired);

        if (date is not null)
        {
            var mapping = _mapper.Map<DTO.EmploymentDate>(date);

            return Ok(mapping);
        }

        return NotFound();
    }
}
