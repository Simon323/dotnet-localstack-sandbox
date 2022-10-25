using System.Diagnostics;

namespace Localstack.Sandbox.Console
{
    class Program
    {
        static void Main()
        {
            // ThreadPool throttling may cause the speed with which
            // the threads are launched to be throttled.
            // You can avoid that by uncommenting the following line,
            // but that is considered bad form:

            // ThreadPool.SetMinThreads(20, 20);

            var sw = Stopwatch.StartNew();
            System.Console.WriteLine("Waiting for all tasks to complete");

            RunWorkersVol1().Wait();

            System.Console.WriteLine("All tasks completed in " + sw.Elapsed);

            //sw = Stopwatch.StartNew();
            //System.Console.WriteLine("Waiting for all tasks to complete");

            //RunWorkersVol2().Wait();

            //System.Console.WriteLine("All tasks completed in " + sw.Elapsed);
        }

        public static async Task RunWorkersVol1()
        {
            await Task.WhenAll(
                JobDispatcher(6000, "task 1"),
                JobDispatcher(5000, "task 2"),
                JobDispatcher(4000, "task 3"),
                JobDispatcher(3000, "task 4"),
                JobDispatcher(2000, "task 5"),
                JobDispatcher(1000, "task 6")
            );
        }

        public static async Task RunWorkersVol2()
        {
            Task[] tasks = new Task[6];

            for (int i = 0; i < 6; ++i)
                tasks[i] = JobDispatcher(1000 + i * 1000, "task " + i);

            await Task.WhenAll(tasks);
        }

        public static async Task JobDispatcher(int time, string query)
        {
            var results = await Task.WhenAll(
                worker(time, query + ": Subtask 1"),
                worker(time, query + ": Subtask 2"),
                worker(time, query + ": Subtask 3")
            );

            System.Console.WriteLine(string.Join("\n", results));
        }

        static async Task<string> worker(int time, string query)
        {
            return await Task.Run(() =>
            {
                System.Console.WriteLine("Starting worker " + query);
                Thread.Sleep(time);
                System.Console.WriteLine("Completed worker " + query);
                return query + ": " + time + ", thread id: " + Thread.CurrentThread.ManagedThreadId;
            });
        }
    }
}