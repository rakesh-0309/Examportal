//using Assessment.Data;
//using Assessment.DTO;
//using Assessment.Model;
//using Assessment.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Stripe;
//using System.Security.Claims;
//using static Assessment.DTO.AutoDtos;
//using static Assessment.DTO.PaymentDtos;
//using static Assessment.DTO.SubmissionDtos;

//namespace Assessment.Controllers
//{
//    public class ProjController : ControllerBase
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly JwtTokenService _JwtTokenService;
//        private readonly AppDbContext _db;
//        //private readonly AppDbContext _context;
//        private readonly IConfiguration _config;
//        private readonly PdfService _pdf;
//        //public ProjController(AppDbContext db) => _db = db;

//        public ProjController(
//    AppDbContext db,
//    IConfiguration config,
//    PdfService pdf,
//    UserManager<ApplicationUser> userManager,
//    SignInManager<ApplicationUser> signInManager,
//    JwtTokenService jwt,
//    AppDbContext context)
//        {
//            _db = db;
//            _config = config;
//            _pdf = pdf;
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _JwtTokenService = jwt;
//            //_context = context;
//        }
        

//        //private readonly PdfService _pdf;

//        //public ProjController(AppDbContext db, PdfService pdf)
//        //{
//        //    _db = db;
//        //    _pdf = pdf;
//        //}

//        //public ProjController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtTokenService jwt)
//        //{
//        //    _userManager = userManager;
//        //    _signInManager = signInManager;
//        //    _JwtTokenService = jwt;
//        //}

//        [HttpPost("register")]
//        public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
//        {
//            var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName, EmailConfirmed = true };
//            var result = await _userManager.CreateAsync(user, request.Password);
//            if (!result.Succeeded) return BadRequest(result.Errors);

//            await _userManager.AddToRoleAsync(user, RoleSeeder.UserRole);
//            var token = _JwtTokenService.Create(user, RoleSeeder.UserRole);
//            return Ok(new LoginResponse(token, user.Id, user.Email!, RoleSeeder.UserRole));
//        }

//        //[HttpPost("login")]
//        //public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
//        //{
//        //    var user = await _userManager.FindByEmailAsync(request.Email);
//        //    if (user == null) return Unauthorized("Invalid credentials");
//        //    var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
//        //    if (!check.Succeeded) return Unauthorized("Invalid credentials");

//        //    var roles = await _userManager.GetRolesAsync(user);
//        //    var role = roles.FirstOrDefault() ?? RoleSeeder.UserRole;
//        //    var token = _jwt.Create(user, role);
//        //    return Ok(new LoginResponse(token, user.Id, user.Email!, role));
//        //}

//        //working
//        //[Produces("application/json")]
//        //[HttpPost("login")]
//        //public async Task<ActionResult<AutoDtos.LoginResponse>> Login(AutoDtos.LoginRequest request)
//        //{
//        //    var user = await _userManager.FindByEmailAsync(request.Email);
//        //    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
//        //        return Unauthorized("Invalid email or password");

//        //    var roles = await _userManager.GetRolesAsync(user);
//        //    var role = roles.FirstOrDefault() ?? "User";

//        //    var token = _JwtTokenService.Create(user, role);

//        //    var response = new AutoDtos.LoginResponse(
//        //        Token: token,
//        //        UserId: user.Id,
//        //        Email: user.Email,
//        //        Role: role
//        //    );

//        //    return Ok(response);
//        //}

//        //[HttpPost("login")]
//        //public async Task<IActionResult> Login([FromBody] LoginRequest req)
//        //{
//        //    // ✅ Static Admin Check
//        //    if (req.Email == "admin@email.com" && req.Password == "Admin@12345")
//        //    {
//        //        var token = _JwtTokenService.Create(req.Email, "Admin");
//        //        return Ok(new
//        //        {
//        //            token,
//        //            email = req.Email,
//        //            role = "Admin"
//        //        });
//        //    }

//        //    // ✅ Dynamic User Check from Database
//        //    var user = await _userManager.FindByEmailAsync(req.Email);
//        //    if (user != null)
//        //    {
//        //        var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
//        //        if (result.Succeeded)
//        //        {
//        //            var token = _JwtTokenService.Create(req.Email, "User");
//        //            return Ok(new
//        //            {
//        //                token,
//        //                email = user.Email,
//        //                role = "User"

//        //            });
//        //        }
//        //    }

//        //    return Unauthorized(new { message = "Invalid email or password" });
//        //}

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginRequest req)
//        {
//            var user = await _userManager.FindByEmailAsync(req.Email);
//            if (user == null) return Unauthorized("Invalid credentials");

//            var validPassword = await _userManager.CheckPasswordAsync(user, req.Password);
//            if (!validPassword) return Unauthorized("Invalid credentials");

//            var roles = await _userManager.GetRolesAsync(user);
//            var role = roles.FirstOrDefault() ?? "User";

//            var token = _JwtTokenService.Create(user.Email, role);

//            return Ok(new { token, role });
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpPost]
//        public IActionResult CreateForm([FromBody] FormDtos form)
//        {
//            // Only Admin can create
//            return Ok("Form created successfully");
//        }

//        [Authorize(Roles = "User,Admin")]
//        [HttpGet]
//        public IActionResult GetForms()
//        {
//            // Both can view
//            return Ok(_db.Forms.ToList());
//        }

//        [Authorize(Roles = "User")]
//        [HttpPost("{formId}")]
//        public IActionResult SubmitForm(int formId, [FromBody] SubmissionDtos data)
//        {
//            return Ok("Form submitted");
//        }

//        [Authorize(Roles = "User,Admin")]
//        [HttpPost("intent")]
//        public IActionResult CreatePayment()
//        {
//            return Ok("Payment Intent Created");
//        }

//        //[Authorize(Roles = "Admin")]
//        //[HttpGet("dashboard")]
//        //public IActionResult Dashboard()
//        //{
//        //    return Ok("Admin Stats here");
//        //}

//        [Authorize(Roles = "User,Admin")]
//        [HttpGet("{paymentId}/pdf")]
//        public IActionResult DownloadReceipt(string paymentId)
//        {
//            return Ok("PDF Downloaded");
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

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Form>>> GetAll([FromQuery] bool onlyActive = true)
//        {
//            var q = _db.Forms.AsNoTracking();
//            if (onlyActive) q = q.Where(f => f.IsActive);
//            return Ok(await q.OrderBy(f => f.Title).ToListAsync());
//        }

//        [HttpGet("{id:guid}")]
//        public async Task<ActionResult<Form>> GetById(Guid id)
//        {
//            var form = await _db.Forms.FindAsync(id);
//            if (form == null) return NotFound();
//            return Ok(form);
//        }

//        // Admin CRUD
//        [Authorize(Roles = "Admin")]
//        [HttpPost]
//        public async Task<ActionResult<Form>> Create(CreateFormRequest req)
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
//            return CreatedAtAction(nameof(GetById), new { id = form.Id }, form);
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpPut("{id:guid}")]
//        public async Task<ActionResult> Update(Guid id, UpdateFormRequest req)
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
//        [HttpDelete("{id:guid}")]
//        public async Task<ActionResult> Delete(Guid id)
//        {
//            var form = await _db.Forms.FindAsync(id);
//            if (form == null) return NotFound();
//            _db.Forms.Remove(form);
//            await _db.SaveChangesAsync();
//            return NoContent();
//        }

//        [Authorize]
//        [HttpPost("{formId:guid}")]
//        public async Task<ActionResult<Submission>> Submit(Guid formId, [FromBody] SubmitFormRequest req)
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
//            return CreatedAtAction(nameof(GetMy), new { }, submission);
//        }

//        [Authorize]
//        [HttpGet("mine")]
//        public async Task<ActionResult<IEnumerable<Submission>>> GetMy()
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
//        [HttpGet("all")]
//        public async Task<ActionResult<IEnumerable<object>>> GetAll()
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

//        [Authorize]
//        [HttpGet("{paymentId:guid}/pdf")]
//        public async Task<IActionResult> Download(Guid paymentId)
//        {
//            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

//            var payment = await _db.Payments
//                .Include(p => p.Submission)
//                .ThenInclude(s => s.Form)
//                .AsNoTracking()
//                .FirstOrDefaultAsync(p => p.Id == paymentId);

//            if (payment == null) return NotFound();
//            if (payment.Status != "Succeeded") return BadRequest("Receipt available only for successful payments.");

//            // Only owner or admin can download
//            if (payment.Submission.UserId != uid && !User.IsInRole("Admin"))
//                return Forbid();

//            var user = await _db.Users.FindAsync(payment.Submission.UserId);
//            var bytes = _pdf.GenerateReceipt(payment, payment.Submission, payment.Submission.Form, user?.Email);
//            return File(bytes, "application/pdf", $"receipt-{payment.Id}.pdf");
//        }
//    }
//}

using Assessment.Data;
using Assessment.DTO;
using Assessment.Model;
using Assessment.Services;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Stripe;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Assessment.DTO.AutoDtos;
using static Assessment.DTO.PaymentDtos;
using static Assessment.DTO.SubmissionDtos;

namespace Assessment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly Services.JwtTokenService _jwtTokenService;
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public ProjController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        Services.JwtTokenService jwt,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwt;
        _config = config;
        _connectionString = config.GetConnectionString("Default")!;
    }

    // ------------------------
    // Auth
    // ------------------------
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, RoleSeeder.UserRole);
        var token = _jwtTokenService.Create(user, RoleSeeder.UserRole);
        return Ok(new LoginResponse(token, user.Id, user.Email!, RoleSeeder.UserRole));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null) return Unauthorized("Invalid credentials");

        var validPassword = await _userManager.CheckPasswordAsync(user, req.Password);
        if (!validPassword) return Unauthorized("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        var token = _jwtTokenService.Create(user, role);
        return Ok(new { token, role });
    }

    //[Authorize]
    //[HttpGet("me")]
    //public async Task<ActionResult<object>> Me()
    //{
    //    var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    //    var user = await _userManager.FindByIdAsync(id);
    //    var roles = await _userManager.GetRolesAsync(user!);
    //    return Ok(new { user!.Id, user.Email, Roles = roles });
    //}

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<object>> Me()
    {
        // Debug info
        Console.WriteLine("=== /me endpoint called ===");
        Console.WriteLine($"IsAuthenticated: {User.Identity?.IsAuthenticated}");
        Console.WriteLine($"Claims count: {User.Claims.Count()}");

        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"  {claim.Type} = {claim.Value}");
        }

        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine($"NameIdentifier: {id ?? "NULL"}");

        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new
            {
                message = "User ID not found in token",
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            user.Id,
            user.Email,
            user.UserName,
            Roles = roles
        });
    }


    // ------------------------
    // Forms - Dapper with Stored Procedures
    // ------------------------
    [HttpGet("formsforall")]
    public async Task<ActionResult<IEnumerable<Form>>> GetForms([FromQuery] bool onlyActive = true)
    {
        using var connection = new SqlConnection(_connectionString);
        var forms = await connection.QueryAsync<Form>(
            "sp_GetAllForms",
            new { OnlyActive = onlyActive },
            commandType: System.Data.CommandType.StoredProcedure);

        return Ok(forms);
    }

    [HttpGet("forms/{id:guid}")]
    public async Task<ActionResult<Form>> GetFormById(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);

        var form = await connection.QuerySingleOrDefaultAsync<Form>(
            "sp_GetFormById",
            new { FormId = id },
            commandType: System.Data.CommandType.StoredProcedure);

        if (form == null) return NotFound();
        return Ok(form);
    }

    //[Authorize(Roles = "Admin")]
    [HttpPost("formsforadmin")]
    public async Task<ActionResult<Form>> CreateForm(CreateFormRequest req)
    {

        using var connection = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@Title", req.Title);
        parameters.Add("@Description", req.Description);
        parameters.Add("@FieldsJson", req.FieldsJson);
        parameters.Add("@Price", req.Price);
        parameters.Add("@Currency", req.Currency);
        parameters.Add("@NewFormId", dbType: System.Data.DbType.Guid, direction: System.Data.ParameterDirection.Output);

        var form = await connection.QuerySingleOrDefaultAsync<Form>(
            "sp_CreateForm",
            parameters,
            commandType: System.Data.CommandType.StoredProcedure);

        if (form == null) return BadRequest("Failed to create form");
        //return (nameof(GetFormById), new { id = form.Id }, form);
        return Ok(form);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("forms/{id:guid}")]
    public async Task<ActionResult> UpdateForm(Guid id, UpdateFormRequest req)
    {
        using var connection = new SqlConnection(_connectionString);

        var rowsAffected = await connection.ExecuteAsync(
            "sp_UpdateForm",
            new
            {
                FormId = id,
                Title = req.Title,
                Description = req.Description,
                FieldsJson = req.FieldsJson,
                IsActive = req.IsActive,
                Price = req.Price,
                Currency = req.Currency
            },
            commandType: System.Data.CommandType.StoredProcedure);

        if (rowsAffected == 0) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("forms/{id:guid}")]
    public async Task<ActionResult> DeleteForm(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);

        var rowsAffected = await connection.ExecuteAsync(
            "sp_DeleteForm",
            new { FormId = id },
            commandType: System.Data.CommandType.StoredProcedure);

        if (rowsAffected == 0) return NotFound();
        return NoContent();
    }

    // ------------------------
    // Submissions - Dapper with Stored Procedures
    // ------------------------
    [Authorize]
    [HttpPost("forms/{formId:guid}/submit")]
    public async Task<ActionResult<SubmissionWithForm>> SubmitForm(Guid formId, [FromBody] SubmitFormRequest req)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        using var connection = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@FormId", formId);
        parameters.Add("@UserId", uid);
        parameters.Add("@DataJson", string.IsNullOrWhiteSpace(req.DataJson) ? "{}" : req.DataJson);
        parameters.Add("@NewSubmissionId", dbType: System.Data.DbType.Guid, direction: System.Data.ParameterDirection.Output);

        try
        {
            var submission = await connection.QuerySingleOrDefaultAsync<SubmissionWithForm>(
                "sp_SubmitForm",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            if (submission == null) return BadRequest("Failed to submit form");
            return CreatedAtAction(nameof(GetMySubmissions), new { }, submission);
        }
        catch (SqlException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("submissions/mine")]
    public async Task<ActionResult<IEnumerable<SubmissionWithForm>>> GetMySubmissions()
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        using var connection = new SqlConnection(_connectionString);

        var submissions = await connection.QueryAsync<SubmissionWithForm>(
            "sp_GetUserSubmissions",
            new { UserId = uid },
            commandType: System.Data.CommandType.StoredProcedure);

        return Ok(submissions);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("submissions/all")]
    public async Task<ActionResult<IEnumerable<SubmissionSummary>>> GetAllSubmissions()
    {
        using var connection = new SqlConnection(_connectionString);

        var submissions = await connection.QueryAsync<SubmissionSummary>(
            "sp_GetAllSubmissions",
            commandType: System.Data.CommandType.StoredProcedure);

        return Ok(submissions);
    }

    // ------------------------
    // Payments - Dapper with Stored Procedures + Stripe
    // ------------------------
    [Authorize]
    [HttpPost("payments/intent")]
    public async Task<ActionResult<CreatePaymentResponse>> CreateIntent([FromBody] CreatePaymentRequest req)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        using var connection = new SqlConnection(_connectionString);


        var parameters = new DynamicParameters();
        parameters.Add("@SubmissionId", req.SubmissionId);
        parameters.Add("@UserId", uid);
        parameters.Add("@NewPaymentId", dbType: System.Data.DbType.Guid, direction: System.Data.ParameterDirection.Output);
        parameters.Add("@AmountMinor", dbType: System.Data.DbType.Int64, direction: System.Data.ParameterDirection.Output);
        parameters.Add("@Currency", dbType: System.Data.DbType.String, size: 10, direction: System.Data.ParameterDirection.Output);


        try
        {
            await connection.ExecuteAsync(
                "sp_CreatePayment",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);
            var paymentId = parameters.Get<Guid>("@NewPaymentId");
            var amountMinor = parameters.Get<long>("@AmountMinor");
            var currency = parameters.Get<string>("@Currency");
            // Create Stripe PaymentIntent
            var options = new PaymentIntentCreateOptions
            {
                Amount = amountMinor,
                Currency = currency,
                Metadata = new Dictionary<string, string>
                {
{ "PaymentId", paymentId.ToString() },
{ "SubmissionId", req.SubmissionId.ToString() }
                },
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            // Update payment with Stripe ID
            await connection.ExecuteAsync(
                "sp_UpdatePaymentProvider",
                new
                {
                    PaymentId = paymentId,
                    ProviderPaymentId = intent.Id,
                    Status = intent.Status ?? "requires_payment_method"
                },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(new CreatePaymentResponse(paymentId, intent.ClientSecret!));
        }
        catch (SqlException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("stripe/webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var sigHeader = Request.Headers["Stripe-Signature"];
        var endpointSecret = _config["Stripe:WebhookSecret"];

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, sigHeader, endpointSecret);
        }
        catch
        {
            return BadRequest();
        }

        string? providerPaymentId = null;
        string? status = null;

        if (stripeEvent.Type == Events.PaymentIntentSucceeded)
        {
            var intent = stripeEvent.Data.Object as PaymentIntent;
            providerPaymentId = intent!.Id;
            status = "Succeeded";
        }
        else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
        {
            var intent = stripeEvent.Data.Object as PaymentIntent;
            providerPaymentId = intent!.Id;
            status = "Failed";
        }

        if (providerPaymentId != null && status != null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
        "sp_UpdatePaymentStatusByProvider",
        new
        {
            ProviderPaymentId = providerPaymentId,
            Status = status
        },
        commandType: System.Data.CommandType.StoredProcedure);
        }

        return Ok();
    }

    //[Authorize]
    //[HttpGet("payments/{paymentId:guid}/pdf")]
    //public async Task<IActionResult> DownloadReceipt(Guid paymentId)
    //{
    //    var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    //    var isAdmin = User.IsInRole("Admin");


    //    using var connection = new SqlConnection(_connectionString);
    //    var payment = await connection.QuerySingleOrDefaultAsync<PaymentReceiptData>(
    //        "sp_GetPaymentForReceipt",
    //        new
    //        {
    //            PaymentId = paymentId,
    //            UserId = isAdmin ? "ADMIN" : uid
    //        },
    //        commandType: System.Data.CommandType.StoredProcedure);

    //    if (payment == null) return NotFound();
    //    if (payment.Status != "Succeeded")
    //        return BadRequest("Receipt available only for successful payments");

    //    var pdfService = new Services.PdfService();
    //    var bytes = pdfService.GenerateReceipt(payment);
    //    return File(bytes, "application/pdf", $"receipt-{payment.PaymentId}.pdf");
    //}

    // ------------------------
    // Admin Dashboard - Dapper with Stored Procedures
    // ------------------------
    [Authorize(Roles = "Admin")]
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStats>> Dashboard()
    {
        using var connection = new SqlConnection(_connectionString);

        var stats = await connection.QuerySingleOrDefaultAsync<DashboardStats>(
            "sp_GetDashboardStats",
            commandType: System.Data.CommandType.StoredProcedure);

        if (stats == null) return BadRequest("Failed to get dashboard stats");
        return Ok(stats);
    }

    [HttpGet("test-token")]
    public IActionResult TestToken()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
            return BadRequest("No token provided");

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            // ✅ Debug:  Check if config values are loaded
            Console.WriteLine($"Config Key Length: {key?.Length ?? 0}");
            Console.WriteLine($"Config Issuer: {issuer ?? "NULL"}");
            Console.WriteLine($"Config Audience: {audience ?? "NULL"}");

            // ✅ Check if values are null
            if (string.IsNullOrEmpty(key))
                return BadRequest("Jwt:Key is not configured");
            if (string.IsNullOrEmpty(issuer))
                return BadRequest("Jwt:Issuer is not configured");
            if (string.IsNullOrEmpty(audience))
                return BadRequest("Jwt: Audience is not configured");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,  // This was null! 
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew = TimeSpan.Zero
            };

            var principal = handler.ValidateToken(token, validationParameters, out _);

            return Ok(new
            {
                status = "valid",
                tokenIssuer = jwtToken.Issuer,
                tokenAudience = jwtToken.Audiences.FirstOrDefault(),
                configIssuer = issuer,
                configAudience = audience,
                keyLength = key.Length,
                claims = principal.Claims.Select(c => new { c.Type, c.Value })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                status = "invalid",
                error = ex.Message,
                type = ex.GetType().Name,
                innerException = ex.InnerException?.Message
            });
        }
    }

    // ------------------------
    // Additional Features
    // ------------------------
    //[HttpGet("aqi/{lat}/{lon}")]
    //public async Task<IActionResult> GetAqi(double lat, double lon)
    //{
    //    var aqiService = new Services.AQIService();
    //    var data = await aqiService.GetAqiAsync(lat, lon);
    //    return Ok(data);
    //}

    [HttpGet("download-pdf")]
    public IActionResult DownloadPdf()
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content().Text("Hello! pdf file");
            });
        }).GeneratePdf();
        return File(pdf, "application/pdf", "report.pdf");
    }
}

// Supporting classes for Dapper mapping
public class SubmissionWithForm
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string UserId { get; set; } = default!;
    public string DataJson { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; }
    public string FormTitle { get; set; } = default!;
    public decimal FormPrice { get; set; }
    public string FormCurrency { get; set; } = default!;
    public string? FormDescription { get; set; }
}

public class SubmissionSummary
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string UserId { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; }
    public string FormTitle { get; set; } = default!;
}

public class DashboardStats
{
    public int TotalForms { get; set; }
    public int TotalSubmissions { get; set; }
    public int TotalPayments { get; set; }
    public int TotalSucceeded { get; set; }
    public long TotalAmountMinor { get; set; }
    public decimal TotalAmount { get; set; }
}

public class PaymentReceiptData
{
    public Guid PaymentId { get; set; }
    public long AmountMinor { get; set; }
    public string Currency { get; set; } = default!;
    public string Provider { get; set; } = default!;
    public string? ProviderPaymentId { get; set; }
    public string Status { get; set; } = default!;
    public DateTime PaymentCreatedAtUtc { get; set; }
    public Guid SubmissionId { get; set; }
    public string UserId { get; set; } = default!;
    public string DataJson { get; set; } = default!;
    public DateTime SubmissionCreatedAtUtc { get; set; }
    public Guid FormId { get; set; }
    public string FormTitle { get; set; } = default!;
    public string? FormDescription { get; set; }
    public string? UserEmail { get; set; }
}



