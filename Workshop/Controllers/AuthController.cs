using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Workshop.Models;
using Workshop.Services;
using Workshop.Data;

namespace Workshop.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            EmailService emailService,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _context = context;
        }

        // Task 3 + Task 5: Register with transaction + email confirmation
        [HttpPost("register")]
        public async Task<IActionResult> Register(string fullName, string email, string password)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    FullName = fullName,
                    Email = email,
                    UserName = email
                };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                // Task 3: Role assignment - rollback if it fails
                var role = await _roleManager.FindByNameAsync("User");
                if (role == null) throw new Exception("Role not found");
                await _userManager.AddToRoleAsync(user, "User");

                await transaction.CommitAsync();

                // Task 5: Send confirmation email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmUrl = Url.Action("ConfirmEmail", "Auth",
                    new { userId = user.Id, token }, Request.Scheme);

                await _emailService.SendAsync(user.Email, "Confirm your email",
                    $"Click here to confirm your email: {confirmUrl}");

                return Ok("Registered successfully. Please check your email to confirm.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Task 5: Confirm email endpoint
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return BadRequest("Email confirmation failed");

            return Ok("Email confirmed! You can now log in.");
        }

        // Task 5: Login - blocked if email not confirmed
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized("Please confirm your email before logging in.");

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (!result.Succeeded) return Unauthorized("Invalid credentials");

            return Ok("Login successful");
        }
    }
}