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
        protected MinioClient _minioClient;
        private readonly ILogger<MinioStorage> _logger;

        public MinioStorage(ILogger<MinioStorage> logger, MinioClient minioClient)
        {
            _logger = logger;
            _minioClient = minioClient;
        }

        /// <inheritdoc />
        public async Task<byte[]> GetData(string objectName, string bucketName, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Скачивание байтов из обьекта с именем: {1} в корзине {2}",
                nameof(GetData), objectName, bucketName);

            var bktExistArgs = new BucketExistsArgs().WithBucket(bucketName);
            var found = await _minioClient.BucketExistsAsync(bktExistArgs, cancellation);
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
                await _minioClient.GetObjectAsync(args, cancellation);
            }
            catch (Exception ex) 
            {
                if (ex is ObjectNotFoundException)
                {
                    return null;
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
            var found = await _minioClient.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                throw new KeyNotFoundException($"Обьект с именем {objectName} не найден.");
            }

            var args = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

            try
            {
                var objectStat = await _minioClient.StatObjectAsync(args, cancellation);
                return objectStat;
            }
            catch (Exception ex)
            {
                if (ex is ObjectNotFoundException)
                {
                    return null;
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
            var found = await _minioClient.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                var mkBktArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(mkBktArgs, cancellation);
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
                    await _minioClient.PutObjectAsync(args);
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
            var found = await _minioClient.BucketExistsAsync(bktExistArgs, cancellation);
            if (!found)
            {
                throw new KeyNotFoundException($"Обьект с именем {objectName} не найден.");
            }

            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            try
            {
                await _minioClient.RemoveObjectAsync(args, cancellation);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при работе с обьектным хранилищем.", ex);
            }
        }
    }
}
