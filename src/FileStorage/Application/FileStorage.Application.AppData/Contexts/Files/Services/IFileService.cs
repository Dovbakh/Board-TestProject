using FileStorage.Contracts.Contexts.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Application.AppData.Contexts.Files.Services
{
    public interface IFileService
    {
        public Task<FileShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation);
        public Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation);
        public Task<FileData> DownloadAsync(Guid id, CancellationToken cancellation);
        public Task DeleteAsync(Guid id, CancellationToken cancellation);

    }
}
