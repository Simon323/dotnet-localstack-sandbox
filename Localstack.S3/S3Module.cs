using Amazon;
using Amazon.S3;
using Autofac;
using Localstack.S3.Services;

namespace Localstack.S3
{
    public class S3Module : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<AmazonS3Service>().As<IAmazonS3Service>();
            //builder.Register(c => new AmazonS3Client(new StoredProfileAWSCredentials("ls"), RegionEndpoint.EnumerableAllRegions.FirstOrDefault(x => x.SystemName == "eu-west-1"))).As<AmazonS3Client>();
            builder.Register(c =>
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USWest2,
                    ForcePathStyle = true,
                    ServiceURL = "http://localhost:4566"
                };
                return new AmazonS3Client("foo", "bar", config);
            }).As<AmazonS3Client>();
        }
    }
}