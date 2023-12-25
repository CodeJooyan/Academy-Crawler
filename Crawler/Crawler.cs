using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Text;

namespace CrawlerService
{
    public class Crawler
    {
        public List<string> GetLinks()
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("https://barnamenevisan.info/").Result;
                var content = response.Content.ReadAsStringAsync().Result;
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);

                string regexPattern = @"/Course\d+/\S+";
                Regex regex1 = new Regex(regexPattern);

                var linkNodes = htmlDocument.DocumentNode.Descendants("a");
                HashSet<string> links = new HashSet<string>();

                foreach (var linkNode in linkNodes)
                {
                    var href = linkNode.GetAttributeValue("href", "");
                    if (href != null && regex1.IsMatch(href))
                    {
                        links.Add("https://barnamenevisan.info" + href);
                    }
                }

                return links.ToList();
            }
        }
        public List<Course> GetCourses(List<string> links)
        {
            List<Course> courses = new List<Course>();

            using (var httpClient = new HttpClient())
            {
                foreach (var link in links)
                {
                    var response = httpClient.GetAsync(link).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(content);
                    Course course = new Course();

                    //course name
                    HtmlNode courseNameNode = htmlDocument.DocumentNode.SelectSingleNode("//header[contains(@class, 'course-page-header')]");
                    if (courseNameNode != null)
                    {
                        HtmlNode heading1 = courseNameNode.SelectSingleNode(".//h1");
                        if (heading1 != null)
                        {
                            course.Title = heading1.InnerText;
                        }

                    }

                    //get short link
                    HtmlNode courseShortLink = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'LinK_Text_Box_shortlink')]");
                    if (courseShortLink != null)
                    {
                        HtmlNode shortLinkSapn = courseShortLink.SelectSingleNode("//span[contains(@class,'link')]");
                        if (shortLinkSapn != null)
                        {
                            course.ShortLink = shortLinkSapn.InnerText;
                        }
                    }

                    // Get master name
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

                    courses.Add(course);
                }
            }
            courses = courses.OrderBy(x => x.StartDate).ToList();
            return courses;
        }
        public List<string> GetCourses(List<Course> model)
        {
            List<string> messages = new List<string>();
            int i = 1;
            foreach (var item in model)
            {
                string path = @$"Desktop\TelegramAds\ads{i}.txt";
                string username = Environment.UserName;
                string directoryPath = $@"C:\Users\{username}\Desktop\TelegramAds\";
                string finalPath = $@"C:\Users\{username}\Desktop\TelegramAds\ads{item.Title}.txt";

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
                    writer.WriteLine(value: $"شروع دوره: {item.StartDate} {item.StartDate}");
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
    }
}