namespace VolunteerMgt.Server.Common.Settings;

public class JwtConfiguration
{
    public string? ValidIssuer { get; set; }
    public string? ValidAudience { get; set; }
    public string? Secret { get; set; }
    /// <summary>
    /// Token expiry time (In seconds).
    /// </summary>
    public int TokenExpiry { get; set; } = 7200;
}
