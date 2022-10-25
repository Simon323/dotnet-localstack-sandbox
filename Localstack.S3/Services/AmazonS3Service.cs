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
        Task DeleteObjects(string bucketName);
    }

    public class AmazonS3Service : IAmazonS3Service
    {
        private readonly AmazonS3Client _client;

        public AmazonS3Service(AmazonS3Client client)
        {
            this._client = client;
        }

        public async Task CompleteUpload()
        {
            var fileTransferUtility = new TransferUtility(this._client);

            TransferUtilityUploadDirectoryRequest request = new TransferUtilityUploadDirectoryRequest
            {
                BucketName = "yourbucket",
                Directory = "C:\\lab\\docker\\docker-containers\\localstack-s3\\local_folder",
                SearchOption = System.IO.SearchOption.AllDirectories,
                CannedACL = S3CannedACL.PublicRead
            };

            await fileTransferUtility.UploadDirectoryAsync(request);

        }

        public async Task CreateBucket(string bucketName)
        {
            var res = await this._client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = bucketName
            });

            Console.WriteLine(res.HttpStatusCode);
        }

        public async Task DeleteObjects(string bucketName)
        {
            try
            {
                var list = Enumerable.Range(0, 20000).Select(x => new KeyVersion { Key = $"file_x{x}.txt" }).ToList();

                DeleteObjectsRequest multiObjectDeleteRequest = new DeleteObjectsRequest
                {
                    BucketName = bucketName,
                    Objects = list
                };
                // You can add specific object key to the delete request using the .AddKey.
                // multiObjectDeleteRequest.AddKey("TickerReference.csv", null);
                DeleteObjectsResponse response = await this._client.DeleteObjectsAsync(multiObjectDeleteRequest);

                Console.WriteLine("Successfully deleted all the {0} items", response.DeletedObjects.Count);
            }
            catch (DeleteObjectsException e)
            {
                PrintDeletionErrorStatus(e);
            }
        }

        private static void PrintDeletionErrorStatus(DeleteObjectsException e)
        {
            // var errorResponse = e.ErrorResponse;
            DeleteObjectsResponse errorResponse = e.Response;
            Console.WriteLine("x {0}", errorResponse.DeletedObjects.Count);

            Console.WriteLine("No. of objects successfully deleted = {0}", errorResponse.DeletedObjects.Count);
            Console.WriteLine("No. of objects failed to delete = {0}", errorResponse.DeleteErrors.Count);

            Console.WriteLine("Printing error data...");
            foreach (DeleteError deleteError in errorResponse.DeleteErrors)
            {
                Console.WriteLine("Object Key: {0}\t{1}\t{2}", deleteError.Key, deleteError.Code, deleteError.Message);
            }
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

            //Console.WriteLine(res.HttpStatusCode);
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
