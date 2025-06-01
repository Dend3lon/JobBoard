namespace JobBoard.Options
{
    public class JwtSettings
    {
        public string? SingingKey { get; set; }
        public string? Issuer { get; set; }
        public string[]? Audience { get; set; }
    }
}
