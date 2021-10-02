namespace Models.Models
{
    public class Blog
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Text { get; set; }
        
        public User Owner { get; set; }
    }
}