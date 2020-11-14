using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class AnswerService
    {
        private Category currentRequest;
        private DataContext context;
        public AnswerService(DataContext dataContext)
        {
            context = dataContext;
        }
        public IEnumerable<Category> GetMainMenu()
        {
            return context.Categories.Where(c => c.Depth == 0).ToList();
        }
        public Category GetCategory(string text)
        {
            return context.Categories.Include(c => c.Categories).FirstOrDefault(c => c.Name.Equals(text));
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
