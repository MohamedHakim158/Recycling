namespace ProjectGraduation.Models
{
    public class Contact
    {
        public string Id = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? LinkedInAccount { get; set; } 
        public string? FacebookAccount { get; set; }
        public string Specialty { get; set; }
        public byte [] Image { get; set; }
        public bool? IsTechnicalSupport { get; set; }
    }
}
