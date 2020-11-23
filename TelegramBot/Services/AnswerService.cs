using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class AnswerService
    {
        private DataContext context;
        private IConfiguration configuration;
        public bool IsQuestion { get; set; }
        public Category Current { get; set; }
        public string Beck => "Назад";
        public AnswerService(IConfiguration configuration, DataContext dataContext)
        {
            this.configuration = configuration;
            context = dataContext;
        }
        public async Task<List<Category>> GetMainMenu(string username)
        {
            return CheckOnAdmin(await context.Categories.Where(c => c.Depth == 0).ToListAsync(), username);
        }
        public async Task<Category> GetCategoryByName(string text)
        {
            var category = await context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Categories)
                .Include(c => c.Answer)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.Equals(text));

            if (category == null)
                return null;

            if(category.Answer == null)
            {
                Current = category;

                if(!category.Categories.Any(c => c.Id == -1))
                    category.Categories.Add(new Category() { Id = -1, Name = $"{Beck}" });
            }

            return category;
        }

        public async Task<Answer> GetAnswer(int categoryId)
        {
            var answer = await context.Answers.AsNoTracking().FirstOrDefaultAsync(a => a.CategoryId == categoryId);
            return answer;
        }

        public async Task<List<Question>> GetQuestions()
        {
            return await context.Questions.AsNoTracking().ToListAsync();
        }

        public async Task CreateQuestion(string name, string userName, string text)
        {
            var question = new Question() { Name = name, UserName = userName, QuestionText = text };

            await context.Questions.AddAsync(question);
            await context.SaveChangesAsync();

            IsQuestion = false;
        }

        public async Task<bool> DeleteQuestion(string questionId)
        {
            if(int.TryParse(questionId, out int id))
            {
                context.Questions.Remove(await context.Questions.SingleOrDefaultAsync(q => q.Id == id));
                await context.SaveChangesAsync();

                return true;
            } else
            {
                return false;
            }    
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
        public InlineKeyboardMarkup CreateDeleteButton(int questionId)
        {
            var inlineKeyboard = new List<List<InlineKeyboardButton>>() 
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Видалити", questionId.ToString())
                }
            };

            return new InlineKeyboardMarkup(inlineKeyboard);
        }
        public List<Category> CheckOnAdmin(List<Category> categories, string userName)
        {
            var admin = configuration.GetSection("Bot")["Admin"];

            if(!string.IsNullOrEmpty(admin))
            {
                if(admin.Split('|').Contains(userName))
                {
                    return categories;
                }
            }

            return categories.Where(c => c.IsAdmin == false).ToList(); ;
        }
    }
}
