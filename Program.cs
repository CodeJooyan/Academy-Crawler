using PotatoService;

class Program
{
    static void Main()
    {
        Potato potato = new Potato();
        var links = potato.GetLinks();
        List<Course> courses = new List<Course>();
        courses = potato.GetCourses(links);
        var courseMessages = potato.GetCourses(courses);
    }
}