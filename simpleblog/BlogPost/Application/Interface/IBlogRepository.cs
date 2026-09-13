using Application.Dto;
using Domain.Model.Entities;

namespace Application.Interface
{
    public interface IBlogRepository
    {
        Task SaveAsync(BlogPost article);
        Task<BlogPost?> GetByIdAsync(string id);
        Task<IEnumerable<BlogPost>> GetAllAsync();
        Task UpdateAsync(BlogPost article);
        Task DeleteAsync(string id);
        Task<IEnumerable<BlogPost>> GetArticleByTitleAsync(string title);
        Task<IEnumerable<BlogPost>> GetArticleByContentAsync(string content);
        Task<IEnumerable<BlogPost>> GetArticleByDateAsync(DateOnly date);
        Task<IEnumerable<BlogPost>> GetArticleInPeriodAsync(DateOnly startingDate, DateOnly finishingDate);
        Task<int> GetCountByDateAsync(DateOnly date);

    }
}
