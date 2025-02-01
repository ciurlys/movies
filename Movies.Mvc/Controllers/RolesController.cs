using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Mvc.Controllers;

public class RolesController : Controller
{
    private string AdminRole = "Administrators";
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<RolesController> _logger;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ILogger<RolesController> logger)
    {
        _logger = logger;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    //GET: /roles/index
    public async Task<IActionResult> Index()
    {
        if (!(await _roleManager.RoleExistsAsync(AdminRole)))
        {
            await _roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        IdentityUser? user = await _userManager.FindByNameAsync(User.Identity!.Name!);

        if (!await _userManager.IsInRoleAsync(user!, AdminRole) && user is not null)
        {
            IdentityResult result = await _userManager
                .AddToRoleAsync(user, AdminRole);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} added to {AdminRole}.");
            }
        }
        return Redirect("/");
    }
}
