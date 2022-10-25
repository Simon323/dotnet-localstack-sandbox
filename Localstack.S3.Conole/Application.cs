using Localstack.S3.Services;
using System.Diagnostics;

namespace Localstack.S3.Console
{
    public class Application : AbstractApplication
    {
        private readonly IAmazonS3Service anazonS3Service;

        public Application(IAmazonS3Service anazonS3Service)
        {
            this.anazonS3Service = anazonS3Service;
        }

        public override async Task Run()
        {
            //////////Put
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await Process5();
            stopwatch.Stop();
            System.Console.WriteLine($"Elapsed time = {stopwatch.ElapsedMilliseconds / 1000} s");

            //////////Delete
            //await this.Process6();
        }

        public async Task RunPOC()
        {


            //////////Put
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await this.Process7();
            stopwatch.Stop();
            System.Console.WriteLine($"Elapsed time = {stopwatch.ElapsedMilliseconds / 1000} s");

            await this.Process6();
        }

        public async Task Process7()
        {
            var no = 4;
            Task[] tasks = new Task[no];

            for (int i = 0; i < no; ++i)
                tasks[i] = Process71(i);

            await Task.WhenAll(tasks);
        }

        public async Task Process71(int numer)
        {
            var offset = numer * 1000;
            var results = await Task.WhenAll(
                Process711(offset + 0, offset + 250),
                Process711(offset + 250, offset + 500),
                Process711(offset + 500, offset + 750),
                Process711(offset + 750, offset + 1000)
            );

            System.Console.WriteLine(string.Join("\n", results));
        }

        public async Task<string> Process711(int start, int end)
        {
            const string bucketName = "test";
            const string fileName = "file_x";

            return await Task.Run(async () =>
            {
                for (int no = start; no < end; no++)
                {
                    await this.anazonS3Service.PutObject(bucketName, $"{fileName}{no}.txt", $"content-{no}");
                }
                return "";
            });
        }

        public async Task Process6() //POC
        {
            const string bucketName = "test";
            await this.anazonS3Service.DeleteObjects(bucketName);
        }

        public async Task Process5() //POC
        {
            const string bucketName = "test";

            await this.anazonS3Service.CreateBucket(bucketName);

            var no = 40;
            Task[] tasks = new Task[no];

            for (int i = 0; i < no; ++i)
                tasks[i] = Process51(i);

            await Task.WhenAll(tasks);
            System.Console.WriteLine("wykonane");
        }

        public async Task Process51(int number)
        {
            var kek = 100;
            const string bucketName = "test";
            const string fileName = "file_x";

            for (int i = 0; i < kek; i++)
            {
                var no = number * kek + i;
                await this.anazonS3Service.PutObject(bucketName, $"{fileName}{no}.txt", $"{no}");
            }
        }

        public async Task Process4()
        {
            const string bucketName = "test";
            const string fileName = "file_x";

            await this.anazonS3Service.CreateBucket(bucketName);

            var options = new ParallelOptions { MaxDegreeOfParallelism = 1 };

            Parallel.For(0, 50, options, async number =>
            {
                //System.Console.WriteLine(number);
                await this.anazonS3Service.PutObject(bucketName, $"{fileName}{number}.txt", $"{number}");
                //System.Console.WriteLine(number);
                System.Console.WriteLine(@"value of i = {0}, thread = {1}", number, Thread.CurrentThread.ManagedThreadId);
            });

            System.Console.WriteLine("End");
        }

        public async Task Process1()
        {
            await this.anazonS3Service.ListBuckets();
            //await this.anazonS3Service.ListObjectsInBucket("keke");
            //await this.anazonS3Service.ReadTxtFile("keke", "kaioken.txt");
            //await this.anazonS3Service.PutObject("keke", "kaioken.txt", "DDD");
            const string bucketName = "test";
            const string fileName = "file_x";

            await this.anazonS3Service.CreateBucket(bucketName);

            for (int i = 0; i < 100; i++)
            {
                await this.anazonS3Service.PutObject(bucketName, $"{fileName}{i}.txt", $"{i}");
            }

            //var options = new ParallelOptions { MaxDegreeOfParallelism = 1 };

            //Parallel.For(0, 3000, options, async number =>
            //{
            //    await this.anazonS3Service.PutObject(bucketName, $"{fileName}{number}.txt", $"{number}");
            //    //System.Console.WriteLine(number);
            //    //System.Console.WriteLine(@"value of i = {0}, thread = {1}", number, Thread.CurrentThread.ManagedThreadId);
            //});

            System.Console.WriteLine("End");
        }

        public async Task Process2()
        {
            const string fileName = "file_x";
            const string bucketName = "test";

            await this.anazonS3Service.CreateBucket(bucketName);

            Parallel.For(0, 100, async number =>
            {

                //await this.anazonS3Service.StartUpload("tst", $"{fileName}{number}.txt");
                await this.anazonS3Service.PutObject(bucketName, $"{fileName}{number}.txt", $"{number}");
            });

            //for (int i = 0; i < 100; i++)
            //{
            //    await this.anazonS3Service.StartUpload("tst", $"{fileName}{i}.txt");
            //}
        }

        public async Task Process3()
        {
            //Parallel.For(0, 3, number =>
            //{
            //    System.Console.WriteLine(number);
            //});

            //return;
            const string bucketName = "test";
            const string fileName = "file_x";

            await this.anazonS3Service.CreateBucket(bucketName);

            //for (int i = 0; i < 3000; i++)
            //{
            //    await this.anazonS3Service.PutObject(bucketName, $"{fileName}{i}.txt", $"{i}");
            //}

            var options = new ParallelOptions { MaxDegreeOfParallelism = 1 };

            Parallel.For(0, 2, options, async number =>
            {
                //for (int i = 0; i < 100; i++)
                //{
                //    var no = number * 100 + i;
                //    await this.anazonS3Service.PutObject(bucketName, $"{fileName}{no}.txt", $"{no}");
                //}

                await this.Generate(number);

                //System.Console.WriteLine(number);
                //System.Console.WriteLine(@"value of i = {0}, thread = {1}", number, Thread.CurrentThread.ManagedThreadId);
            });

            System.Console.WriteLine("End");
        }

        public async Task Generate(int number)
        {
            const string bucketName = "test";
            const string fileName = "file_x";

            for (int i = 0; i < 100; i++)
            {
                var no = number * 100 + i;
                await this.anazonS3Service.PutObject(bucketName, $"{fileName}{no}.txt", $"content-{no}");
            }
        }
    }
}
