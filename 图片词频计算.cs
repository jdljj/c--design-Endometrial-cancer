using JiebaNet.Segmenter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace 图云词频计算
{
    class Program
    {
        private static List<string> need_words;
        private static double doc_num = 0;
        private static int year;
        static void Main(string[] args)
        {
            year = 1990;
            string path = @"D:\c#\图云词频计算\need_words.txt";   //路径
            string str = File.ReadAllText(path);
            need_words = str.Split('\n').ToList<string>();    //将需要的单词添加
            while (year <= 2020)
            {
                doc_num = 0;
                method2();
                words_rate();
                year++;
            }
        }
        public static void method2()
        {
            string path = @"D:\c#\stopwords-master\baidu_stopwords.txt";   //路径
            string str = File.ReadAllText(path);
            var stop_words = str.Split('\n');
            path = @"D:\c#\图云词频计算\"+year;  //文件夹位置
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                doc_num++;
                string fullName = f.FullName;
                var text = File.ReadAllText(fullName);
                string pattern = @"abstract:[\S\s]+";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(text);
                if (match.Groups.Count != 0)
                {
                    if (match.Groups[0].ToString().Length >= 9)
                        text = match.Groups[0].ToString().Substring(9);
                    else
                        text = "abstract";
                }
                else
                {
                    text = "abstract"; //防止出现“”
                }
                if (text == "") text = "abstract";
                var segmenter = new JiebaSegmenter();
                var segments = segmenter.Cut(text, cutAll: true);  // 默认为精确模式

                System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\c#\图云词频计算\"+year+"words.txt", true);  //不覆盖
                foreach (var temp in segments)
                {
                    if (!stop_words.Contains(temp))
                    {
                        file.WriteLine(temp);
                    }
                }
                file.Close();
            }
        }
        public static void words_rate()
        {
            string path = @"D:\c#\图云词频计算\words_rate.txt";   //路径
            var rateLines = File.ReadAllLines(path);   //保存了原来words_rate.txt的每一行
            string[] newrateLines = new string[rateLines.Length];

            foreach (var temp in rateLines)
            {
                Console.WriteLine(temp);
            }

            List<string> AllWords = new List<string>();
            List<int> AllFrequencies = new List<int>();
            var file_words = File.ReadAllText(@"D:\c#\图云词频计算\"+year+"words.txt");
            var words = file_words.Split('\n');
            foreach (var word in words)
            {
                if (AllWords.Contains(word))
                {
                    //如果已经存在就+1
                    AllFrequencies[AllWords.IndexOf(word)]++;
                }
                else
                {
                    bool result = false;
                    for (int j = 0; j < word.Length; j++)
                    {
                        if (Char.IsNumber(word, j))
                        {
                            result = true;
                        }
                    }
                    if (!result)
                    {
                        //如果不存在 且不为数字就添加
                        AllWords.Add(word);
                        AllFrequencies.Add(1);
                    }
                }
            }
            int index = 0;
            foreach (var temp in AllFrequencies)
            {
                if (need_words.Contains(AllWords[index]))
                {
                    Console.WriteLine(need_words.IndexOf(AllWords[index]));
                    newrateLines[need_words.IndexOf(AllWords[index]) + 1] = (double)1000*temp/doc_num + " " + rateLines[need_words.IndexOf(AllWords[index]) + 1];
                }
                index++;
            }
            System.IO.StreamWriter fileio = new System.IO.StreamWriter(@"D:\c#\图云词频计算\words_rate.txt", false);  //覆盖
            newrateLines[0] = year+" " + rateLines[0];
            foreach (var temp in newrateLines)
            {
                if (temp != "")
                {
                    Console.WriteLine(temp);
                    fileio.WriteLine(temp);
                }
            }
            fileio.Close();

        }
    }
}
