namespace test.Api.Repositories;

public class UnitOfWork
{
    private readonly DataContext _context = new();

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}