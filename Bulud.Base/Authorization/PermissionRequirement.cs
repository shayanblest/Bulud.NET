using Microsoft.AspNetCore.Authorization;

namespace Bulud.Base.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    private string Permission { get; } = permission;
    public string Resource => Permission.Split(':')[0];
    public char Action => Permission.Split(':')[1].ToUpper()[0];
}

// Members:Update