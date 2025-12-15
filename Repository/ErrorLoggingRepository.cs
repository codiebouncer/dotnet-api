using Microsoft.EntityFrameworkCore;
using PropMan.Models;

public class ErrorLogRepository : IErrorLogRepository
{
    private readonly DataContext _context;

    public ErrorLogRepository(DataContext context)
    {
        _context = context;
    }

    public async Task LogAsync(ErrorLog log)
    {
        try
        {
            _context.ErrorLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch
        {
            // Failsafe: We DO NOT throw inside error logging.
            // If logging fails, avoid crashing the main request pipeline.
        }
    }
}
