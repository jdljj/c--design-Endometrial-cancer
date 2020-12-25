using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

//https://www.jb51.net/article/177025.htm
// \u0022双引号 \s{1,}多个空格 \S+任意非空格 [^\n]一切非换行符 [\s\t\n]+多个空格或换行
// @"(?<=<div>)[\s\S]*?(?=</div>)" 提取两个div中的内容


namespace 子宫内膜爬虫
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

        public static void GetInfomation(string html, int size, string year)
        {
            string pattern = @"<a\n\s{1,}class=\u0022docsum-title\u0022\n\s{1,}href=\u0022/(\d+)/\u0022\n\s{1,}ref=\u0022\S+\u0022\n\s{1,}data-ga-category=\u0022result_click\u0022";
            pattern += @"\s{1,}data-ga-action=\u0022\S+\u0022\n\s{1,}data-ga-label=\u0022\S+\u0022\n\s{1,}data-full-article-url=\u0022\S+\u0022\n\s{1,}data-article-id=\u0022\d+\u0022>\n";
            pattern += @"\s{1,}([^\n]+)\n\s{1,}</a>";
            MatchCollection matches = Regex.Matches(html, pattern);
            string pattern2 = @"<span class=\u0022docsum-journal-citation full-journal-citation\u0022>([^.]+). ([^;]+);";
            MatchCollection matches2 = Regex.Matches(html, pattern2);
            for (int i = 0; i < size; i++)
            {
                if (matches[i] == null)
                {
                    break;
                }

                Console.WriteLine(i);
                String filename = "files/" + year + "/" + matches[i].Groups[1] + ".txt";
                StreamWriter sw = new StreamWriter(filename)
                {
                    AutoFlush = true
                };
                String url = "https://pubmed.ncbi.nlm.nih.gov/" + matches[i].Groups[1] + "/";
                sw.WriteLine("url=" + url);
                String title = matches[i].Groups[2].ToString();
                title = title.Replace("<b>", "");
                title = title.Replace("</b>", "");
                sw.WriteLine("title=" + title);
                sw.WriteLine("from:" + matches2[i].Groups[1]);
                sw.WriteLine("time=" + matches2[i].Groups[2]);
                String abs_html = HttpGet(url, "");
                String abstr = GetAbstract(abs_html);
                sw.Write("abstract: " + abstr);
            }
        }

        static String GetAbstract(string html)
        {
            string pattern = @"(?<=<div class=\u0022abstract-content selected\u0022\n\s{1,}id=\u0022enc-abstract\u0022>)[\s\S]*?(?=</div>)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);
            string result = match.Groups[0].ToString();
            result = result.Replace("\n", "");
            result = result.Replace("<p>", "");
            result = result.Replace("</p>", "");
            result = result.Replace("<strong class=\"sub-title\">", "");
            result = result.Replace("</strong>", "");
            result = result.Replace("  ", "");
            return result;
        }

        static void Main(string[] args)
        {
            string year = "2000";
            int index = 1;
            while (true)
            {
                int page = index;
                String info = HttpGet("https://pubmed.ncbi.nlm.nih.gov/?term=endometrial%20cancer&filter=years." + year + "-" + year + "&size=200&page=" + page, "");
                GetInfomation(info, 200, year);
                index++;
            }
        }
    }
}
