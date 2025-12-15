namespace Propman.Services
{
    public interface IInvoiceJobService
{
        Task CheckAndNotifyOverdueInvoices();
        Task SendPaymentReminders();
        Task GenerateInvoice();
}
}