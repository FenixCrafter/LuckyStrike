using Microsoft.AspNetCore.Identity;
using WebApi.Statics;

namespace WebApi.Handlers;

public class RoleHandler
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleHandler(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    public async Task ValidateRoles()
    {
        Type type = typeof(Roles);
        foreach (var role in type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
        {
            var roleName = role.GetValue(null).ToString();

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
