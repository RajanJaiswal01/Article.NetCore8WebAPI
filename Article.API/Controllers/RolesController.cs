using Article.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // GET: api/roles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return Ok(roles);
    }

    // POST: api/roles
    [HttpPost]
    public async Task<ActionResult> CreateRole([FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return BadRequest("Role name cannot be empty");
        }

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists)
        {
            return BadRequest("Role already exists");
        }

        var role = new IdentityRole(roleName);
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            return Ok($"Role '{roleName}' created successfully");
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    // DELETE: api/roles/{roleId}
    [HttpDelete("{roleId}")]
    public async Task<ActionResult> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return NotFound($"Role with ID '{roleId}' not found");
        }

        var result = await _roleManager.DeleteAsync(role);

        if (result.Succeeded)
        {
            return Ok($"Role '{role.Name}' deleted successfully");
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    // POST: api/roles/assign/{userId}/{roleName}
    [HttpPost("assign/{userId}/{roleName}")]
    public async Task<ActionResult> AssignRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"User with ID '{userId}' not found");
        }

        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return NotFound($"Role '{roleName}' not found");
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (result.Succeeded)
        {
            return Ok($"User '{user.UserName}' assigned to role '{roleName}' successfully");
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    // GET: api/roles/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"User with ID '{userId}' not found");
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(roles);
    }
}
