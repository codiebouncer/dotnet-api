public interface ITransactionService
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    Task ExecuteAsync(Func<Task> action);
}
