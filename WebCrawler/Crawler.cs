using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebCrawler
{
    internal class Crawler
    {
        public static int finalDepth;
        public static List<string> finalPath;
        public static String target = "http://en.wikipedia.org/wiki/Elizabeth_II";
        public static ConcurrentQueue<(string, int, List<string>)> queue = new(); //ostatni argument czyli lista stringów to lista przez jakie lokalizacje przechodzł Crawler

        public void startCrawler()
        {
            while (finalDepth == -1)
            {
                (string, int, List<string>) currentPair;
                if(!queue.TryDequeue(out currentPair))
                {
                    continue;
                }

                if(currentPair.Item1.Equals(target))
                {
                    finalPath = currentPair.Item3;
                    finalDepth = currentPair.Item2;
                    return;
                }

                WebClient web = new WebClient();
                String text = "";
                System.IO.Stream stream = web.OpenRead(currentPair.Item1);

                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }

                List<String> hrefList = new List<String>();

                Regex.Matches(text, "<a href=\"/wiki/([^\":()]*)\"", RegexOptions.Compiled)
                    .Cast<Match>().Select(i => i.Value).ToList().ForEach(i => hrefList.Add("http://en.wikipedia.org" + i.Substring(9, i.Length - 10)));

                //

                if(hrefList.Any(i => i.Equals(target)))
                {
                    var newList = new List<String>(currentPair.Item3);
                    newList.Add(currentPair.Item1);
                    finalPath = newList;
                    finalDepth = currentPair.Item2 + 1;
                    return;
                }

                foreach (var href in hrefList)
                {
                    var newList = new List<String>(currentPair.Item3);
                    newList.Add(currentPair.Item1);
                    queue.Enqueue((href, currentPair.Item2 + 1, newList));
                }
            }
        }
    }
}