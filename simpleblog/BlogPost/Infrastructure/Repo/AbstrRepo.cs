using Application.Interface;
using Domain.Model.Entities;
using Infrastructure.Dto;
using Infrastructure.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Repo
{
    public abstract class AbstrRepo : IBlogRepository
    {
        internal string _filePath;
        internal readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public AbstrRepo(string? filePath = null)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_filePath))
            {
                SaveToFileAsync(new Dictionary<string, BlogPostPersistenceDto>());
            }
        }
        public async Task SaveAsync(BlogPost article)
        {
            var articles = await LoadFromFileAsync();
            var dto = article.ToPersistenceDto();
            articles[article.Id.ToString()] = dto;
            await SaveToFileAsync(articles);
        }

        internal abstract Task<Dictionary<string, BlogPostPersistenceDto>> LoadFromFileAsync();

        internal abstract Task SaveToFileAsync(Dictionary<string, BlogPostPersistenceDto> articles);

        public async Task<BlogPost?> GetByIdAsync(string id)
        {
            var articles = await LoadFromFileAsync();

            if (!articles.TryGetValue(id, out var dto))
                return null;

            return dto.ToEntity();
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            var articles = await LoadFromFileAsync();

            return articles.Values
                .Select(a => a.ToEntity())
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        public async Task UpdateAsync(BlogPost article)
        {
            var articles = await LoadFromFileAsync();
            var idKey = article.Id.ToString();

            if (!articles.ContainsKey(idKey))
                throw new InvalidOperationException($"Articolo con ID {article.Id} non trovato");

            articles[idKey] = article.ToPersistenceDto();

            await SaveToFileAsync(articles);
        }

        public async Task DeleteAsync(string id)
        {
            var articles = await LoadFromFileAsync();

            if (!articles.ContainsKey(id))
                throw new InvalidOperationException($"Articolo con ID {id} non trovato");

            articles.Remove(id);

            await SaveToFileAsync(articles);
        }

        public async Task<IEnumerable<BlogPost>> GetArticleByTitleAsync(string title)
        {
            var articles = await LoadFromFileAsync();
            return articles.Values
                .Where(a => a.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .Select(a => a.ToEntity())
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        public async Task<IEnumerable<BlogPost>> GetArticleByContentAsync(string content)
        {
            var articles = await LoadFromFileAsync();
            return articles.Values
                .Where(a => a.Content.Contains(content, StringComparison.OrdinalIgnoreCase))
                .Select(a => a.ToEntity())
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        public async Task<IEnumerable<BlogPost>> GetArticleByDateAsync(DateOnly date)
        {
            var articles = await LoadFromFileAsync();
            return articles.Values
                .Where(a => a.Timestamp.Equals(date.ToDateTime(TimeOnly.MinValue)))
                .Select(a => a.ToEntity())
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        public async Task<IEnumerable<BlogPost>> GetArticleInPeriodAsync(DateOnly startingDate, DateOnly finishingDate)
        {
            var articles = await LoadFromFileAsync();
            var startDateTime = startingDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = finishingDate.ToDateTime(TimeOnly.MaxValue);

            return articles.Values
                .Where(a =>
                    a.Timestamp >= startDateTime.Ticks &&
                    a.Timestamp <= endDateTime.Ticks)
                .Select(a => a.ToEntity())
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        public async Task<int> GetCountByDateAsync(DateOnly date)
        {
            var articles = await LoadFromFileAsync();
            var targetDateTime = date.ToDateTime(TimeOnly.MinValue);
            int count = 0;
            foreach (var article in articles.Values)
            {
                var articleDateTime = new DateTime(article.Timestamp);
                if (articleDateTime.Date == targetDateTime.Date)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
