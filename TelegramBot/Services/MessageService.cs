using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class MessageService
    {
        private IWebHostEnvironment hostEnvironment;
        private AnswerService answerService;
        public MessageService(IWebHostEnvironment environment,AnswerService service)
        {
            hostEnvironment = environment;
            answerService = service;
        }

        public async Task SendStartMessage(TelegramBotClient client, Chat chat)
        {
            SendCategory(client, chat, await answerService.GetMainMenu(chat.Username));
        }

        public async Task SendPreviousMenu(TelegramBotClient client, Chat chat)
        {
            var categories = new List<Category>();

            if (answerService.Current == null || answerService.Current.ParentCategory == null)
                categories = await answerService.GetMainMenu(chat.Username);
            else
                categories = answerService.Current.ParentCategory.Categories;

            SendCategory(client, chat, categories);
        }

        public async Task SendMessage(TelegramBotClient client, Chat chat, string messageText)
        {
            var category = await answerService.GetCategoryByName(messageText);

            if (category == null)
                return;

            if (category?.Categories?.Count > 0)
            {
                SendCategory(client, chat, category.Categories);
            }
            else
            {
                var answ = await answerService.GetAnswer(category.Id);

                answerService.IsQuestion = answ.IsQuestion;

                if (answ.IsFile)
                {
                    var file = new FileStream($"{hostEnvironment.WebRootPath}/{answ.Text}", FileMode.Open);

                    await client.SendDocumentAsync(chat,
                        new InputOnlineFile(file, answ.Text));

                    file.Close();
                } else if(answ.IsAllQuestions)
                {
                    await SendTextMessage(client, chat, answ.Text);

                    await SendAllQuestions(client, chat);
                }
                else
                {
                    await SendTextMessage(client, chat, answ.Text);
                }
            }
        }
        private async void SendCategory(TelegramBotClient client, Chat chat, IEnumerable<Category> categories)
        {
            await client.SendTextMessageAsync(chat, "Виберіть пункт із списку нижче:",
                    replyMarkup: answerService.CreateMenu(categories));
        }
        public async Task SendTextMessage(TelegramBotClient client, Chat chat, string text)
        {
            await client.SendTextMessageAsync(chat, text);
        }
        public async Task SendAllQuestions(TelegramBotClient client, Chat chat)
        {
            foreach (var question in answerService.GetQuestions().Result)
            {
                await client.SendTextMessageAsync(chat, 
                    $"Name: {question.Name}\nUsername: {question.UserName}\nText: {question.QuestionText}",
                    replyMarkup: answerService.CreateDeleteButton(question.Id));
            }
        }
        public async Task DeleteMessage(TelegramBotClient client, Chat chat, int messageId)
        {
            await client.DeleteMessageAsync(chat, messageId);
        }
    }
}
