using FileStorage.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.ObjectStorage
{
    public class MinioClientConfiguration
    {
        private readonly MinioClientOptions _minioClientOptions;
        public MinioClientConfiguration(IOptions<MinioClientOptions> minioClientOptionsAccessor)
        {
            _minioClientOptions = minioClientOptionsAccessor.Value;
        }

        public MinioClient Configure()
        {
            var minioCLient = new MinioClient();
            minioCLient.WithEndpoint(_minioClientOptions.Endpoint)
                    .WithCredentials(_minioClientOptions.AccessKey, _minioClientOptions.SecretKey)
                    .WithSSL(false)
                    .Build();

            return minioCLient;
        }    
    }
}
