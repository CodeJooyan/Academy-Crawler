using CrawlerService;
using TelegramBotService;

class Program
{
    static async Task Main(string[] args)
    {
        List<Course> courses;

        Console.WriteLine("Welcome back !");
        courses = StartGetCourses() ?? new List<Course>();
        Console.WriteLine("what should i do?");
        await CommandManager();
    }
    public static async Task StartTelegram(List<Course>? courses)
    {
        if (courses is null) await CommandManager();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Connecting To Telegram bot");
        Console.ResetColor();
        using (TelegramBot telegramBot = new TelegramBot(courses!))
        {
            await telegramBot.RunBot();
            Console.ReadKey();

            telegramBot.Dispose();
        }
    }
    public static List<Course>? StartGetCourses()
    {
        List<Course> courses = new List<Course>();
        var crawler = new Crawler();

        try
        {
            // courses.AddRange(crawler.GetAllCourses());
            courses.AddRange(crawler.GetCourses(crawler.GetLinks()));

            return courses;
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Something Went Wrong! cant get all of courses");
            return null;
        }
    }
    public static async Task CommandManager()
    {
        Console.WriteLine("Write a commnad");
        string? command = Console.ReadLine();

        if (command is null) await CommandManager();

        switch (command)
        {
            case "restart":
                List<Course>? courses = StartGetCourses();
                await StartTelegram(courses);
                await CommandManager();
                break;
        }
    }
}