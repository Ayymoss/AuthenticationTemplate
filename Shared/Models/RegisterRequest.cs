using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlazorAuthenticationLearn.Shared.Models;

public class RegisterRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    [Required(ErrorMessage = "{0} is required"), StringLength(16, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;
    [Required(ErrorMessage = "{0} is required"), DataType(DataType.EmailAddress), EmailAddress(ErrorMessage = "{0} is not valid")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "{0} is required"), DataType(DataType.Password), StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    [NotMapped, Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
