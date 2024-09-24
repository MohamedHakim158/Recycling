namespace ProjectGraduation.Helping_Models
{
    public class PostModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
        public IFormFile Image { get; set; }
        public string CategoryName { get; set; }
        public string ProfileId { get; set; }
    }
}
