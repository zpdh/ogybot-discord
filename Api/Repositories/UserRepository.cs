using Microsoft.EntityFrameworkCore;

namespace test.Api.Repositories;

public abstract class UserRepository<T> where T : class
{
    
    //TODO: Add repository validations in case any exceptions occur.

    protected readonly DataContext DataContext;

    protected UserRepository(DataContext dataContext)
    {
        DataContext = dataContext;
    }

    public async Task<List<T>> SelectAllAsync()
    {
        return await DataContext.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task InsertAsync(T entity)
    {
        await DataContext.Set<T>().AddAsync(entity);
    }

    public void Delete(T entity)
    {
        DataContext.Set<T>().Remove(entity);
    }
}