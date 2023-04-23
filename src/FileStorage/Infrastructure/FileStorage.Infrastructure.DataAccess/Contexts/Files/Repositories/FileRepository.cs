using FileStorage.Application.AppData.Contexts.Files.Repositories;
using FileStorage.Contracts.Contexts.Files;
using FileStorage.Infrastructure.ObjectStorage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.DataAccess.Contexts.Files.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly IObjectStorage _objectStorage;

        public FileRepository(IObjectStorage objectStorage)
        {
            _objectStorage = objectStorage;
        }
        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            return _objectStorage.Delete(id.ToString(), "images", cancellation);
        }

        public async Task<FileData> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            var fileBytes = await _objectStorage.GetData(id.ToString(), "images", cancellation);

            var file = new FileData { Name = "name", Content = fileBytes, ContentType = "image/jpeg" };

            return file;
        }

        public async Task<FileShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            var objectStat = await _objectStorage.GetInfo(id.ToString(), "images", cancellation);

            var fileInfo = new FileShortInfo { Name = objectStat.ObjectName, CreatedAt = objectStat.LastModified, Id = id, Length = objectStat.Size, ContentType = objectStat.ContentType };

            return fileInfo;
        }

        public async Task<Guid> UploadAsync(string contentType, byte[] bytes, CancellationToken cancellation)
        {
            var fileName = Guid.NewGuid();
            var fileFolder = "images";

            await _objectStorage.Upload(fileName.ToString(), fileFolder, contentType, bytes, cancellation);

            return fileName;
        }
    }
}
