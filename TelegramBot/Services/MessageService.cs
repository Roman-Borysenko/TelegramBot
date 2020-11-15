using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
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

        public void SendStartMessage(TelegramBotClient client, Chat chat)
        {
            SendCategory(client, chat, answerService.GetMainMenu());
        }

        public void SendPreviousMenu(TelegramBotClient client, Chat chat)
        {
            var categories = new List<Category>();

            if (answerService.Current == null || answerService.Current.ParentCategory == null)
                categories = answerService.GetMainMenu();
            else
                categories = answerService.Current.ParentCategory.Categories;

            SendCategory(client, chat, categories);
        }

        public async void SendMessage(TelegramBotClient client, Chat chat, string messageText)
        {
            var category = answerService.GetCategoryByName(messageText);

            if (category == null)
                return;

            if (category?.Categories?.Count > 0)
            {
                SendCategory(client, chat, category.Categories);
            }
            else
            {
                var answ = answerService.GetAnswer(category.Id);
                if (!answ.IsFile)
                {
                    await client.SendTextMessageAsync(chat, answ.Text);
                }
                else
                {
                    var file = new FileStream($"{hostEnvironment.WebRootPath}/{answ.Text}", FileMode.Open);

                    await client.SendDocumentAsync(chat,
                        new InputOnlineFile(file, answ.Text));

                    file.Close();
                }
            }
        }
        private async void SendCategory(TelegramBotClient client, Chat chat, IEnumerable<Category> categories)
        {
            await client.SendTextMessageAsync(chat, "Виберіть пункт із списку нижче:",
                    replyMarkup: answerService.CreateMenu(categories));
        }
    }
}
