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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FileStorage.Clients.Contexts.Images
{
    public class ImageClient : IImageClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory; 
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<ImageClient> _logger;

        public ImageClient(HttpClient httpClient, IMapper mapper, IHttpContextAccessor contextAccessor, IHttpClientFactory httpClientFactory, 
            ILogger<ImageClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("FileClient");
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task<ImageShortInfoClientResponse> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/info/{id.ToString()}";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Получение информации об изображении с ID: {4}",
                nameof(ImageClient), nameof(GetInfoAsync), _httpClient.BaseAddress, uri, id);

            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var fileInfo = await response.Content.ReadFromJsonAsync<ImageShortInfo>();
            var clientResponse = _mapper.Map<ImageShortInfo, ImageShortInfoClientResponse>(fileInfo);

            return clientResponse;
        }

        public async Task<ImageDataClientResponse> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/{id.ToString()}";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Скачивание файла с ID: {4}",
                nameof(ImageClient), nameof(DownloadAsync), _httpClient.BaseAddress, uri, id);

            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var fileData = await response.Content.ReadFromJsonAsync<ImageData>();
            var clientResponse = _mapper.Map<ImageData, ImageDataClientResponse>(fileData);

            return clientResponse;
        }  

        public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation)
        {          
            var uri = $"v1/files/";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Загрука файла с содержимым: {4}",
                nameof(ImageClient), nameof(DownloadAsync), _httpClient.BaseAddress, uri, JsonConvert.SerializeObject(file));

            var multipartFormDataContent = new MultipartFormDataContent();
            byte[] fileData;
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                fileData = reader.ReadBytes((int)file.OpenReadStream().Length);
            }
            var fileContent = new ByteArrayContent(fileData);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            multipartFormDataContent.Add(fileContent, "file", file.FileName);

            using var response = await _httpClient.PostAsync(uri, multipartFormDataContent, cancellation);
            response.EnsureSuccessStatusCode();

            var fileId = await response.Content.ReadFromJsonAsync<Guid>();

            return fileId;
        }
        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/{id.ToString()}";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Удаление файла с ID: {4}",
                nameof(ImageClient), nameof(DownloadAsync), _httpClient.BaseAddress, uri, id);

            using var response = await _httpClient.DeleteAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> IsImageExists(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/exists/{id.ToString()}";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Проверка на наличие изображения с ID: {4}",
                nameof(ImageClient), nameof(DownloadAsync), _httpClient.BaseAddress, uri, id);

            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var clientResponse = await response.Content.ReadFromJsonAsync<bool>();

            return clientResponse;
        }
    }
}
