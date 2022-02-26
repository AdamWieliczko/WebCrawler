using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    internal class Program
    {

        public static void Main(String[] args)
        {
            Crawler.queue.Enqueue(("http://en.wikipedia.org/wiki/Special:Random", 0, new List<string>()));
            Crawler.finalDepth = -1;
            Crawler.finalPath = new List<string>();
            var crawler = new Crawler();
            Stopwatch k = new Stopwatch();
            int howManyTasks = 10;
            k.Start();
            Task[] tasks = new Task[howManyTasks];
            for(int i = 0; i < howManyTasks; i++)
            {
                tasks[i] = new Task(() => crawler.startCrawler());
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            if(Crawler.finalDepth >= 0)
            {
                k.Stop();
                Console.WriteLine("Found target in " + Crawler.finalDepth + " time " + k.ElapsedMilliseconds);
                foreach(var dir in Crawler.finalPath)
                {
                    Console.Write(dir + " -> ");
                }
                Console.Write(Crawler.target);
            }
            else
            {
                Console.WriteLine("Can't find connection to target");
            }
        }
    }
}
