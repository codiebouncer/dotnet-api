namespace PropMan.Models;
public class ErrorLog
{
    public Guid ErrorLogId { get; set; } = Guid.NewGuid();
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Path { get; set; }
    public DateTime Timestamp { get; set; }
}
