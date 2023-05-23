using FileStorage.Contracts.Contexts.Images;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Application.AppData.Contexts.Images.Repositories
{
    public interface IImageRepository
    {
        public Task<ImageShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation);
        public Task<Guid> UploadAsync(string contentType, byte[] bytes, CancellationToken cancellation);
        public Task<ImageData> DownloadAsync(Guid id, CancellationToken cancellation);
        public Task DeleteAsync(Guid id, CancellationToken cancellation);

        Task<bool> IsExists(Guid id, CancellationToken cancellation);
    }
}
