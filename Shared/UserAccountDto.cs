using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAuthenticationLearn.Shared;

public class UserAccountDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Username is required"), StringLength(16, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required"), StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    [NotMapped, Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
    
}
