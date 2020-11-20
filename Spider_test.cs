using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

//https://www.jb51.net/article/177025.htm
// \u0022双引号 \s{1,}多个空格 \S+任意非空格 [^\n]一切非换行符 [\s\t\n]+多个空格或换行

namespace test
{
    class Program
    {
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        public static void GetInfomation(string html)
        {
            string pattern = @"<a\n\s{1,}class=\u0022docsum-title\u0022\n\s{1,}href=\u0022/(\d+)/\u0022\n\s{1,}ref=\u0022\S+\u0022\n\s{1,}data-ga-category=\u0022result_click\u0022";
            pattern += @"\s{1,}data-ga-action=\u0022\S+\u0022\n\s{1,}data-ga-label=\u0022\S+\u0022\n\s{1,}data-full-article-url=\u0022\S+\u0022\n\s{1,}data-article-id=\u0022\d+\u0022>\n";
            pattern += @"\s{1,}([^\n]+)\n\s{1,}</a>";
            MatchCollection matches = Regex.Matches(html, pattern);
            string pattern2 = @"<span class=\u0022docsum-journal-citation full-journal-citation\u0022>([^.]+). ([^;]+);";
            MatchCollection matches2 = Regex.Matches(html, pattern2);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i + 1 + "：");
                String url = "https://pubmed.ncbi.nlm.nih.gov/" + matches[i].Groups[1] +"/";
                Console.WriteLine("url={0}", url);
                String title = matches[i].Groups[2].ToString();
                title = title.Replace("<b>", "");
                title = title.Replace("</b>", "");
                Console.WriteLine("title={0}", title);
                Console.WriteLine("from:{0}", matches2[i].Groups[1]);
                Console.WriteLine("time={0}", matches2[i].Groups[2]);
                String abs_html = HttpGet(url, "");
                GetAbstract(abs_html);
                Console.WriteLine();
            }
        }

        static void GetAbstract(string html)
        {
            //string pattern = @"<div class=\u0022abstract-content selected\u0022\n\s{1,}id=\u0022enc-abstract\u0022>[\s\t\n]+<p>[\s\t\n]+([^\n]+)";
            //MatchCollection matches = Regex.Matches(html, pattern);
            //Console.WriteLine(matches[0].Groups[1]);
            
            //以上代码可以爬出大部分abstract，但是在有粗体的页面无效
            string pattern = @"<div class=\u0022abstract-content selected\u0022\n\s{1,}id=\u0022enc-abstract\u0022>*</div>";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);
            Console.WriteLine(match.Groups.Count);
            Console.WriteLine("{0}",match.Groups[0]);
            
            //以上代码暂时无效
            
        }

        static void Main(string[] args)
        {
            String info = HttpGet("https://pubmed.ncbi.nlm.nih.gov/?term=endometrial+cancer&size=10", "");
            GetInfomation(info);
        }
    }
}
