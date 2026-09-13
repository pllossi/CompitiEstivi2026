namespace Application.Dto
{
    public record ArticleReadDto(
       string Id,
       string Title,
       string Content,
       DateTime CreatedAt
   );
}
