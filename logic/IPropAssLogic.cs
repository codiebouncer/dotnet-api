namespace Propman.Logic
{
    public interface IPropAssLogic
    {
        Task RentAssignment(Guid planId, Guid tenantId, Guid unitId, Guid proptenantId);
        Task<(decimal outstanding, int totalInstallments, decimal installmentAmount)> WorkAndPay(
            Guid propId,
            decimal initdeposit,
            Guid proptenantId,
            Guid planId,
            DateTime StartDate,
            DateTime EndDate


            );
        Task WorkAndPay2(
            Guid proptenantId,
            Guid planId,
            DateTime StartDate


            );    
    }
}