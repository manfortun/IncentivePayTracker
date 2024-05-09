using AutoMapper;
using IncentivePayTracker.API.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace IncentivePayTracker.API.Controllers;

[ApiController]
[Route("/API/[controller]")]
public class InfractionController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InfractionController(
        AppDbContext dbContext,
        IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(dbContext);
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var infractions = _unitOfWork.Infractions.GetAll();

        var mapping = _mapper.Map<IEnumerable<DTO.Infraction>>(infractions);

        return Ok(mapping);
    }

    [HttpGet("{year}/{id}")]
    public IActionResult Get(int year, int id)
    {
        var compositeInfractions = new List<DTO.AnnualInfractionSummary>();
        var infractions = _unitOfWork.EmployeeInfractions.GetAll(ei => ei.EmployeeId == id && ei.Year == year);

        if (infractions is not null)
        {
            foreach (var month in Enumerable.Range(1, 12))
            {
                foreach (var infraction in infractions.Where(i => i.Month == month))
                {
                    var compInfr = compositeInfractions.FirstOrDefault(c => c.Month == month && c.InfractionId == infraction.InfractionId);

                    if (compInfr is null)
                    {
                        compInfr = new DTO.AnnualInfractionSummary
                        {
                            Year = year,
                            Month = month,
                            InfractionId = infraction.InfractionId,
                            AmountPerInfraction = infraction.Infraction.Amount,
                        };
                    }

                    compInfr.Count += infraction.Count;
                    compositeInfractions.Add(compInfr);
                }
            }
        }

        return Ok(compositeInfractions);
    }
}
