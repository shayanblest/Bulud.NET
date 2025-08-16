using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulud.Base.Infrastructure;

public class AppDbContextBase<TUser,TRole, TUserRole>(DbContextOptions options)
    : IdentityDbContext<TUser, TRole, string, IdentityUserClaim<string>, TUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>(options) where  TUser : IdentityUser where TRole : IdentityRole where TUserRole : IdentityUserRole<string>
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && !a.FullName.StartsWith("Microsoft.") && !a.FullName.StartsWith("System.")).ToArray();
        foreach (var assembly in assemblies)
        {
            builder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}