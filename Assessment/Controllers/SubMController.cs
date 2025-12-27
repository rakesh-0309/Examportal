//using Assessment.Data;
//using Assessment.DTO;
//using Assessment.Model;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;
//using static Assessment.DTO.SubmissionDtos;

//namespace Assessment.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class SubM : ControllerBase
//    {
//        private readonly AppDbContext _db;

//        public SubM(AppDbContext db) => _db = db;

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
//    }

//}
