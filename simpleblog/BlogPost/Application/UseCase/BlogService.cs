using Application.Interface;
using Domain.Model.Entities;
using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Mapper;
using System.Diagnostics.CodeAnalysis;

namespace Application.UseCase
{
    /// <summary>
    /// Servizio principale per la gestione degli articoli del blog.
    /// Implementa la logica di business e coordina le operazioni tra il ViewModel e il Repository.
    /// Segue il pattern Service Layer per separare la logica di business dall'accesso ai dati.
    /// </summary>
    public class BlogService : IBlogService
    {
        // Repository per l'accesso ai dati persistenti
        private readonly IBlogRepository _repository;

        /// <summary>
        /// Costruttore: riceve il repository tramite dependency injection
        /// </summary>
        public BlogService(IBlogRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Crea un nuovo articolo nel database.
        /// Converte il DTO in entità di dominio e lo salva tramite il repository.
        /// </summary>
        /// <param name="articleDto">DTO con i dati del nuovo articolo (titolo e contenuto)</param>
        public async Task CreateArticleAsync(ArticleCreateDto articleDto)
        {
            // Converte il DTO in entità di dominio (genera automaticamente ID e data)
            var entity = articleDto.ToEntity();

            // Salva l'entità nel database tramite il repository
            await _repository.SaveAsync(entity);
        }

        /// <summary>
        /// Recupera un articolo specifico tramite il suo ID.
        /// </summary>
        /// <param name="id">ID univoco dell'articolo da recuperare</param>
        /// <returns>DTO con i dati dell'articolo, oppure null se non trovato</returns>
        public async Task<ArticleReadDto?> GetArticleByIdAsync(string id)
        {
            // Recupera l'entità dal repository
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            // Converte l'entità in DTO per la visualizzazione
            return entity.ToArticleReadDto();
        }

        /// <summary>
        /// Recupera tutti gli articoli presenti nel database.
        /// Gli articoli sono ordinati per data di creazione (più recenti prima).
        /// </summary>
        /// <returns>Collezione di DTO con tutti gli articoli</returns>
        public async Task<IEnumerable<ArticleReadDto>> GetAllArticlesAsync()
        {
            // Recupera tutte le entità dal repository
            var entities = await _repository.GetAllAsync();

            // Converte ogni entità in DTO usando LINQ
            return entities.Select(e => e.ToArticleReadDto());
        }

        /// <summary>
        /// Aggiorna un articolo esistente con nuovi dati.
        /// Cerca l'articolo per ID, modifica i suoi dati e lo salva.
        /// </summary>
        /// <param name="id">ID dell'articolo da aggiornare</param>
        /// <param name="articleDto">DTO con i nuovi dati (titolo e contenuto)</param>
        /// <exception cref="InvalidOperationException">Lanciata se l'articolo non esiste</exception>
        public async Task UpdateArticleAsync(string id, ArticleCreateDto articleDto)
        {
            // Recupera l'entità esistente dal database
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"Article {id} not found");

            // Modifica solo i campi editabili (titolo e contenuto)
            // ID e data di creazione rimangono invariati
            entity.Title = articleDto.Title;
            entity.Content = articleDto.Content;

            // Salva le modifiche nel database
            await _repository.UpdateAsync(entity);
        }

        /// <summary>
        /// Elimina definitivamente un articolo dal database.
        /// </summary>
        /// <param name="id">ID dell'articolo da eliminare</param>
        public async Task DeleteArticleAsync(string id)
        {
            // Delega l'eliminazione al repository
            await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Cerca articoli il cui titolo contiene il testo specificato.
        /// La ricerca è case-insensitive (maiuscole/minuscole non contano).
        /// </summary>
        /// <param name="title">Testo da cercare nel titolo</param>
        /// <returns>Collezione di articoli che corrispondono alla ricerca</returns>
        public async Task<IEnumerable<ArticleReadDto>> GetArticleByTitleAsync(string title)
        {
            // Cerca nel repository e converte i risultati in DTO
            var articles = await _repository.GetArticleByTitleAsync(title);
            return articles.Select(a => a.ToArticleReadDto());
        }

        /// <summary>
        /// Cerca articoli il cui contenuto contiene il testo specificato.
        /// La ricerca è case-insensitive (maiuscole/minuscole non contano).
        /// </summary>
        /// <param name="content">Testo da cercare nel contenuto</param>
        /// <returns>Collezione di articoli che corrispondono alla ricerca</returns>
        public async Task<IEnumerable<ArticleReadDto>> GetArticleByContentAsync(string content)
        {
            // Cerca nel repository e converte i risultati in DTO
            var articles = await _repository.GetArticleByContentAsync(content);
            return articles.Select(a => a.ToArticleReadDto());
        }

        /// <summary>
        /// Cerca tutti gli articoli creati in una data specifica.
        /// Considera solo il giorno, ignorando l'ora esatta di creazione.
        /// </summary>
        /// <param name="date">Data in cui cercare gli articoli</param>
        /// <returns>Collezione di articoli creati in quella data</returns>
        public async Task<IEnumerable<ArticleReadDto>> GetArticleByDateAsync(DateOnly date)
        {
            // Cerca nel repository e converte i risultati in DTO
            var articles = await _repository.GetArticleByDateAsync(date);
            return articles.Select(a => a.ToArticleReadDto());
        }

        /// <summary>
        /// Cerca tutti gli articoli creati in un periodo di tempo.
        /// Include sia la data di inizio che quella di fine.
        /// </summary>
        /// <param name="startingDate">Data di inizio del periodo (inclusa)</param>
        /// <param name="finishingDate">Data di fine del periodo (inclusa)</param>
        /// <returns>Collezione di articoli creati nel periodo specificato</returns>
        public async Task<IEnumerable<ArticleReadDto>> GetArticleInPeriodAsync(DateOnly startingDate, DateOnly finishingDate)
        {
            // Cerca nel repository e converte i risultati in DTO
            var articles = await _repository.GetArticleInPeriodAsync(startingDate, finishingDate);
            return articles.Select(a => a.ToArticleReadDto());
        }

        /// <summary>
        /// Conta quanti articoli sono stati creati in una data specifica.
        /// Utile per statistiche senza dover recuperare tutti i dati.
        /// </summary>
        /// <param name="date">Data per cui contare gli articoli</param>
        /// <returns>Numero di articoli creati in quella data</returns>
        public async Task<int> GetCountByDateAsync(DateOnly date)
        {
            // Delega il conteggio al repository
            return await _repository.GetCountByDateAsync(date);
        }
    }

}
