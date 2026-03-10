using Episodes.Data;
using Episodes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Episodes.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public sealed class MeController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public MeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(new MeResponse(
            user.Id,
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            user.CreatedAt));
    }
}