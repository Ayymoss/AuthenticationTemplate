namespace BlazorAuthenticationLearn.Shared;

public static class Roles
{
    public static readonly List<string> Names = new()
    {
        "SuperAdmin", 
        "User"
    };
}

public enum GlobalRoles
{
    SuperAdmin,
    User
}
