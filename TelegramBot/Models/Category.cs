using System.Collections.Generic;

namespace TelegramBot.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Depth { get; set; }
        public List<Category> Categories { get; set; }
        public Answer Answer { get; set; }
        public Category ParentCategory { get; set; }
    }
}
