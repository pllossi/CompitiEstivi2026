using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IBlogService
    {
        public Task CreateArticleAsync(ArticleCreateDto articleDto);

        public Task<ArticleReadDto?> GetArticleByIdAsync(string id);

        public Task<IEnumerable<ArticleReadDto>> GetAllArticlesAsync();

        public Task UpdateArticleAsync(string id, ArticleCreateDto articleDto);

        public Task DeleteArticleAsync(string id);

        public Task<IEnumerable<ArticleReadDto>> GetArticleByTitleAsync(string title);

        public Task<IEnumerable<ArticleReadDto>> GetArticleByContentAsync(string content);

        public Task<IEnumerable<ArticleReadDto>> GetArticleByDateAsync(DateOnly date);

        public Task<IEnumerable<ArticleReadDto>> GetArticleInPeriodAsync(DateOnly startingDate, DateOnly finishingDate);

        public Task<int> GetCountByDateAsync(DateOnly date);
    }

}
