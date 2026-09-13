using Application.Interface;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUI.ViewModel
{
    /// <summary>
    /// ViewModel principale per la gestione degli articoli del blog.
    /// Utilizza il pattern MVVM con CommunityToolkit.Mvvm per il binding automatico.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        // Service per l'accesso ai dati degli articoli
        private readonly IBlogService _blogService;

        /// <summary>
        /// Costruttore: inizializza il ViewModel e carica gli articoli all'avvio
        /// </summary>
        public MainViewModel(IBlogService blogService)
        {
            _blogService = blogService;
            Articles = new ObservableCollection<ArticleReadDto>();
            // Carica automaticamente tutti gli articoli all'avvio
            LoadArticlesCommand.Execute(null);
        }

        /// <summary>
        /// Collezione osservabile di articoli visualizzati nella lista.
        /// Si aggiorna automaticamente quando viene modificata.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ArticleReadDto> _articles;

        // Articolo attualmente selezionato nella lista
        private ArticleReadDto? _selectedArticle;
        
        /// <summary>
        /// Proprietà per l'articolo selezionato.
        /// Quando viene selezionato un articolo, i suoi dati vengono caricati automaticamente nel form.
        /// </summary>
        public ArticleReadDto? SelectedArticle
        {
            get => _selectedArticle;
            set
            {
                // Se l'articolo cambia e non è null, carica i dati nel form
                if (SetProperty(ref _selectedArticle, value) && value != null)
                {
                    Title = value.Title;
                    Content = value.Content;
                    StatusMessage = $"Article '{value.Title}' loaded for editing";
                }
            }
        }

        /// <summary>
        /// Titolo dell'articolo nel form di modifica/creazione
        /// </summary>
        [ObservableProperty]
        private string _title = string.Empty;

        /// <summary>
        /// Contenuto dell'articolo nel form di modifica/creazione
        /// </summary>
        [ObservableProperty]
        private string _content = string.Empty;

        /// <summary>
        /// Titolo da cercare nei filtri di ricerca
        /// </summary>
        [ObservableProperty]
        private string _searchTitle = string.Empty;

        /// <summary>
        /// Contenuto da cercare nei filtri di ricerca
        /// </summary>
        [ObservableProperty]
        private string _searchContent = string.Empty;

        /// <summary>
        /// Data per la ricerca per data singola (come DateTime per compatibilità con DatePicker)
        /// </summary>
        [ObservableProperty]
        private DateTime? _searchDate;

        /// <summary>
        /// Data di inizio per la ricerca per periodo (come DateTime per compatibilità con DatePicker)
        /// </summary>
        [ObservableProperty]
        private DateTime? _startDate;

        /// <summary>
        /// Data di fine per la ricerca per periodo (come DateTime per compatibilità con DatePicker)
        /// </summary>
        [ObservableProperty]
        private DateTime? _endDate;

        /// <summary>
        /// Messaggio di stato visualizzato nella barra inferiore
        /// </summary>
        [ObservableProperty]
        private string _statusMessage = string.Empty;

        /// <summary>
        /// Comando per caricare tutti gli articoli dal database.
        /// Svuota la lista corrente e la riempie con tutti gli articoli ordinati per data.
        /// </summary>
        [RelayCommand]
        private async Task LoadArticlesAsync()
        {
            try
            {
                // Recupera tutti gli articoli dal service
                var articles = await _blogService.GetAllArticlesAsync();
                Articles.Clear();
                
                // Aggiunge ogni articolo alla collezione osservabile
                foreach (var article in articles)
                {
                    Articles.Add(article);
                }
                StatusMessage = $"Loaded {Articles.Count} articles";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Loading error: {ex.Message}";
            }
        }

        /// <summary>
        /// Comando per creare un nuovo articolo.
        /// Valida i dati, crea l'articolo e ricarica la lista.
        /// </summary>
        [RelayCommand]
        private async Task CreateArticleAsync()
        {
            try
            {
                // Validazione: titolo e contenuto obbligatori
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content))
                {
                    StatusMessage = "Title and content are required";
                    return;
                }

                // Crea il DTO e lo invia al service
                var articleDto = new ArticleCreateDto(Title, Content);
                await _blogService.CreateArticleAsync(articleDto);

                // Pulisce il form dopo la creazione
                Title = string.Empty;
                Content = string.Empty;
                SelectedArticle = null;

                // Ricarica la lista per mostrare il nuovo articolo
                await LoadArticlesAsync();
                StatusMessage = "Article created successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Creation error: {ex.Message}";
            }
        }

        /// <summary>
        /// Comando per aggiornare un articolo esistente.
        /// Richiede che sia selezionato un articolo dalla lista.
        /// </summary>
        [RelayCommand]
        private async Task UpdateArticleAsync()
        {
            try
            {
                // Validazione: deve essere selezionato un articolo
                if (SelectedArticle == null)
                {
                    StatusMessage = "Select an article to update";
                    return;
                }

                // Validazione: titolo e contenuto obbligatori
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content))
                {
                    StatusMessage = "Title and content are required";
                    return;
                }

                // Crea il DTO con i dati modificati e aggiorna l'articolo
                var articleDto = new ArticleCreateDto(Title, Content);
                await _blogService.UpdateArticleAsync(SelectedArticle.Id, articleDto);

                // Pulisce il form dopo l'aggiornamento
                Title = string.Empty;
                Content = string.Empty;
                SelectedArticle = null;

                // Ricarica la lista per mostrare le modifiche
                await LoadArticlesAsync();
                StatusMessage = "Article updated successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Update error: {ex.Message}";
            }
        }

        /// <summary>
        /// Comando per eliminare un articolo.
        /// Mostra una finestra di conferma prima di procedere con l'eliminazione.
        /// </summary>
        [RelayCommand]
        private async Task DeleteArticleAsync()
        {
            try
            {
                // Validazione: deve essere selezionato un articolo
                if (SelectedArticle == null)
                {
                    StatusMessage = "Select an article to delete";
                    return;
                }

                // Mostra un messaggio di conferma all'utente
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the article '{SelectedArticle.Title}'?",
                    "Confirm deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                // Se l'utente conferma, procede con l'eliminazione
                if (result == MessageBoxResult.Yes)
                {
                    await _blogService.DeleteArticleAsync(SelectedArticle.Id);
                    
                    // Pulisce il form
                    Title = string.Empty;
                    Content = string.Empty;
                    SelectedArticle = null;
                    
                    // Ricarica la lista senza l'articolo eliminato
                    await LoadArticlesAsync();
                    StatusMessage = "Article deleted successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Deletion error: {ex.Message}";
            }
        }

        /// <summary>
        /// Comando per deselezionare l'articolo corrente e pulire il form.
        /// Utile per annullare una modifica o iniziare da zero.
        /// </summary>
        [RelayCommand]
        private void Deselect()
        {
            Title = string.Empty;
            Content = string.Empty;
            SelectedArticle = null;
            StatusMessage = "Selection canceled";
        }

        /// <summary>
        /// Comando per cercare articoli per titolo.
        /// La ricerca è case-insensitive e trova articoli che contengono il testo cercato.
        /// Se il campo è vuoto, mostra tutti gli articoli.
        /// </summary>
        [RelayCommand]
        private async Task SearchByTitleAsync()
        {
            try
            {
                // Se il campo di ricerca è vuoto, mostra tutti gli articoli
                if (string.IsNullOrWhiteSpace(SearchTitle))
                {
                    await LoadArticlesAsync();
                    return;
                }

                // Esegue la ricerca e aggiorna la lista con i risultati
                var articles = await _blogService.GetArticleByTitleAsync(SearchTitle);
                Articles.Clear();
                foreach (var article in articles)
                {
                    Articles.Add(article);
                }
                StatusMessage = $"Found {Articles.Count} articles with title '{SearchTitle}'";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Search error: {ex.Message}";
            }
        }

        /// <summary>
        /// Comando per cercare articoli per contenuto.
        /// La ricerca è case-insensitive e trova articoli che contengono il testo cercato.
        /// Se il campo è vuoto, mostra tutti gli articoli.
        /// </summary>
        [RelayCommand]
        private async Task SearchByContentAsync()
        {
            try
            {
                // Se il campo di ricerca è vuoto, mostra tutti gli articoli
                if (string.IsNullOrWhiteSpace(SearchContent))
                {
                    await LoadArticlesAsync();
                    return;
                }

                // Esegue la ricerca e aggiorna la lista con i risultati
                var articles = await _blogService.GetArticleByContentAsync(SearchContent);
                Articles.Clear();
                foreach (var article in articles)
                {
                    Articles.Add(article);
                }
                StatusMessage = $"Found {Articles.Count} articles with content '{SearchContent}'";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Search error: {ex.Message}";
            }
        }

        /// <summary>
        /// Comando per cercare articoli creati in una data specifica.
        /// Trova tutti gli articoli creati nel giorno selezionato.
        /// Se la data non è selezionata, mostra tutti gli articoli.
        /// </summary>
        [RelayCommand]
        private async Task SearchByDateAsync()
        {
            try
            {
                // Se la data non è selezionata, mostra tutti gli articoli
                if (SearchDate == null)
                {
                    await LoadArticlesAsync();
                    return;
                }

                // Converte DateTime in DateOnly per la ricerca
                var dateOnly = DateOnly.FromDateTime(SearchDate.Value);
                var articles = await _blogService.GetArticleByDateAsync(dateOnly);
                Articles.Clear();
                foreach (var article in articles)
                {
                    Articles.Add(article);
                }
                StatusMessage = $"Found {Articles.Count} articles for date {dateOnly}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Search error: {ex.Message}";
            }
        }

        /// <summary>
        /// Comando per cercare articoli in un periodo di tempo.
        /// Trova tutti gli articoli creati tra la data di inizio e la data di fine (incluse).
        /// Se le date non sono selezionate, mostra tutti gli articoli.
        /// </summary>
        [RelayCommand]
        private async Task SearchByPeriodAsync()
        {
            try
            {
                // Se entrambe le date non sono selezionate, mostra tutti gli articoli
                if (StartDate == null && EndDate == null)
                {
                    await LoadArticlesAsync();
                    return;
                }

                // Validazione: entrambe le date devono essere selezionate per la ricerca
                if (StartDate == null || EndDate == null)
                {
                    StatusMessage = "Select both dates";
                    return;
                }

                // Converte DateTime in DateOnly per la ricerca
                var startDateOnly = DateOnly.FromDateTime(StartDate.Value);
                var endDateOnly = DateOnly.FromDateTime(EndDate.Value);

                // Validazione: la data di inizio deve essere prima della data di fine
                if (startDateOnly > endDateOnly)
                {
                    StatusMessage = "Start date must be before end date";
                    return;
                }

                var articles = await _blogService.GetArticleInPeriodAsync(startDateOnly, endDateOnly);
                Articles.Clear();
                foreach (var article in articles)
                {
                    Articles.Add(article);
                }
                StatusMessage = $"Found {Articles.Count} articles in period {startDateOnly} - {endDateOnly}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Search error: {ex.Message}";
            }
        }

        /// <summary>
        /// Comando per contare quanti articoli sono stati creati in una data specifica.
        /// Non modifica la lista, mostra solo il conteggio nella barra di stato.
        /// </summary>
        [RelayCommand]
        private async Task GetCountByDateAsync()
        {
            try
            {
                // Validazione: deve essere selezionata una data
                if (SearchDate == null)
                {
                    StatusMessage = "Select a date";
                    return;
                }

                // Converte DateTime in DateOnly per il conteggio
                var dateOnly = DateOnly.FromDateTime(SearchDate.Value);
                var count = await _blogService.GetCountByDateAsync(dateOnly);
                StatusMessage = $"Number of articles for date {dateOnly}: {count}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Count error: {ex.Message}";
            }
        }
    }
}
