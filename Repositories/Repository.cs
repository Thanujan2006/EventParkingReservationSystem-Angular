using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext Db;
    protected readonly DbSet<T> Set;

    public Repository(ApplicationDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public IQueryable<T> Query() => Set.AsQueryable();

    public async Task<T?> GetByIdAsync(params object[] keyValues)
        => await Set.FindAsync(keyValues);

    public async Task AddAsync(T entity) => await Set.AddAsync(entity);
    public async Task AddRangeAsync(IEnumerable<T> entities) => await Set.AddRangeAsync(entities);
    public void Update(T entity) => Set.Update(entity);
    public void Remove(T entity) => Set.Remove(entity);
    public void RemoveRange(IEnumerable<T> entities) => Set.RemoveRange(entities);
}
