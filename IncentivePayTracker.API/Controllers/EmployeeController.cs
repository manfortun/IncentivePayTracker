using AutoMapper;
using IncentivePayTracker.API.DataAccess;
using IncentivePayTracker.API.Models;
using IncentivePayTracker.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IncentivePayTracker.API.Controllers;

[ApiController]
[Route("/API/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmployeeController(
        AppDbContext dbContext,
        IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(dbContext);
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var employees = _unitOfWork.Employees.GetAll();

        var mapping = _mapper.Map<IEnumerable<DTO.Employee>>(employees);

        return Ok(mapping);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var employee = _unitOfWork.Employees.Get(id);

        if (employee is null)
        {
            return NotFound();
        }

        var mapping = _mapper.Map<DTO.Employee>(employee);

        return Ok(mapping);
    }

    [HttpGet("CompleteInformation/{id}")]
    public IActionResult GetCompleteInformation(int id)
    {
        var employee = _unitOfWork.Employees.Get(id);

        if (employee is null)
        {
            return NotFound();
        }

        var employeeMapped = _mapper.Map<DTO.Employee>(employee);

        var timeIns = _unitOfWork.TimeIns.GetAll(timeIn => timeIn.EmployeeId == id);

        var timeInsMapped = _mapper.Map<IEnumerable<DTO.EmployeeTimeIn>>(timeIns);

        var employmentDates = _unitOfWork.EmploymentDates.GetAll(date => date.EmployeeId == id);

        var employmentDatesMapped = _mapper.Map<IEnumerable<DTO.EmploymentDate>>(employmentDates);

        var record = new EmployeeCompleteInformation
        {
            Employee = employeeMapped,
            TimeIns = timeInsMapped.ToList(),
            EmploymentDates = employmentDatesMapped.ToList()
        };

        return Ok(record);
    }

    [HttpPatch]
    public IActionResult Update(EmployeeCompleteInformation empInfo)
    {
        // update employee information
        var employee = _unitOfWork.Employees.Get(empInfo.Employee.Id);

        if (employee is null) return NotFound();

        employee.FirstName = empInfo.Employee.FirstName.ToUpper();
        employee.LastName = empInfo.Employee.LastName.ToUpper();

        // save employee information
        _unitOfWork.Employees.Update(employee);
        _unitOfWork.Save();

        // delete previous employment dates
        var employmentDates = _unitOfWork.EmploymentDates.GetAll(d => d.EmployeeId == employee.Id);
        foreach (var date in employmentDates)
        {
            _unitOfWork.EmploymentDates.Delete(date);
        }

        // insert new employment dates
        if (empInfo.EmploymentDates is not null)
        {
            foreach (var ed in empInfo.EmploymentDates)
            {
                _unitOfWork.EmploymentDates.Insert(_mapper.Map<Models.EmploymentDate>(ed));
            }
        }

        _unitOfWork.Save();

        // delete previous time in records
        var timeIns = _unitOfWork.TimeIns.GetAll(t => t.EmployeeId == empInfo.Employee.Id);
        foreach (var time in timeIns)
        {
            _unitOfWork.TimeIns.Delete(time);
        }

        // insert new time in records
        if (empInfo.TimeIns is not null)
        {
            foreach (var time in empInfo.TimeIns)
            {
                var mapped = _mapper.Map<Models.EmployeeTimeIn>(time);
                mapped.EmployeeId = employee.Id;
                _unitOfWork.TimeIns.Insert(mapped);
            }
        }

        _unitOfWork.Save();
        return Ok();
    }
}
