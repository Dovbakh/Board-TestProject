using Board.Contracts.Contexts.Images;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Images.Services
{
    public interface IImageService
    {
        public Task<ImageShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation);
        public Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation);
        public Task<ImageData> DownloadAsync(Guid id, CancellationToken cancellation);
        public Task DeleteAsync(Guid id, CancellationToken cancellation);

        Task<bool> IsImageExists(Guid id, CancellationToken cancellation);
    }
}
