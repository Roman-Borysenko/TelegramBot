using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public string Text { get; set; }
        public bool IsFile { get; set; }
    }
}
