using AutoMapper;
using FileStorage.Contracts.Contexts.Images;
using FileStorage.Contracts.Clients.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace FileStorage.Clients.Contexts.Images
{
    public class ImageClient : IImageClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public ImageClient(HttpClient httpClient, IConfiguration configuration, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _mapper = mapper;
            _contextAccessor = contextAccessor;

            SetToken();
        }

        public async Task<ImageShortInfoClientResponse> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/info/{id.ToString()}";  //"?" + nameof(offset) + "=" + offset.ToString() + 
            
            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var fileInfo = await response.Content.ReadFromJsonAsync<ImageShortInfo>();
            var clientResponse = _mapper.Map<ImageShortInfo, ImageShortInfoClientResponse>(fileInfo);

            return clientResponse;
        }

        public async Task<ImageDataClientResponse> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/{id.ToString()}";  //"?" + nameof(offset) + "=" + offset.ToString() + 
            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var fileData = await response.Content.ReadFromJsonAsync<ImageData>();
            var clientResponse = _mapper.Map<ImageData, ImageDataClientResponse>(fileData);

            return clientResponse;
        }  

        public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation)
        {
            var multipartFormDataContent = new MultipartFormDataContent();
            byte[] fileData;
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                fileData = reader.ReadBytes((int)file.OpenReadStream().Length);
            }
            var fileContent = new ByteArrayContent(fileData);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            multipartFormDataContent.Add(fileContent, "file", file.FileName);


            var uri = $"v1/files/";  //"?" + nameof(offset) + "=" + offset.ToString() + 
            using var response = await _httpClient.PostAsync(uri, multipartFormDataContent, cancellation);
            response.EnsureSuccessStatusCode();

            var fileId = await response.Content.ReadFromJsonAsync<Guid>();

            return fileId;
        }
        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/{id.ToString()}";  //"?" + nameof(offset) + "=" + offset.ToString() + 
            using var response = await _httpClient.DeleteAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> IsImageExists(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/exists/{id.ToString()}";  //"?" + nameof(offset) + "=" + offset.ToString() + 
            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var clientResponse = await response.Content.ReadFromJsonAsync<bool>();

            return clientResponse;
        }

        private void SetToken()
        {
            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
                return;

            token = token.Replace("Bearer ", "");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

    }
}
