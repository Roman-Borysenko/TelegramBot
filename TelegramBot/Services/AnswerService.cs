using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class AnswerService
    {
        private DataContext context;
        public Category Current { get; set; }
        public string Beck => "Повернутися";
        public AnswerService(DataContext dataContext)
        {
            context = dataContext;
        }
        public List<Category> GetMainMenu()
        {
            var category = context.Categories.Where(c => c.Depth == 0).ToList();

            return category;
        }
        public Category GetCategoryByName(string text)
        {
            var category = context.Categories
                .Include(c => c.ParentCategory)
                .ThenInclude(c => c.Categories)
                .Include(c => c.Categories)
                .Include(c => c.Answer)
                .FirstOrDefault(c => c.Name.Equals(text));

            if(category.Answer == null && !category.Categories.Any(c => c.Id == -1))
            {
                category.Categories.Add(new Category() { Id = -1, Name = $"{Beck}" });
            }

            Current = category;

            return category;
        }

        public Answer GetAnswer(int categoryId)
        {
            var answer = context.Answers.FirstOrDefault(a => a.CategoryId == categoryId);
            return answer;
        }

        public ReplyKeyboardMarkup CreateMenu(IEnumerable<Category> categories)
        {
            var inlineKeyboard = new List<List<KeyboardButton>>();

            foreach(var category in categories)
            {
                inlineKeyboard.Add(new List<KeyboardButton>() { new KeyboardButton(category.Name) });
            }

            return new ReplyKeyboardMarkup(inlineKeyboard);
        }
    }
}
