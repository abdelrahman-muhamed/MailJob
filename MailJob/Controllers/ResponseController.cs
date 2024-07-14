using MailJob.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MailJob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {

        private readonly EmailService _emailService;

        public ResponseController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-approval-request")]
        public async Task<IActionResult> SendApprovalRequest([FromBody] EmailRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ToEmail))
            {
                return BadRequest("The toEmail field is required.");
            }

            await _emailService.SendApprovalRequest(request.ToEmail);
            return Ok("Approval request sent.");
        }

        [HttpGet("approve")]
        public IActionResult Approve()
        {
            // Handle approval logic here
            return Ok("You have approved.");
        }

        [HttpGet("reject")]
        public IActionResult Reject()
        {
            // Handle rejection logic here
            return Ok("You have rejected.");
        }
        public class EmailRequest
        {
            public string ToEmail { get; set; }
        }
    }
}
