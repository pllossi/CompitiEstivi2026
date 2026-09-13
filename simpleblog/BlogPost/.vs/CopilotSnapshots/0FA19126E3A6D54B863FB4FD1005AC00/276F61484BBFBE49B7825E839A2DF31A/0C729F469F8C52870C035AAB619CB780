using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Dto;
using Domain.Model.Entities;

namespace Infrastructure.Mapper
{
    /// <summary>
    /// Mapper statico per la conversione tra entità di dominio e DTO di persistenza.
    /// Gestisce la conversione tra DateTime (usato nel dominio) e Unix timestamp (usato in Firebase).
    /// I metodi di estensione permettono di chiamare le conversioni direttamente sugli oggetti.
    /// </summary>
    public static class PersistenceMapper
    {
        /// <summary>
        /// Converte un'entità di dominio BlogPost in un DTO per la persistenza su Firebase.
        /// Il DateTime viene convertito in Unix timestamp (secondi dal 1 gennaio 1970).
        /// </summary>
        /// <param name="entity">Entità di dominio da convertire</param>
        /// <returns>DTO con i dati pronti per essere salvati su Firebase</returns>
        public static BlogPostPersistenceDto ToPersistenceDto(this BlogPost entity)
        {
            return new BlogPostPersistenceDto
            (
                // Converte il Guid in stringa per Firebase
                entity.Id.ToString(),
                
                // Titolo e contenuto rimangono stringhe
                entity.Title,
                entity.Content,
                
                // Converte DateTime in Unix timestamp (secondi dal 01/01/1970)
                // DateTimeOffset gestisce automaticamente il fuso orario
                ((DateTimeOffset)entity.CreatedAt).ToUnixTimeSeconds()
            );
        }

        /// <summary>
        /// Converte un DTO di persistenza in un'entità di dominio BlogPost.
        /// Il Unix timestamp viene convertito in DateTime nel fuso orario locale.
        /// IMPORTANTE: Usa LocalDateTime per ottenere l'ora corretta nel fuso orario dell'utente.
        /// </summary>
        /// <param name="dto">DTO recuperato da Firebase</param>
        /// <returns>Entità di dominio pronta per la logica di business</returns>
        public static BlogPost ToEntity(this BlogPostPersistenceDto dto)
        {
            return new BlogPost
            (
                // Converte la stringa in Guid
                new Guid(dto.Id),
                
                // Titolo e contenuto rimangono stringhe
                dto.Title,
                dto.Content,
                
                // Converte Unix timestamp in DateTime locale
                // FromUnixTimeSeconds crea un DateTimeOffset in UTC
                // LocalDateTime lo converte nel fuso orario locale dell'utente (es: UTC+1 per l'Italia)
                // Questo risolve il problema dell'orario sbagliato mostrato nell'UI
                DateTimeOffset.FromUnixTimeSeconds(dto.Timestamp).LocalDateTime
            );
        }

    }

}
