

//using Assessment.Data;
//using Assessment.DTO;
//using Assessment.Model;
//using Assessment.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Stripe;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using System.Security.Claims;
//using static Assessment.DTO.AutoDtos;
//using static Assessment.DTO.PaymentDtos;
//using static Assessment.DTO.SubmissionDtos;

//namespace Assessment.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class FormController : ControllerBase
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly JwtTokenService _JwtTokenService;
//        private readonly AppDbContext _db;
//        private readonly IConfiguration _config;
//        private readonly PdfService _pdf;
//        private readonly AQIService _aqi;

//        public FormController(
//            AppDbContext db,
//            IConfiguration config,
//            PdfService pdf,
//            AQIService aqi,
//            UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            JwtTokenService jwt)
//        {
//            _db = db;
//            _config = config;
//            _pdf = pdf;
//            _aqi = aqi;
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _JwtTokenService = jwt;
//        }

//        // ------------------------
//        // Auth
//        // ------------------------
//        [HttpPost("register")]
//        public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
//        {
//            var user = new ApplicationUser
//            {
//                UserName = request.Email,
//                Email = request.Email,
//                FullName = request.FullName,
//                EmailConfirmed = true
//            };

//            var result = await _userManager.CreateAsync(user, request.Password);
//            if (!result.Succeeded) return BadRequest(result.Errors);

//            await _userManager.AddToRoleAsync(user, RoleSeeder.UserRole);
//            var token = _JwtTokenService.Create(user, RoleSeeder.UserRole);
//            return Ok(new LoginResponse(token, user.Id, user.Email!, RoleSeeder.UserRole));
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginRequest req)
//        {
//            var user = await _userManager.FindByEmailAsync(req.Email);
//            if (user == null) return Unauthorized("Invalid credentials");

//            var validPassword = await _userManager.CheckPasswordAsync(user, req.Password);
//            if (!validPassword) return Unauthorized("Invalid credentials");

//            var roles = await _userManager.GetRolesAsync(user);
//            var role = roles.FirstOrDefault() ?? "User";

//            var token = _JwtTokenService.Create(user, role);
//            return Ok(new { token, role });
//        }

//        [Authorize]
//        [HttpGet("me")]
//        public async Task<ActionResult<object>> Me()
//        {
//            var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
//            var user = await _userManager.FindByIdAsync(id);
//            var roles = await _userManager.GetRolesAsync(user!);
//            return Ok(new { user!.Id, user.Email, Roles = roles });
//        }

//        // ------------------------
//        // Forms (public listing + admin CRUD)
//        // Base path: api/proj/forms
//        // ------------------------
//        [HttpGet("formsforall")]
//        public async Task<ActionResult<IEnumerable<Form>>> GetForms([FromQuery] bool onlyActive = true)
//        {
//            var q = _db.Forms.AsNoTracking();
//            if (onlyActive) q = q.Where(f => f.IsActive);
//            return Ok(await q.OrderBy(f => f.Title).ToListAsync());
//        }

//        [HttpGet("forms/{id:guid}")]
//        public async Task<ActionResult<Form>> GetFormById(Guid id)
//        {
//            var form = await _db.Forms.FindAsync(id);
//            if (form == null) return NotFound();
//            return Ok(form);
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpPost("formsforadmin")]
//        public async Task<ActionResult<Form>> CreateForm(CreateFormRequest req)
//        {
//            var form = new Form
//            {
//                Title = req.Title,
//                Description = req.Description,
//                FieldsJson = req.FieldsJson,
//                Price = req.Price,
//                Currency = req.Currency
//            };
//            _db.Forms.Add(form);
//            await _db.SaveChangesAsync();
//            return CreatedAtAction(nameof(GetFormById), new { id = form.Id }, form);
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpPut("forms/{id:guid}")]
//        public async Task<ActionResult> UpdateForm(Guid id, UpdateFormRequest req)
//        {
//            var form = await _db.Forms.FindAsync(id);
//            if (form == null) return NotFound();

//            form.Title = req.Title;
//            form.Description = req.Description;
//            form.FieldsJson = req.FieldsJson;
//            form.IsActive = req.IsActive;
//            form.Price = req.Price;
//            form.Currency = req.Currency;
//            await _db.SaveChangesAsync();
//            return NoContent();
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpDelete("forms/{id:guid}")]
//        public async Task<ActionResult> DeleteForm(Guid id)
//        {
//            var form = await _db.Forms.FindAsync(id);
//            if (form == null) return NotFound();
//            _db.Forms.Remove(form);
//            await _db.SaveChangesAsync();
//            return NoContent();
//        }

//        // ------------------------
//        // Submissions
//        // ------------------------
//        [Authorize]
//        [HttpPost("forms/{formId:guid}/submit")]
//        public async Task<ActionResult<Submission>> SubmitForm(Guid formId, [FromBody] SubmitFormRequest req)
//        {
//            var form = await _db.Forms.FindAsync(formId);
//            if (form == null || !form.IsActive) return BadRequest("Form not available.");

//            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
//            var submission = new Submission
//            {
//                FormId = formId,
//                UserId = uid,
//                DataJson = string.IsNullOrWhiteSpace(req.DataJson) ? "{}" : req.DataJson
//            };

//            _db.Submissions.Add(submission);
//            await _db.SaveChangesAsync();
//            return CreatedAtAction(nameof(GetMySubmissions), new { }, submission);
//        }

//        [Authorize]
//        [HttpGet("submissions/mine")]
//        public async Task<ActionResult<IEnumerable<Submission>>> GetMySubmissions()
//        {
//            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
//            var list = await _db.Submissions
//                .AsNoTracking()
//                .Where(s => s.UserId == uid)
//                .Include(s => s.Form)
//                .OrderByDescending(s => s.CreatedAtUtc)
//                .ToListAsync();
//            return Ok(list);
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpGet("submissions/all")]
//        public async Task<ActionResult<IEnumerable<object>>> GetAllSubmissions()
//        {
//            var list = await _db.Submissions
//                .AsNoTracking()
//                .Include(s => s.Form)
//                .OrderByDescending(s => s.CreatedAtUtc)
//                .Select(s => new
//                {
//                    s.Id,
//                    s.CreatedAtUtc,
//                    FormTitle = s.Form.Title,
//                    s.UserId
//                }).ToListAsync();

//            return Ok(list);
//        }

//        // ------------------------
//        // Payments
//        // Base path: api/proj/payments
//        // ------------------------
//        [Authorize]
//        [HttpPost("payments/intent")]
//        public async Task<ActionResult<CreatePaymentResponse>> CreateIntent([FromBody] CreatePaymentRequest req)
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
//                {
//                    { "PaymentId", payment.Id.ToString() },
//                    { "SubmissionId", submission.Id.ToString() }
//                },
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

//        [Authorize]
//        [HttpGet("payments/{paymentId:guid}/pdf")]
//        public async Task<IActionResult> DownloadReceipt(Guid paymentId)
//        {
//            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

//            var payment = await _db.Payments
//                .Include(p => p.Submission)
//                .ThenInclude(s => s.Form)
//                .AsNoTracking()
//                .FirstOrDefaultAsync(p => p.Id == paymentId);

//            if (payment == null) return NotFound();
//            if (payment.Status != "Succeeded") return BadRequest("Receipt available only for successful payments.");

//            if (payment.Submission.UserId != uid && !User.IsInRole("Admin"))
//                return Forbid();

//            var user = await _db.Users.FindAsync(payment.Submission.UserId);
//            var bytes = _pdf.GenerateReceipt(payment, payment.Submission, payment.Submission.Form, user?.Email);
//            return File(bytes, "application/pdf", $"receipt-{payment.Id}.pdf");
//        }

//        // ------------------------
//        // Admin dashboard
//        // ------------------------
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

//        [HttpGet("aqi/{lat}/{lon}")]
//        public async Task<IActionResult> GetAqi(double lat, double lon)
//        {
//            var data = await _aqi.GetAqiAsync(lat, lon);
//            return Ok(data);
//        }

//        [HttpGet("download-pdf")]
//        public IActionResult DownloadPdf()
//        {
//            var pdf = Document.Create(container =>
//            {
//                container.Page(page =>
//                {
//                    page.Margin(30);
//                    page.Content().Text("Helllo! pdf file");
//                });
//            }).GeneratePdf();
//            return File(pdf, "application/pdf", "report.pdf");

//        }
//    }
//}
