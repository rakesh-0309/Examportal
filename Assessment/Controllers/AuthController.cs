//using Assessment.Model;
//using Assessment.Services;
//using Assessment.DTO;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using static Assessment.DTO.AutoDtos;

//namespace Assessment.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class AuthController : ControllerBase
//{
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly SignInManager<ApplicationUser> _signInManager;
//    private readonly JwtTokenService _JwtTokenService;

//    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtTokenService jwt)
//    {
//        _userManager = userManager;
//        _signInManager = signInManager;
//        _JwtTokenService = jwt;
//    }

//    [HttpPost("register")]
//    public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
//    {
//        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName, EmailConfirmed = true };
//        var result = await _userManager.CreateAsync(user, request.Password);
//        if (!result.Succeeded) return BadRequest(result.Errors);

//        await _userManager.AddToRoleAsync(user, RoleSeeder.UserRole);
//        var token = _JwtTokenService.Create(user, RoleSeeder.UserRole);
//        return Ok(new LoginResponse(token, user.Id, user.Email!, RoleSeeder.UserRole));
//    }

//    //[HttpPost("login")]
//    //public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
//    //{
//    //    var user = await _userManager.FindByEmailAsync(request.Email);
//    //    if (user == null) return Unauthorized("Invalid credentials");
//    //    var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
//    //    if (!check.Succeeded) return Unauthorized("Invalid credentials");

//    //    var roles = await _userManager.GetRolesAsync(user);
//    //    var role = roles.FirstOrDefault() ?? RoleSeeder.UserRole;
//    //    var token = _jwt.Create(user, role);
//    //    return Ok(new LoginResponse(token, user.Id, user.Email!, role));
//    //}
//    [Produces("application/json")]
//    [HttpPost("login")]
//    public async Task<ActionResult<AutoDtos.LoginResponse>> Login(AutoDtos.LoginRequest request)
//    {
//        var user = await _userManager.FindByEmailAsync(request.Email);
//        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
//            return Unauthorized("Invalid email or password");

//        var roles = await _userManager.GetRolesAsync(user);
//        var role = roles.FirstOrDefault() ?? "User";

//        var token = _JwtTokenService.Create(user, role);

//        var response = new AutoDtos.LoginResponse(
//            Token: token,
//            UserId: user.Id,
//            Email: user.Email,
//            Role: role
//        );

//        return Ok(response);
//    }


//    [Authorize]
//    [HttpGet("me")]
//    public async Task<ActionResult<object>> Me()
//    {
//        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
//        var user = await _userManager.FindByIdAsync(id);
//        var roles = await _userManager.GetRolesAsync(user!);
//        return Ok(new { user!.Id, user.Email, Roles = roles });
//    }
//}

