using PropMan.Models;
namespace Propman.Services;

public class TransactionService : ITransactionService
{
    private readonly DataContext _context;

    public TransactionService(DataContext context)
    {
        _context = context;
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var result = await action();

            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await action();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
