using CrawlerService;
using TelegramBotService;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Clear();
        List<Course> courses = new List<Course>();
        var crawler = new Crawler();

        try
        {
            // courses.AddRange(crawler.GetAllCourses());
            courses.AddRange(crawler.GetCourses(crawler.GetLinks()));
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Something Went Wrong! cant get all of courses");
        }
        using (TelegramBot telegramBot = new TelegramBot(courses)){
            await telegramBot.RunBot();
            Console.ReadKey();

            telegramBot.Dispose();
        }
    }
}