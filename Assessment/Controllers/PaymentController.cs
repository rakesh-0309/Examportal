//using Assessment.Data;
//using Assessment.Model;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Stripe;
//using System.Security.Claims;
//using static Assessment.DTO.PaymentDtos;

//namespace Assessment.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class PaymentsController : ControllerBase
//    {
//        private readonly AppDbContext _db;
//        private readonly IConfiguration _config;

//        public PaymentsController(AppDbContext db, IConfiguration config)
//        {
//            _db = db;
//            _config = config;
//        }

//        // Create Stripe PaymentIntent for a submission
//        [Authorize]
//        [HttpPost("intent")]
//        public async Task<ActionResult<CreatePaymentResponse>> CreateIntent(CreatePaymentRequest req)
//        {
//            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
//            var submission = await _db.Submissions.Include(s => s.Form).FirstOrDefaultAsync(s => s.Id == req.SubmissionId);
//            if (submission == null) return NotFound("Submission not found.");
//            if (submission.UserId != uid) return Forbid();

//            var amountMinor = (long)Math.Round(submission.Form.Price * 100m);
//            var payment = new Payment
//            {
//                SubmissionId = submission.Id,
//                AmountMinor = amountMinor,
//                Currency = submission.Form.Currency,
//                Provider = "Stripe",
//                Status = "Pending"
//            };
//            _db.Payments.Add(payment);
//            await _db.SaveChangesAsync();

//            var options = new PaymentIntentCreateOptions
//            {
//                Amount = payment.AmountMinor,
//                Currency = payment.Currency,
//                Metadata = new Dictionary<string, string>
//            {
//                { "PaymentId", payment.Id.ToString() },
//                { "SubmissionId", submission.Id.ToString() }
//            },
//                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
//                {
//                    Enabled = true
//                }
//            };
//            var service = new PaymentIntentService();
//            var intent = await service.CreateAsync(options);

//            payment.ProviderPaymentId = intent.Id;
//            payment.Status = intent.Status ?? "requires_payment_method";
//            await _db.SaveChangesAsync();

//            return Ok(new CreatePaymentResponse(payment.Id, intent.ClientSecret!));
//        }

//        // Stripe webhook to capture successful payments
//        [AllowAnonymous]
//        [HttpPost("stripe/webhook")]
//        public async Task<IActionResult> StripeWebhook()
//        {
//            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
//            var sigHeader = Request.Headers["Stripe-Signature"];
//            var endpointSecret = _config["Stripe:WebhookSecret"];

//            Event stripeEvent;
//            try
//            {
//                stripeEvent = EventUtility.ConstructEvent(json, sigHeader, endpointSecret);
//            }
//            catch
//            {
//                return BadRequest();
//            }

//            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
//            {
//                var intent = stripeEvent.Data.Object as PaymentIntent;
//                var providerPaymentId = intent!.Id;

//                var payment = await _db.Payments.FirstOrDefaultAsync(p => p.ProviderPaymentId == providerPaymentId);
//                if (payment != null)
//                {
//                    payment.Status = "Succeeded";
//                    await _db.SaveChangesAsync();
//                }
//            }
//            else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
//            {
//                var intent = stripeEvent.Data.Object as PaymentIntent;
//                var providerPaymentId = intent!.Id;

//                var payment = await _db.Payments.FirstOrDefaultAsync(p => p.ProviderPaymentId == providerPaymentId);
//                if (payment != null)
//                {
//                    payment.Status = "Failed";
//                    await _db.SaveChangesAsync();
//                }
//            }

//            return Ok();
//        }

//        // Admin dashboard aggregates
//        [Authorize(Roles = "Admin")]
//        [HttpGet("dashboard")]
//        public async Task<ActionResult<object>> Dashboard()
//        {
//            var totalForms = await _db.Forms.CountAsync();
//            var totalSubmissions = await _db.Submissions.CountAsync();
//            var totalPayments = await _db.Payments.CountAsync();
//            var totalSucceeded = await _db.Payments.Where(p => p.Status == "Succeeded").CountAsync();
//            var amountMinor = await _db.Payments.Where(p => p.Status == "Succeeded").SumAsync(p => (long?)p.AmountMinor) ?? 0;

//            return Ok(new
//            {
//                totalForms,
//                totalSubmissions,
//                totalPayments,
//                totalSucceeded,
//                totalAmount = amountMinor / 100m
//            });
//        }
//    }

//}

