using System;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using HtmlAgilityPack;

namespace WebScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await WebCrawlerForHtmlAsync();

            MergeDocument();
        }

        private static async Task WebCrawlerForHtmlAsync()
        {
            var skipArray = new int[700];
            for (int i = 1310; i <= 1311; i++)
                //for (int i = 2; i <= 5; i++)
            {
                if (skipArray.Contains(i))
                {
                    continue;
                }
                //網址路徑
                string url = $"網址路徑";
                string filename = $"page_{(i - 1).ToString().PadLeft(5, '0')}.txt";

                try
                {
                    await WebCrawlerAsync(url, i, filename);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"處理 {url} 時發生錯誤：{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 合併各頁小說
        /// </summary>
        private static void MergeDocument()
        {
            //讀檔路徑
            string folderPath = @"路徑";
            string outputFile = "merged.txt";
            string[] inputFiles = Directory.GetFiles(folderPath, "*.txt");
            inputFiles = inputFiles.Where(x => !x.Contains(outputFile)).ToArray();
            using (StreamWriter writer = new StreamWriter(Path.Combine(folderPath, outputFile)))
            {
                foreach (string file in inputFiles)
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
            }

            Console.WriteLine("已完成合併。");
        }

        /// <summary>
        /// 網路小說爬蟲
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="i">The i.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="times">The times.</param>
        private static async Task WebCrawlerAsync(string url, int i, string filename,int times=0)
        {
            //存檔路徑
            var folderPath = @"路徑";
            var fullPath = Path.Combine(folderPath, filename);
            try
            {
                    // 下載網頁內容
                    // 建立 HttpClient
                    var client = new HttpClient();
                    // 透過 HttpClient 發送 GET 請求，並取得回應內容
                    var html = await client.GetStringAsync(url);

                    // 解析HTML

                    var decodedHtml = WebUtility.HtmlDecode(html);

                    var doc = new HtmlDocument();
                    doc.LoadHtml(decodedHtml);
                    //Console.WriteLine($"已讀取 {i-1}筆網頁");

                    // 找到文章內容
                    var contentNode = doc.GetElementbyId("nr1");
                    var content = contentNode.InnerText ?? "";


                    // 將資料寫入檔案


                    await using (var file = new StreamWriter(fullPath, false, Encoding.UTF8))
                    {
                        await file.WriteAsync(content);
                        Console.WriteLine($"已匯出 {filename}");
                    }
            }
            catch (Exception e)
            {
                    times += 1;
                    if (times <=5 )
                    {
                        Console.WriteLine($"{url}異常,times:{times}");
                        await WebCrawlerAsync(url, i, filename, times);

                    }
                    else
                    {
                        fullPath = Path.Combine(folderPath, $"error.txt");
                        await using var file = new StreamWriter(fullPath, true, Encoding.UTF8);
                        var loggerStr = @$" 
                                    異常:{i}
                                    異常網址:{url}
                                    異常檔案:{filename}
                                    異常訊息:{e}
                                  ";
                        await file.WriteLineAsync(loggerStr);
                    }

            }


        }
    }
}