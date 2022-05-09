﻿namespace BlazorAuthenticationLearn.Shared.Models;

public class Developer : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public decimal Experience { get; set; }
}
