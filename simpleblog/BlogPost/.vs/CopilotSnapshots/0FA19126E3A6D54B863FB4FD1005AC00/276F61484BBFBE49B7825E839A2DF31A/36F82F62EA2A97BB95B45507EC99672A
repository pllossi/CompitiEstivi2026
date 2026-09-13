using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Domain.Model.Entities;
using Infrastructure.Dto;
using Infrastructure.Mapper;
using Application.Interface;

namespace Infrastructure.Repo
{
    /// <summary>
    /// Repository per la gestione degli articoli del blog su Firebase Realtime Database.
    /// Implementa tutte le operazioni CRUD e di ricerca utilizzando il database NoSQL di Firebase.
    /// I dati vengono salvati come JSON nel nodo "articles" del database.
    /// </summary>
    public class BlogPostFirebaseRepo : IBlogRepository
    {
        // Client per la comunicazione con Firebase
        private readonly FirebaseClient _client;
        
        // Nome del nodo principale nel database Firebase dove sono salvati gli articoli
        private const string ArticlesNode = "articles";

        /// <summary>
        /// Costruttore: inizializza la connessione a Firebase.
        /// </summary>
        /// <param name="firebaseUrl">URL del database Firebase (es: https://myapp.firebaseio.com)</param>
        /// <exception cref="ArgumentException">Lanciata se l'URL non è valido</exception>
        public BlogPostFirebaseRepo(string firebaseUrl)
        {
            // Validazione dell'URL
            if (string.IsNullOrWhiteSpace(firebaseUrl))
                throw new ArgumentException("Invalid Firebase URL", nameof(firebaseUrl));

            // Crea il client Firebase per le operazioni sul database
            _client = new FirebaseClient(firebaseUrl);
        }

        /// <summary>
        /// Salva un nuovo articolo nel database Firebase.
        /// Struttura: articles/{articleId}/{dati}
        /// </summary>
        /// <param name="article">Entità articolo da salvare</param>
        public async Task SaveAsync(BlogPost article)
        {
            // Verifica che l'articolo non sia null
            ArgumentNullException.ThrowIfNull(article);

            // Converte l'entità di dominio in DTO per la persistenza
            // (DateTime viene convertito in Unix timestamp per Firebase)
            var dto = article.ToPersistenceDto();

            // Salva nel database Firebase usando PutAsync (crea o sovrascrive)
            // Path: articles/{articleId}
            await _client
                .Child(ArticlesNode)
                .Child(article.Id.ToString())
                .PutAsync(dto)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Recupera un singolo articolo tramite il suo ID.
        /// </summary>
        /// <param name="id">ID univoco dell'articolo da recuperare</param>
        /// <returns>Entità articolo, oppure null se non trovato</returns>
        public async Task<BlogPost?> GetByIdAsync(string id)
        {
            // Validazione dell'ID
            if (string.IsNullOrWhiteSpace(id))
                return null;

            // Legge i dati da Firebase per lo specifico ID
            var dto = await _client
                .Child(ArticlesNode)
                .Child(id)
                .OnceSingleAsync<BlogPostPersistenceDto>()
                .ConfigureAwait(false);

            // Se non trovato, ritorna null
            // Altrimenti converte il DTO in entità di dominio
            return dto == null ? null : dto.ToEntity();
        }

        /// <summary>
        /// Recupera tutti gli articoli dal database.
        /// Gli articoli sono ordinati per data di creazione (più recenti prima).
        /// </summary>
        /// <returns>Collezione di tutte le entità articolo</returns>
        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            // Legge tutti i dati dal nodo articles
            // OnceAsync recupera una snapshot una tantum (non in tempo reale)
            var dtos = await _client
                .Child(ArticlesNode)
                .OnceAsync<BlogPostPersistenceDto>()
                .ConfigureAwait(false);

            // Converte i DTO in entità, filtra i null e ordina per data
            return dtos
                .Select(m => m.Object?.ToEntity())  // Converte ogni DTO in entità
                .Where(e => e != null)               // Rimuove eventuali null
                .Cast<BlogPost>()                    // Cast esplicito a BlogPost
                .OrderByDescending(a => a.CreatedAt) // Ordina dal più recente al più vecchio
                .ToList();
        }

        /// <summary>
        /// Aggiorna un articolo esistente nel database.
        /// Sovrascrive completamente i dati precedenti.
        /// </summary>
        /// <param name="article">Entità articolo con i dati aggiornati</param>
        public async Task UpdateAsync(BlogPost article)
        {
            // Verifica che l'articolo non sia null
            ArgumentNullException.ThrowIfNull(article);

            // Converte l'entità in DTO per la persistenza
            var dto = article.ToPersistenceDto();

            // Sovrascrive i dati nel database (stesso metodo di SaveAsync)
            await _client
                .Child(ArticlesNode)
                .Child(article.Id.ToString())
                .PutAsync(dto)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Elimina definitivamente un articolo dal database.
        /// </summary>
        /// <param name="id">ID dell'articolo da eliminare</param>
        /// <exception cref="ArgumentException">Lanciata se l'ID non è valido</exception>
        public async Task DeleteAsync(string id)
        {
            // Validazione dell'ID
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid ID", nameof(id));

            // Elimina il nodo dal database Firebase
            await _client
                .Child(ArticlesNode)
                .Child(id)
                .DeleteAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Cerca articoli il cui titolo contiene il testo specificato.
        /// La ricerca è case-insensitive e funziona su dati già caricati in memoria
        /// (Firebase Realtime Database non supporta ricerche full-text native).
        /// </summary>
        /// <param name="title">Testo da cercare nel titolo</param>
        /// <returns>Collezione di articoli che corrispondono alla ricerca</returns>
        public async Task<IEnumerable<BlogPost>> GetArticleByTitleAsync(string title)
        {
            // Se il titolo è vuoto, ritorna una collezione vuota
            if (string.IsNullOrWhiteSpace(title))
                return Array.Empty<BlogPost>();

            // Carica tutti gli articoli in memoria
            var all = await GetAllAsync().ConfigureAwait(false);
            
            // Filtra in memoria usando LINQ
            // Contains è case-insensitive grazie a StringComparison.OrdinalIgnoreCase
            return all
                .Where(a => a.Title?.Contains(title, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }

        /// <summary>
        /// Cerca articoli il cui contenuto contiene il testo specificato.
        /// La ricerca è case-insensitive e funziona su dati già caricati in memoria.
        /// </summary>
        /// <param name="content">Testo da cercare nel contenuto</param>
        /// <returns>Collezione di articoli che corrispondono alla ricerca</returns>
        public async Task<IEnumerable<BlogPost>> GetArticleByContentAsync(string content)
        {
            // Se il contenuto è vuoto, ritorna una collezione vuota
            if (string.IsNullOrWhiteSpace(content))
                return Array.Empty<BlogPost>();

            // Carica tutti gli articoli e filtra in memoria
            var all = await GetAllAsync().ConfigureAwait(false);
            return all
                .Where(a => a.Content?.Contains(content, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }

        /// <summary>
        /// Cerca tutti gli articoli creati in una data specifica.
        /// Considera solo il giorno, ignorando l'ora esatta.
        /// </summary>
        /// <param name="date">Data in cui cercare gli articoli</param>
        /// <returns>Collezione di articoli creati in quella data</returns>
        public async Task<IEnumerable<BlogPost>> GetArticleByDateAsync(DateOnly date)
        {
            // Carica tutti gli articoli
            var all = await GetAllAsync().ConfigureAwait(false);
            
            // Filtra comparando solo la parte data (ignora l'ora)
            return all
                .Where(a => DateOnly.FromDateTime(a.CreatedAt) == date)
                .ToList();
        }

        /// <summary>
        /// Cerca tutti gli articoli creati in un periodo di tempo.
        /// Include sia la data di inizio che quella di fine.
        /// </summary>
        /// <param name="startingDate">Data di inizio del periodo (inclusa)</param>
        /// <param name="finishingDate">Data di fine del periodo (inclusa)</param>
        /// <returns>Collezione di articoli creati nel periodo specificato</returns>
        /// <exception cref="ArgumentException">Lanciata se la data di fine è prima di quella di inizio</exception>
        public async Task<IEnumerable<BlogPost>> GetArticleInPeriodAsync(DateOnly startingDate, DateOnly finishingDate)
        {
            // Validazione: la data di fine deve essere >= data di inizio
            if (finishingDate < startingDate)
                throw new ArgumentException("finishingDate must be >= startingDate");

            // Carica tutti gli articoli
            var all = await GetAllAsync().ConfigureAwait(false);
            
            // Filtra per range di date
            return all
                .Where(a =>
                {
                    // Converte DateTime in DateOnly per confronto
                    var d = DateOnly.FromDateTime(a.CreatedAt);
                    // Verifica che la data sia nel range (inclusi gli estremi)
                    return d >= startingDate && d <= finishingDate;
                })
                .ToList();
        }

        /// <summary>
        /// Conta quanti articoli sono stati creati in una data specifica.
        /// Più efficiente che recuperare tutti gli articoli se serve solo il conteggio.
        /// </summary>
        /// <param name="date">Data per cui contare gli articoli</param>
        /// <returns>Numero di articoli creati in quella data</returns>
        public async Task<int> GetCountByDateAsync(DateOnly date)
        {
            // Riutilizza il metodo di ricerca per data e conta i risultati
            var matches = await GetArticleByDateAsync(date).ConfigureAwait(false);
            return matches.Count();
        }
    }
}
