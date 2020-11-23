using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TextToUser { get; set; }
        public int Depth { get; set; }
        public List<Category> Categories { get; set; }
        public Answer Answer { get; set; }
        public int? ParentCategoryId { get; set; }
        [ForeignKey("ParentCategoryId")]
        public Category ParentCategory { get; set; }
        public bool IsAdmin { get; set; }
    }
}
