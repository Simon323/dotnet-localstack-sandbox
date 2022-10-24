using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Text;

namespace Localstack.S3.Services
{
    public interface IAmazonS3Service
    {
        Task ListBuckets();
        Task CreateBucket(string bucketName);
        Task ListObjectsInBucket(string bucketName);
        Task ReadTxtFile(string bucketName, string key);
        Task PutObject(string bucketName, string key, string body);
        Task StartUpload(string bucketName, string key);
        Task CompleteUpload();
    }

    public class AmazonS3Service : IAmazonS3Service
    {
        private readonly AmazonS3Client _client;

        public AmazonS3Service(AmazonS3Client client)
        {
            this._client = client;
        }

        public Task CompleteUpload()
        {
            throw new NotImplementedException();
        }

        public async Task CreateBucket(string bucketName)
        {
            var res = await this._client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = bucketName
            });

            Console.WriteLine(res.HttpStatusCode);
        }

        public async Task ListBuckets()
        {
            try
            {
                var response = await this._client.ListBucketsAsync();

                foreach (var bucket in response.Buckets)
                {
                    Console.WriteLine(bucket.BucketName);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task ListObjectsInBucket(string bucketName)
        {
            var objects = await this._client.ListObjectsAsync(bucketName);
            if (objects != null)
            {
                if (objects.S3Objects.Count == 0)
                {
                    Console.WriteLine($"Bucket {bucketName} is empty");
                }
                else
                {
                    Console.WriteLine($"For bucket {bucketName}, file: {string.Join(",", objects.S3Objects.Select(x => x.Key))}");
                }
            }
        }

        public async Task PutObject(string bucketName, string key, string body)
        {
            var res = await this._client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                ContentBody = body
            });

            Console.WriteLine(res.HttpStatusCode);
        }

        public async Task ReadTxtFile(string bucketName, string key)
        {
            var objectResponse = await this._client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            });

            var bytes = new byte[objectResponse.ResponseStream.Length];
            objectResponse.ResponseStream.Read(bytes, 0, bytes.Length);
            Console.WriteLine(Encoding.UTF8.GetString(bytes));
        }

        public async Task StartUpload(string bucketName, string key)
        {
            var fileTransferUtility = new TransferUtility(this._client);
            var s = "tst";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;


            await fileTransferUtility.UploadAsync(new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = stream
            });

            //new TransferUtility(s3Client)
            //var res = await this._client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
            //{

            //});
        }
    }
}
