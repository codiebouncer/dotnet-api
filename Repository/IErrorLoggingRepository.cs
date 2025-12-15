using PropMan.Models;

public interface IErrorLogRepository
{
    Task LogAsync(ErrorLog log);
}
