using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.ObjectStorage
{
    /// <inheritdoc />
    public class MinioStorage : IObjectStorage
    {
        protected MinioClient _storage { get; }
        private readonly IConfiguration _configuration;
        private readonly ILogger<MinioStorage> _logger;
        private const string MinioAccessName = "MinioFileStorage";

        public MinioStorage(IConfiguration configuration, ILogger<MinioStorage> logger)
        {
            _configuration = configuration;
            _storage = CreateStorage();
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<byte[]> GetData(string objectName, string bucketName, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Скачивание байтов из обьекта с именем: {1} в корзине {2}",
                nameof(GetData), objectName, bucketName);

            var bktExistArgs = new BucketExistsArgs().WithBucket(bucketName);
            var found = await _storage.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                throw new KeyNotFoundException($"Обьект с именем {objectName} не найден.");
            }

            MemoryStream memoryStream = new MemoryStream();

            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream((stream) => stream.CopyTo(memoryStream));

            try
            {
                await _storage.GetObjectAsync(args, cancellation);
            }
            catch (Exception ex) 
            {
                if (ex is ObjectNotFoundException)
                {
                    throw new KeyNotFoundException($"Обьект с именем {objectName} не найден.");
                }

                throw new Exception("Ошибка при работе с обьектным хранилищем.", ex);
            }
           
            return memoryStream.ToArray();
        }

        /// <inheritdoc />
        public async Task<ObjectStat> GetInfo(string objectName, string bucketName, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации об обьекте с именем: {1} в корзине {2}",
                nameof(GetInfo), objectName, bucketName);

            var bktExistArgs = new BucketExistsArgs().WithBucket(bucketName);
            var found = await _storage.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                throw new KeyNotFoundException($"Обьект с именем {objectName} не найден.");
            }

            var args = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

            try
            {
                var objectStat = await _storage.StatObjectAsync(args, cancellation);
                return objectStat;
            }
            catch (Exception ex)
            {
                if (ex is ObjectNotFoundException)
                {
                    throw new KeyNotFoundException($"Обьект с именем {objectName} не найден.");
                }

                throw new Exception("Ошибка при работе с обьектным хранилищем.", ex);
            }          
        }

        /// <inheritdoc />
        public async Task Upload(string objectName, string bucketName, string contentType, byte[] bytes, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Загрука обьекта. Имя: {1}, корзина: {2}, тип контента: {3}, байты: {4}",
                nameof(Upload), objectName, bucketName, contentType, bytes);

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

                
                try
                {
                    await _storage.PutObjectAsync(args);
                }
                catch (Exception ex)
                {
                    if (ex is EntityTooLargeException)
                    {
                        throw new EntityTooLargeException($"Обьект превышает допустимый размер.");
                    }
                    throw new Exception("Ошибка при работе с обьектным хранилищем.", ex);
                }
            }
        }

        /// <inheritdoc />
        public async Task Delete(string objectName, string bucketName, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление обьекта с именем: {1} в корзине {2}",
                nameof(Delete), objectName, bucketName);

            var bktExistArgs = new BucketExistsArgs().WithBucket(bucketName);
            var found = await _storage.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                throw new KeyNotFoundException($"Обьект с именем {objectName} не найден.");
            }

            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            try
            {
                await _storage.RemoveObjectAsync(args, cancellation);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при работе с обьектным хранилищем.", ex);
            }
        }

        private MinioClient CreateStorage()
        {          
            var endpoint = "127.0.0.1:9000"; // _configuration.GetSection(MinioAccessName).GetRequiredSection("Endpoint").Value;
            var accessKey = "F0XUA4t6ERfJd0tM"; // _configuration.GetSection(MinioAccessName).GetRequiredSection("AccessKey").Value;
            var secretKey = "ghZPAqEdKuEMCx04VApe6Aes6dci6zg7"; // _configuration.GetSection(MinioAccessName).GetRequiredSection("SecretKey").Value;
            _logger.LogInformation("{0} -> Создание соединения с сервером MinIO: {1}",
                nameof(Delete), endpoint);

            var minioCLient = new MinioClient();
            minioCLient.WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                    .WithSSL(false)
                    .Build();       

            return minioCLient;
        }
    }
}
