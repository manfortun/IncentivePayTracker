using AutoMapper;
using IncentivePayTracker.API.DataAccess;
using IncentivePayTracker.API.Models;
using IncentivePayTracker.API.Services;
using IncentivePayTracker.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IncentivePayTracker.API.Controllers;

[ApiController]
[Route("/API/[controller]")]
public class EmployeeInfractionController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmployeeInfractionController(
        AppDbContext dbContext,
        IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(dbContext);
        _mapper = mapper;
    }

    /// <summary>
    /// Obtain Incentive Pays from <paramref name="date"/>.
    /// </summary>
    /// <param name="date">Date of records to obtain. When not set, records of the current date is obtained.</param>
    /// <returns></returns>
    [HttpGet("{year}-{month}")]
    public IActionResult GetAll(int year, int month)
    {
        var employeeInfractions = new List<Models.EmployeeInfraction>();
        var records = _unitOfWork.EmployeeInfractions.GetAll(i => i.Year == year && i.Month == month);

        if (records?.Any() == true)
        {
            foreach (var record in records)
            {
                var employmentDates = _unitOfWork.EmploymentDates.GetAll(ed => ed.EmployeeId == record.EmployeeId);

                if (employmentDates?.Any() != true)
                {
                    employeeInfractions.Add(record);
                }
                else if (employmentDates.Any(ed => ed.IsEmployedDuring(year, month)))
                {
                    employeeInfractions.Add(record);
                }
            }
        }

        var mapped = _mapper.Map<IEnumerable<DTO.EmployeeInfraction>>(employeeInfractions);
        return Ok(mapped);
    }

    [HttpGet("Composite/{year}-{month}")]
    public IActionResult GetComposite(int year, int month)
    {
        var compEmpInfrList = new List<DTO.CompositeEmployeeInfraction>();
        var employees = _unitOfWork.Employees.GetAll();

        foreach (var employee in employees)
        {
            var employmentDates = _unitOfWork.EmploymentDates.GetAll(ed => ed.EmployeeId == employee.Id);

            if (employmentDates?.Any() == true)
            {
                if (!employmentDates.Any(ed => ed.IsEmployedDuring(year, month)))
                {
                    continue;
                }
            }

            var compEmpInfr = new DTO.CompositeEmployeeInfraction()
            {
                Employee = _mapper.Map<DTO.Employee>(employee)
            };

            compEmpInfr.Infractions = new List<DTO.CompositeInfraction>();
            var employeeInfraction = _unitOfWork.EmployeeInfractions.GetAll(ei => ei.EmployeeId == employee.Id && ei.Year == year && ei.Month == month);

            if (employeeInfraction?.Any() == true)
            {
                foreach (var infr in employeeInfraction)
                {
                    var compositeInfraction = new CompositeInfraction
                    {
                        Infraction = _mapper.Map<DTO.Infraction>(infr.Infraction),
                        Count = infr.Count
                    };

                    compEmpInfr.Infractions.Add(compositeInfraction);
                }
            }

            compEmpInfrList.Add(compEmpInfr);
        }

        return Ok(compEmpInfrList);
    }

    [HttpGet("Composite/{year}/{id}")]
    public IActionResult GetEmployeeInfractions(int year, int id)
    {
        var employee = _unitOfWork.Employees.Get(id);

        if (employee is null) return NotFound();

        var employmentDates = _unitOfWork.EmploymentDates.GetAll(ed => ed.EmployeeId == id);
        if (employmentDates?.Any() == true)
        {
            if (!employmentDates.Any(ed => ed.IsEmployedDuring(year)))
            {
                return NotFound();
            }
        }

        var employeeInfractions = _unitOfWork.EmployeeInfractions.GetAll(ei => ei.EmployeeId == id);

        var compEmpInfr = new DTO.CompositeEmployeeInfraction()
        {
            Employee = _mapper.Map<DTO.Employee>(employee)
        };

        compEmpInfr.Infractions = new List<DTO.CompositeInfraction>();
        var employeeInfraction = _unitOfWork.EmployeeInfractions.GetAll(ei => ei.EmployeeId == employee.Id);

        if (employeeInfraction?.Any() == true)
        {
            foreach (var infr in employeeInfraction)
            {
                var compositeInfraction = new CompositeInfraction
                {
                    Infraction = _mapper.Map<DTO.Infraction>(infr.Infraction),
                    Count = infr.Count
                };

                compEmpInfr.Infractions.Add(compositeInfraction);
            }
        }

        return Ok(compEmpInfr);
    }

    [HttpGet("Summary/{year}-{month}")]
    public IActionResult GetSummary(int year, int month)
    {
        var employees = _unitOfWork.Employees.GetAll();
        var infrSummaries = new Dictionary<int, InfractionSummary>();
        var records = _unitOfWork.EmployeeInfractions.GetAll(ei => ei.Month == month && ei.Year == year);

        if (records is not null)
        {
            foreach (var employee in employees)
            {
                if (!infrSummaries.TryGetValue(employee.Id, out var infr))
                {
                    infr = new InfractionSummary
                    {
                        EmployeeId = employee.Id,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        IsExempted = employee.IsExempted,
                    };

                    infr.TotalInfractionsAmount = 0;
                    infrSummaries.Add(infr.EmployeeId, infr);
                }

                var infractions = records.Where(r => r.EmployeeId == employee.Id);

                foreach (var infraction in infractions)
                {
                    infr.TotalInfractionsAmount += infraction.Count * infraction.Infraction.Amount;
                }
            }
        }

        return Ok(infrSummaries.Values?.ToList() ?? []);
    }

    [HttpGet("MonthlyInfraction/{month}-{year}/{id}")]
    public IActionResult GetMonthlyInfraction(int id, int year, int month)
    {
        var employee = _unitOfWork.Employees.Get(id);

        if (employee is null) return NotFound();

        var infractions = _unitOfWork.EmployeeInfractions.GetAll(ei => ei.EmployeeId == employee.Id && ei.Month == month && ei.Year == year);

        var compositeInfractions = new CompositeEmployeeInfraction
        {
            Employee = _mapper.Map<DTO.Employee>(employee),
            Infractions = new List<CompositeInfraction>(),
            Month = month,
            Year = year
        };

        if (infractions is not null)
        {
            foreach (var infr in infractions)
            {
                var compositeInfraction = compositeInfractions.Infractions.Find(i => i.Infraction.Id == infr.InfractionId);

                if (compositeInfraction is null)
                {
                    compositeInfraction = new CompositeInfraction
                    {
                        Infraction = new DTO.Infraction
                        {
                            Id = infr.InfractionId,
                            Amount = infr.Infraction.Amount,
                            Description = infr.Infraction.Description
                        },
                        Count = 0
                    };

                    compositeInfractions.Infractions.Add(compositeInfraction);
                }

                compositeInfraction.Count += infr.Count;
            }
        }

        return Ok(compositeInfractions);
    }

    [HttpPost]
    public IActionResult Add(CompositeEmployeeInfraction compEmpInfr)
    {
        DateOnly uploadDate = DateOnly.FromDateTime(new DateTime(compEmpInfr.Year, compEmpInfr.Month, 1));
        DateOnly lastDayOfTheMonth = uploadDate.AddDays(DateTime.DaysInMonth(uploadDate.Year, uploadDate.Month) - 1);
        // check if the employee already exists in the database
        Models.Employee? employee = _unitOfWork.Employees
            .FirstOrDefault(e => e.FirstName.ToLower() == compEmpInfr.Employee.FirstName.ToLower() &&
                e.LastName.ToLower() == compEmpInfr.Employee.LastName.ToLower());

        if (employee is null)
        {
            // add employee to database if not exist
            Models.Employee emp = _mapper.Map<Models.Employee>(compEmpInfr.Employee);

            _unitOfWork.Employees.Insert(emp);
            _unitOfWork.Save();
            employee = emp;
        }

        if (compEmpInfr.Infractions?.Any() == true)
        {
            foreach (var infraction in compEmpInfr.Infractions)
            {
                // check if existing
                Models.EmployeeInfraction? empInfr = _unitOfWork.EmployeeInfractions.FirstOrDefault(ei =>
                    ei.InfractionId == infraction.Infraction.Id && ei.EmployeeId == employee.Id
                    && ei.Month == compEmpInfr.Month && ei.Year == compEmpInfr.Year);


                if (empInfr is null)
                {
                    empInfr = new Models.EmployeeInfraction
                    {
                        EmployeeId = employee.Id,
                        InfractionId = infraction.Infraction.Id,
                        Count = infraction.Count,
                        Month = compEmpInfr.Month,
                        Year = compEmpInfr.Year
                    };

                    _unitOfWork.EmployeeInfractions.Insert(empInfr);
                }
                else
                {
                    empInfr.Count = infraction.Count;

                    _unitOfWork.EmployeeInfractions.Update(empInfr);
                }
            }
        }

        _unitOfWork.Save();

        return Ok(_mapper.Map<DTO.Employee>(employee));
    }
}
