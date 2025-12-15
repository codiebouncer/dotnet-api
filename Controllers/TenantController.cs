using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Propman.Services;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;
using PropMan.Services;



namespace Propman.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IInvoiceJobService _job;
        private readonly IEmailService _email;
        private readonly IPaymentService _payment;

        public TenantController(ITenantService tenantService, IInvoiceJobService job, IEmailService email,IPaymentService payment)
        {
            _tenantService = tenantService;
            _job = job;
            _email = email;
            _payment = payment;


        }
        [HttpPost("create-tenant")]
        [Authorize]
        public async Task<ActionResult<string>> CreateTenant(CreateTenant request)
        {
            var result = await _tenantService.CreateTenant(request);
            return Ok(result);
        }
        [HttpPost("delete-tenant/{id}")]
        [Authorize]
        public async Task<ActionResult<string>> DeleteTenant(Guid id)
        {
            var result = await _tenantService.DeleteTenant(id);
            return Ok(result);
        }
        [HttpGet("all-tenants")]
        public async Task<ActionResult<ApiResponse<List<Tenant>>>> GetTenant(int page, int pageSize, string? search,DateTime? startDate,DateTime? endDate)
        {
            var result = await _tenantService.GetAllTens(page, pageSize, search!,startDate,endDate);
            return Ok(result);
        }
        [HttpPut("update-tenant")]
        public async Task<ActionResult<string>> UpdateTenant(UpdateTenant request)
        {
            var result = await _tenantService.UpdateTenant(request);
            return Ok(result);
        }
        [HttpPost("make-payment")]
        public async Task<ActionResult<string>> MakePayment(PaymentRequest request)
        {
            var result = await _tenantService.MakePayment(request);
            return Ok(result);
        }
        [HttpGet("all-plans")]
        public async Task<ActionResult<string>> AllPlans()
        {
            var result = await _tenantService.AllPlans();
            return Ok(result);
        }
        [HttpGet("all-propertytenants")]
        public async Task<ActionResult<string>> AllPropTens()
        {
            var result = await _tenantService.AllPropTens();
            return Ok(result);
        }
        [HttpGet("invoices-by-propertytenant")]
        public async Task<ActionResult<string>> InvoiceByPropTenId(Guid proptenantId)
        {
            var result = await _tenantService.InvoiceByPropTenId(proptenantId);
            return Ok(result);
        }
        [HttpGet("payments-by-propertytenant")]
        public async Task<ActionResult<string>> AllPaymentsById(Guid proptenantId)
        {
            var result = await _tenantService.AllPaymentsById(proptenantId);
            return Ok(result);
        }


        [HttpPost("generate-invoices")]
        public async Task<ActionResult<string>> GenerateInvoice()
        {
            try
            {
                await _job.GenerateInvoice();
                return Ok(" Invoices generated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating invoices: {ex.Message}");
            }
        }
        [HttpPost("send-invoice")]
        public async Task<IActionResult> SendInvoiceEmail()
        {
            string to = "kofisakyi430@gmail.com";
            string subject = "Your Monthly Invoice";
            string body = "<h3>Hello!</h3><p>Your invoice for this month has been generated.</p>";

            await _email.SendEmail(to, subject, body);
            return Ok("Email sent successfully!");
        }
        [HttpGet("payments/{tenantId}")]
        public async Task<IActionResult> GetPayments(Guid tenantId,int page,int pageSize,DateTime? startDate,DateTime? endDate)
        {
            var result = await _tenantService.GetTenantPayments(tenantId, page, pageSize,startDate,endDate);
            return Ok(result);
        }
        [HttpGet("invoices/{tenantId}")]
        public async Task<IActionResult> GetInvoices(Guid tenantId,int page,int pageSize,DateTime? startDate,DateTime? endDate)
        {
            var result = await _tenantService.GetTenantInvoices(tenantId,page,pageSize,startDate,endDate);
            return Ok(result);
        }
        [HttpGet("all-payments")]
        public async Task<IActionResult> AllPayments(int page,int pageSize,DateTime? startDate,DateTime? endDate)
        {
            var result = await _payment.AllPayments(page,pageSize,startDate,endDate);
            return Ok(result);
        }
        [HttpGet("dropdown-tenant")]
        public async Task<IActionResult> TenantDropdown()
        {
            var result = await _tenantService.TenantDropdown();
            return Ok(result);
        }
    }
}
