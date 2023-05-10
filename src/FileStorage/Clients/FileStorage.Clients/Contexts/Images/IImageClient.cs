using FileStorage.Contracts.Clients.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Clients.Contexts.Images
{
    public interface IImageClient
    {
        public Task<ImageShortInfoClientResponse> GetInfoAsync(Guid id, CancellationToken cancellation);
        public Task<ImageDataClientResponse> DownloadAsync(Guid id, CancellationToken cancellation);
        public Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation);
        public Task DeleteAsync(Guid id, CancellationToken cancellation);

        public Task<bool> IsImageExists(Guid id, CancellationToken cancellation);
    }
}
