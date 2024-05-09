using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncentivePayTracker.API.DataAccess;

public class BaseRepository<T> where T : class
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;
    public BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null)
    {
        var set = _dbSet.AsQueryable();

        if (predicate is not null)
        {
            set = set.Where(predicate);
        }

        return [.. set];
    }

    public T? FirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.FirstOrDefault(predicate);
    }

    public T? Get(int id)
    {
        return _dbSet.Find(id);
    }

    public void Insert(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
    }

    public void Delete(int id)
    {
        T? entity = Get(id);

        if (entity is not null)
        {
            Delete(entity);
        }
    }
}
