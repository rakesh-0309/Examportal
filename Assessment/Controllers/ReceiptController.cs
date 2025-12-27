//using Assessment.Data;
//using Assessment.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using Microsoft.EntityFrameworkCore;


//namespace Assessment.Controllers
//{
//    public class ReceiptController : Controller
//    {
//        private readonly AppDbContext _db;
//        private readonly PdfService _pdf;

//        public ReceiptController(AppDbContext db, PdfService pdf)
//        {
//            _db = db;
//            _pdf = pdf;
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
