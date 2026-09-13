using Application.Dto;
using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public static class ArticleReadMapper
    {
        public static BlogPost ToEntity(this ArticleReadDto dto)
        {
            return new BlogPost(
                new Guid(dto.Id),
                dto.Title,
                dto.Content,
                dto.CreatedAt
            );
        }

        public static ArticleReadDto ToArticleReadDto(this BlogPost entity)
        {
            return new ArticleReadDto(
                entity.Id.ToString(),
                entity.Title,
                entity.Content,
                entity.CreatedAt
                );
        }
    }

}
