using AutoMapper;
using FileStorage.Contracts.Contexts.Files;
using FileStorage.Contracts.Clients.Files;
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

namespace FileStorage.Clients.Contexts.Files
{
    public class FileClient : IFileClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public FileClient(HttpClient httpClient, IConfiguration configuration, IMapper mapper)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<FileShortInfoClientResponse> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/info/{id.ToString()}";  //"?" + nameof(offset) + "=" + offset.ToString() + 
            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var fileInfo = await response.Content.ReadFromJsonAsync<FileShortInfo>();
            var clientResponse = _mapper.Map<FileShortInfo, FileShortInfoClientResponse>(fileInfo);

            return clientResponse;
        }

        public async Task<FileDataClientResponse> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/files/{id.ToString()}";  //"?" + nameof(offset) + "=" + offset.ToString() + 
            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var fileData = await response.Content.ReadFromJsonAsync<FileData>();
            var clientResponse = _mapper.Map<FileData, FileDataClientResponse>(fileData);

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

    }
}
