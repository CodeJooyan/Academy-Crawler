using System.Text;
using CrawlerService;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotService
{
    public class TelegramBot : IDisposable
    {
        private readonly List<Course> _courses;
        public TelegramBot(List<Course> courses)
        {
            _courses = courses;
        }
        private TelegramBotClient _telegramBot;

        public async Task RunBot()
        {
            _telegramBot = new TelegramBotClient("");
            Console.WriteLine("Bot Connected");

            int offset = 0;
            while (true)
            {
                Update[] updates = await _telegramBot.GetUpdatesAsync(offset);

                foreach (var update in updates)
                {
                    offset = update.Id + 1;
                    if (update.Message == null || update.Message.Text == "")
                        continue;

                    string message = update.Message.Text!.ToLower();
                    var from = update.Message.From;
                    var chatId = update.Message.Chat.Id;

                    if (message.Contains("شروع"))
                    {
                        foreach (var item in _courses)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.AppendLine("✨آکادمی برنامه نویسان برگزار میکند ✨");
                            stringBuilder.AppendLine(item.Title);
                            stringBuilder.AppendLine($"مدرس : {item.MasterName}");
                            stringBuilder.AppendLine($"طول دوره: {item.Sections} ({item.HowLongIsCourse})");
                            stringBuilder.AppendLine($"شروع دوره: {item.StartDate} {item.StartDate}");
                            stringBuilder.AppendLine("به صورت حضوری و آنلاین");
                            stringBuilder.AppendLine("شعبه اصلی");
                            stringBuilder.AppendLine("");
                            stringBuilder.AppendLine("");
                            stringBuilder.AppendLine("برای مشاهده جزییات بیشتر روی لینک زیر کلیک کنید");
                            stringBuilder.AppendLine("");
                            stringBuilder.AppendLine(item.ShortLink?.Trim().ToLower());
                            stringBuilder.AppendLine("");
                            stringBuilder.AppendLine("");
                            stringBuilder.AppendLine("☎️ 021-91303737 -- 021-88454816");
                            stringBuilder.AppendLine("");
                            stringBuilder.AppendLine("🆔 @AcademyBarnamenevisan");
                            await _telegramBot.SendTextMessageAsync(chatId, stringBuilder.ToString());
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }
}