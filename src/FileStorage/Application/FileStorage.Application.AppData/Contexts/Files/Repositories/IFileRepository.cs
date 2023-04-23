using FileStorage.Contracts.Contexts.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Application.AppData.Contexts.Files.Repositories
{
    public interface IFileRepository
    {
        public Task<FileShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation);
        public Task<Guid> UploadAsync(string contentType, byte[] bytes, CancellationToken cancellation);
        public Task<FileData> DownloadAsync(Guid id, CancellationToken cancellation);
        public Task DeleteAsync(Guid id, CancellationToken cancellation);
    }
}
