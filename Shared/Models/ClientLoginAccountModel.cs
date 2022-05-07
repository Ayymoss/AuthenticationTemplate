﻿using System.ComponentModel.DataAnnotations;

namespace BlazorAuthenticationLearn.Shared.Models;

public class ClientLoginAccountModel
{
    [Required(ErrorMessage = "{0} is required"), StringLength(16, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
    public string Username { get; set; }
    [Required(ErrorMessage = "{0} is required"), DataType(DataType.Password), StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    public string Password { get; set; }
}
