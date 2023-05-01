using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.ObjectStorage
{
    public class MinioStorage : IObjectStorage
    {
        protected MinioClient _storage { get; }
        private readonly IConfiguration _configuration;

        private const string MinioAccessName = "MinioFileStorage";

        public MinioStorage(IConfiguration configuration)
        {
            _configuration = configuration;
            _storage = CreateStorage();
        }

        public async Task<byte[]> GetData(string objectName, string bucketName, CancellationToken cancellation)
        {
            var bktExistArgs = new BucketExistsArgs().WithBucket(bucketName);
            var found = await _storage.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                throw new DirectoryNotFoundException("Не найдена директория.");
            }

            MemoryStream memoryStream = new MemoryStream();

            var args = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithCallbackStream((stream) => stream.CopyTo(memoryStream));

            await _storage.GetObjectAsync(args, cancellation);

            return memoryStream.ToArray();
        }

        public async Task<ObjectStat> GetInfo(string objectName, string bucketName, CancellationToken cancellation)
        {
            var bktExistArgs = new BucketExistsArgs().WithBucket(bucketName);
            var found = await _storage.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                throw new DirectoryNotFoundException("Не найдена директория.");
            }

            var args = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

            var objectStat = await _storage.StatObjectAsync(args, cancellation);

            return objectStat;
        }

        public async Task Upload(string objectName, string bucketName, string contentType, byte[] bytes, CancellationToken cancellation)
        {
            var bktExistArgs = new BucketExistsArgs().WithBucket(bucketName);
            var found = await _storage.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                var mkBktArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                await _storage.MakeBucketAsync(mkBktArgs, cancellation);
            }

            using (var memoryStream = new MemoryStream(bytes))
            {
                var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(memoryStream)
                .WithObjectSize(memoryStream.Length)
                .WithContentType(contentType);

                await _storage.PutObjectAsync(args);
            }
        }

        public async Task Delete(string objectName, string bucketName, CancellationToken cancellation)
        {
            var bktExistArgs = new BucketExistsArgs().WithBucket(bucketName);
            var found = await _storage.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                throw new DirectoryNotFoundException("Директория не найдена.");
            }

            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            await _storage.RemoveObjectAsync(args, cancellation);
        }

        private MinioClient CreateStorage()
        {
            var endpoint = "127.0.0.1:9000"; // _configuration.GetSection(MinioAccessName).GetRequiredSection("Endpoint").Value;
            var accessKey = "ykE4mQ5LGAnFpqU5"; // _configuration.GetSection(MinioAccessName).GetRequiredSection("AccessKey").Value;
            var secretKey = "fFJTD2SX7LnoAldmFNvPwdU2fa9aWGNY"; // _configuration.GetSection(MinioAccessName).GetRequiredSection("SecretKey").Value;

            var minioCLient = new MinioClient();
            minioCLient.WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                    .WithSSL(false)
                    .Build();       

            return minioCLient;
        }
    }
}
