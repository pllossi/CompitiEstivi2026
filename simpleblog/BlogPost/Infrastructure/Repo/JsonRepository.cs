using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interface;
using Domain.Model.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;
using Infrastructure.Dto;
using Infrastructure.Mapper;
using System.Runtime.InteropServices;

namespace Infrastructure.Repo
{
    public class JsonRepository : AbstrRepo
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        public JsonRepository(string? filePath = null) : base(filePath)
        {
            filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BlogProject",
                "articles.json"
            );
        }

        internal async override Task<Dictionary<string, BlogPostPersistenceDto>> LoadFromFileAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                    return new Dictionary<string, BlogPostPersistenceDto>();

                var jsonString = await File.ReadAllTextAsync(_filePath);

                if (jsonString.Length == 0)
                    return new Dictionary<string, BlogPostPersistenceDto>();

                Stream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

                return await JsonSerializer.DeserializeAsync<Dictionary<string, BlogPostPersistenceDto>>(jsonStream, _jsonOptions) ?? new Dictionary<string, BlogPostPersistenceDto>();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        internal async override Task SaveToFileAsync(Dictionary<string, BlogPostPersistenceDto> articles)
        {
            await _semaphore.WaitAsync();
            try
            {
                using FileStream createStream = File.Create(_filePath);
                await JsonSerializer.SerializeAsync(createStream, articles, _jsonOptions);

            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
