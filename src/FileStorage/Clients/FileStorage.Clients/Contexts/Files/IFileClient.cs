using FileStorage.Contracts.Clients.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Clients.Contexts.Files
{
    public interface IFileClient
    {
        public Task<FileShortInfoClientResponse> GetInfoAsync(Guid id, CancellationToken cancellation);
        public Task<FileDataClientResponse> DownloadAsync(Guid id, CancellationToken cancellation);
        public Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation);
        public Task DeleteAsync(Guid id, CancellationToken cancellation);
    }
}
