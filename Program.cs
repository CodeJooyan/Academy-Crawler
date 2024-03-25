using CrawlerService;
using TelegramBotService;

class Program
{
    static async Task Main(string[] args)
    {
        List<Course> courses = new List<Course>();
        try
        {
            courses = Crawler.GetLinks().GetCourses();

            await courses.SendToTelegramAsync();
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Something Went Wrong! cant get all of courses");
        }
    }
}