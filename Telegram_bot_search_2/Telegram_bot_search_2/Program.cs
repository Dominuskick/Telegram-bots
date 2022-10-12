using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Linq;

namespace Cs_my_doc_bot3
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = "5747434056:AAG20Tfi8YTW8p2AMP0XEJEutaXVRMXlqqw";
            //var token = "5459996216:AAHlWw8Gvj2k1zQk9PiOPfJKX9two3n2I5U";
            var client = new Doc_Bot(token);

            client.Start();
            Console.ReadLine();
        }
    }

    public class Doc_Bot
    {
        private readonly TelegramBotClient bot;

        static string[] tempNamefile;
        static string[] tempNameDirectory;
        static string WithCallbackData;
        static long chatId = 0;
        static int num = 0;

        private static string TempPath = @"C:\";
        private static string filters = "*.txt|*.jpg|*.jpeg|*.gif|*.doc|*.docx|*.xsl|*xlsx|*.mp4|*.mp3|*.zip";

        public Doc_Bot(string token)
        {
            bot = new TelegramBotClient(token);
        }

        public void Start()
        {
            bot.StartReceiving(Update, Error);

        }

        private static string GetNameFile(string Path)
        {
            var NewString = "";
            for (int i = 0; i < Path.Length; i++)
            {
                if (Convert.ToString(Path[i]) == @"\")
                {
                    NewString = Path[(i + 1)..];
                }
            }

            return NewString;
        }

        private static string GetName(string Path)
        {
            var NewString = "";
            for (int i = 0; i < Path.Length; i++)
            {
                if (Convert.ToString(Path[i]) == @"\")
                {
                    NewString = Path[(i + 1)..];
                    WithCallbackData = Path[..(i + 1)];
                }
            }

            return NewString;
        }

        private static string[] GetFiles(string sourceFolder, string filters)
        {
            return filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter)).ToArray();
        }


        private static InlineKeyboardMarkup GetDirectorie(string Path)
        {
            tempNamefile = GetFiles(Path, filters);
            tempNameDirectory = System.IO.Directory.GetDirectories(Path);

            var tempButton = new InlineKeyboardButton[tempNameDirectory.Length + tempNamefile.Length + 1][];

            int i = 0;
            foreach (var item in tempNameDirectory)
            {
                Console.WriteLine(item);
                tempButton[i] = new[] { InlineKeyboardButton.WithCallbackData(item, GetName(item)) };
                i++;
            }

            foreach (var item in tempNamefile)
            {
                Console.WriteLine(item);
                tempButton[i] = new[] { InlineKeyboardButton.WithCallbackData(item, GetName(item)) };
                i++;
            }
            tempButton[^1] = new[] { InlineKeyboardButton.WithCallbackData("Back", "Back") };
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(tempButton);
            return inlineKeyboard;
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            //return;
            Message? message = update.Message;

            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null && update.CallbackQuery.Message != null)
            {
                //if (update.CallbackQuery.Data.StartsWith(@"C:\"))
                {
                    var temp = WithCallbackData + update.CallbackQuery.Data;
                    FileAttributes attr = System.IO.File.GetAttributes($@"{temp}");
                    if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        await using Stream stream = System.IO.File.Open(temp, FileMode.Open);
                        await botClient.SendDocumentAsync(update.CallbackQuery.Message.Chat.Id, new InputOnlineFile(stream, GetNameFile(temp)));
                    }
                    else
                    {
                        //await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id.ToString(), update.CallbackQuery.Message.MessageId, "test");
                        if (update.CallbackQuery.Data == "Back")
                        {
                            var inlineKeyboard = GetDirectorie($"{WithCallbackData}");
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Нажмите на папку:", replyMarkup: inlineKeyboard);
                        }
                        else
                        {
                            Console.WriteLine(temp);
                            var inlineKeyboard = GetDirectorie($"{temp}");
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Нажмите на папку:", replyMarkup: inlineKeyboard);
                        }

                    }
                }
            }
            if (update.Type == UpdateType.Message)
            {
                if (message.Chat.Username == /*"Fillostrat"*/"Dominuskick")
                {
                    chatId = message.Chat.Id;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Hi Fillostrat");
                    try
                    {
                        var inlineKeyboard = GetDirectorie(TempPath);

                        if (message.Text.ToLower().StartsWith("/keys"))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Нажмите на папку:", replyMarkup: inlineKeyboard);
                        }

                    }
                    catch (Exception)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "File dont find");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "You do not have access");

                }
            }
        }
        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}