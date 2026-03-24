namespace Wrap.Data.Repository;

public class BaseRepository : IDisposable
{
    private bool isDisposed = false;
    private readonly FilmProductionDbContext? dbContext;

    protected BaseRepository(FilmProductionDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    protected FilmProductionDbContext? Context 
        => dbContext;

    protected async Task<int> SaveChangesAsync()
    {
        int affectedRows = await Context!.SaveChangesAsync();
        return affectedRows;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                dbContext?.Dispose();
            }
        }

        isDisposed = true;
    }
}