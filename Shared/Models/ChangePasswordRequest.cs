using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAuthenticationLearn.Shared.Models;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "{0} is required"), DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "{0} is required"), DataType(DataType.Password), StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    
    [NotMapped, Compare("Password", ErrorMessage = "Passwords do not match"), DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
