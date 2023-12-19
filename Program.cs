using CrawlerService;

class Program
{
    static void Main()
    {
        Crawler crawler = new Crawler();
        var links = crawler.GetLinks();
        List<Course> courses = new List<Course>();
        courses = crawler.GetCourses(links);
        var courseMessages = crawler.GetCourses(courses);
    }
}