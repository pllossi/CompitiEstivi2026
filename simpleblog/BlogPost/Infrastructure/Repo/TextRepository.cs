using Application.Interface;
using Domain.Model.Entities;
using global::Infrastructure.Dto;
using global::Infrastructure.Mapper;
using System.Text.Json;

namespace Infrastructure.Repo
{
    public class TextRepository : AbstrRepo
    {
        private static string varSep = " | ";
        private static string artSep = "\n------\n";
        public TextRepository(string? filePath = null) : base(filePath)
        {
            filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BlogProject",
                "articles.txt"
            );
        }
        internal async override Task<Dictionary<string, BlogPostPersistenceDto>> LoadFromFileAsync()
        {
            _semaphore.Wait();
            try
            {
                if (!File.Exists(_filePath))
                    return new Dictionary<string, BlogPostPersistenceDto>();

                var content = await File.ReadAllTextAsync(_filePath);

                if (string.IsNullOrWhiteSpace(content))
                    return new Dictionary<string, BlogPostPersistenceDto>();

                var articles = new Dictionary<string, BlogPostPersistenceDto>();
                var articleEntries = content.Split(artSep, StringSplitOptions.RemoveEmptyEntries);
                foreach (var articleEntry in articleEntries)
                {
                    var parts = articleEntry.Split(varSep, StringSplitOptions.RemoveEmptyEntries);
                    articles.Add(articleEntry.Split(varSep)[0], new BlogPostPersistenceDto(
                        parts[0],
                        parts[1],
                        parts[2],
                        long.Parse(parts[3])
                        ));
                }
                return articles;
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
                var content = string.Join(artSep, articles.Values.Select(dto =>
                    $"{dto.Id}{varSep}{dto.Title}{varSep}{dto.Content}{varSep}{dto.Timestamp}"
                ));
                await File.WriteAllTextAsync(_filePath, content);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}


