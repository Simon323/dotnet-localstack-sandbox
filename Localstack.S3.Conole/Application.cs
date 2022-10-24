using Localstack.S3.Services;

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
            //Process2();
            await Process1();
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

            //for (int i = 0; i < 3000; i++)
            //{
            //    await this.anazonS3Service.PutObject(bucketName, $"{fileName}{i}.txt", $"{i}");
            //}

            var options = new ParallelOptions { MaxDegreeOfParallelism = 1 };

            Parallel.For(0, 3000, options, async number =>
            {
                await this.anazonS3Service.PutObject(bucketName, $"{fileName}{number}.txt", $"{number}");
                //System.Console.WriteLine(number);
                //System.Console.WriteLine(@"value of i = {0}, thread = {1}", number, Thread.CurrentThread.ManagedThreadId);
            });

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
    }
}
