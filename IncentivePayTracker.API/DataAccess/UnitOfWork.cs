using IncentivePayTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IncentivePayTracker.API.DataAccess;

public class UnitOfWork : IDisposable
{
    private bool _isDisposed = false;
    private readonly DbContext _dbContext;

    private BaseRepository<Employee> _employees = default!;
    private BaseRepository<Infraction> _infractions = default!;
    private BaseRepository<EmployeeInfraction> _employeeInfractions = default!;
    private BaseRepository<EmploymentDate> _employmentDates = default!;
    private BaseRepository<EmployeeTimeIn> _timeIns = default!;

    public BaseRepository<Employee> Employees => _employees ??= new BaseRepository<Employee>(_dbContext);
    public BaseRepository<Infraction> Infractions => _infractions ??= new BaseRepository<Infraction>(_dbContext);
    public BaseRepository<EmployeeInfraction> EmployeeInfractions => _employeeInfractions ??= new BaseRepository<EmployeeInfraction>(_dbContext);
    public BaseRepository<EmploymentDate> EmploymentDates => _employmentDates ??= new BaseRepository<EmploymentDate>(_dbContext);
    public BaseRepository<EmployeeTimeIn> TimeIns => _timeIns ??= new BaseRepository<EmployeeTimeIn>(_dbContext);

    public UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int Save()
    {
        return _dbContext.SaveChanges();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool dispose)
    {
        if (dispose && !_isDisposed)
        {
            _dbContext.Dispose();
        }

        _isDisposed = true;
    }
}
