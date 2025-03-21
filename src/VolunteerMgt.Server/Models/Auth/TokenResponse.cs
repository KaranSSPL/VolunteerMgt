﻿namespace VolunteerMgt.Server.Models.Auth;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public string Email { get; set; } = string.Empty;
}
