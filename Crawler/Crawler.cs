using CrawlerService.Extentions;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using TelegramBotService;

namespace CrawlerService
{
    public static class Crawler
    {
        public static List<string> GetLinks()
        {
            using (var httpClient = new HttpClient())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Geting Courses Link From Barnamenevisan.info");
                Console.ResetColor();

                #region Get Html Response From URL

                var response = httpClient.GetAsync("https://barnamenevisan.info/").Result;
                var content = response.Content.ReadAsStringAsync().Result;
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);

                #endregion

                #region Get Link Nodes

                string regexPattern = @"/Course\d+/\S+";
                Regex regex1 = new Regex(regexPattern);

                var linkNodes = htmlDocument.DocumentNode.Descendants("a");
                HashSet<string> links = new HashSet<string>();

                #endregion

                int linksCount = 0;

                #region Convert Nodes To Normal Links

                foreach (var linkNode in linkNodes)
                {
                    var href = linkNode.GetAttributeValue("href", "");
                    if (href != null && regex1.IsMatch(href))
                    {
                        links.Add("https://barnamenevisan.info" + href);
                        linksCount++;
                        Console.WriteLine($"{linksCount} links found .....! {href}");
                    }
                }

                #endregion

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Links Taked..!");
                Console.ResetColor();

                return links.ToList();
            }
        }
        public static List<Course> GetCourses(this List<string> links)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("trying to get telegram courses from links");
            Console.ResetColor();

            List<Course> courses = new List<Course>();

            using (var httpClient = new HttpClient())
            {
                foreach (var link in links)
                {
                    #region Load Html From Url

                    var response = httpClient.GetAsync(link).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(content);

                    #endregion

                    Course course = new Course();

                    #region Get Course Details

                    #region Course Name

                    HtmlNode courseNameNode = htmlDocument.DocumentNode.SelectSingleNode("//header[contains(@class, 'course-page-header')]");
                    if (courseNameNode != null)
                    {
                        HtmlNode heading1 = courseNameNode.SelectSingleNode(".//h1");
                        if (heading1 != null)
                        {
                            course.Title = heading1.InnerText;
                        }

                    }

                    #endregion

                    #region ShortLink

                    HtmlNode courseShortLink = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'LinK_Text_Box_shortlink')]");
                    if (courseShortLink != null)
                    {
                        HtmlNode shortLinkSapn = courseShortLink.SelectSingleNode("//span[contains(@class,'link')]");
                        if (shortLinkSapn != null)
                        {
                            course.ShortLink = shortLinkSapn.InnerText;
                        }
                    }

                    #endregion

                    #region Course Master Name

                    HtmlNode mainInfoNode = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'course-details-right-side')]");
                    if (mainInfoNode != null)
                    {
                        HtmlNode heading2 = mainInfoNode.SelectSingleNode(".//h2");
                        if (heading2 != null)
                        {
                            HtmlNode masterName = heading2.SelectSingleNode(".//a");
                            if (masterName != null)
                            {
                                HtmlNode nodeToRemove = htmlDocument.DocumentNode.SelectSingleNode("//span[text()=' ( مشاهده رزومه ) ']");
                                nodeToRemove?.Remove();
                                course.MasterName = masterName.InnerText;
                            }

                        }

                        HtmlNode StartDate = mainInfoNode.SelectSingleNode("//span[contains(@class, 'education-page-card-main-item2 mr-5')]");
                        if (StartDate != null)
                        {
                            course.StartDate = StartDate.InnerText;
                        }
                    }

                    #endregion

                    #region Course Sections Count and houres

                    //get course sections
                    string searchText = "جلسه";
                    HtmlNode courseSection = htmlDocument.DocumentNode.SelectSingleNode($"//*[text()[contains(.,'{searchText}')]]");
                    if (courseSection != null)
                    {
                        course.Sections = courseSection.InnerText;
                    }

                    //get course full time hour
                    string searchText2 = "ساعت";
                    HtmlNode courseTime = htmlDocument.DocumentNode.SelectSingleNode($"//*[text()[contains(.,'{searchText2}')]]");
                    if (courseTime != null)
                    {
                        course.HowLongIsCourse = courseTime.InnerText;
                    }

                    #endregion

                    #region Get day of week

                    //course.DayOfWeek = course.StartDate.ToShamsiByString().GetPersianDayOfWeek();

                    #endregion

                    #endregion

                    courses.Add(course);
                    Console.WriteLine("***************************************************************");
                }
            }
            Console.Clear();
            courses = courses.OrderBy(x => x.StartDate).ToList();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("List Generated !");
            Console.ResetColor();

            return courses;
        }
        public static async Task SendToTelegramAsync(this List<Course> courses)
        {
            TelegramBot telegramBot = new TelegramBot(courses);

            await telegramBot.RunBot();
        }

        #region Archived
        public static List<string> WriteCoursesToDirectory(this List<Course> model)
        {
            List<string> messages = new List<string>();
            int i = 1;
            foreach (var item in model)
            {
                string path = @$"Desktop\TelegramAds\ads{i}.txt";
                string username = Environment.UserName;
                string directoryPath = $@"C:\Users\{username}\Desktop\TelegramAds\";
                string finalPath = $@"C:\Users\{username}\Desktop\TelegramAds\ads{i}.txt";

                if (!Directory.Exists(Path.GetDirectoryName(directoryPath)))
                {
                    Directory.CreateDirectory(directoryPath);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(@$"directory created in {directoryPath}");
                    Console.ResetColor();
                }

                using (StreamWriter writer = new StreamWriter(finalPath))
                {
                    writer.WriteLine("✨آکادمی برنامه نویسان برگزار میکند ✨");
                    writer.WriteLine(item.Title);
                    writer.WriteLine($"مدرس: {item.MasterName}");
                    writer.WriteLine($"طول دوره: {item.Sections} ({item.HowLongIsCourse})");
                    writer.WriteLine($"شروع دوره: {item.StartDate} {item.StartDate}");
                    writer.WriteLine("به صورت حضوری و آنلاین");
                    writer.WriteLine("شعبه اصلی");
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("برای مشاهده جزییات بیشتر روی لینک زیر کلیک کنید");
                    writer.WriteLine("");
                    writer.WriteLine(item.ShortLink?.Trim().ToLower());
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("☎️ 021-91303737 -- 021-88454816");
                    writer.WriteLine("");
                    writer.WriteLine("🆔 @AcademyBarnamenevisan");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(@$"file created in {finalPath}");
                    Console.ResetColor();
                }
                i++;
            }

            return messages;
        }
        #endregion

    }
}