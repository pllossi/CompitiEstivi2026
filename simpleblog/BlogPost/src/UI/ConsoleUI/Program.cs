using Application.Dto;
using Application.Interface;
using Application.UseCase;
using Domain.Model.Entities;
using Infrastructure.Repo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ConsoleUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                   
                    services.AddScoped<IBlogRepository>(sp =>
                        new JsonRepository());

                    services.AddScoped<IBlogService, BlogService>();
                })
                .Build();
            await RunApp(host.Services);
        }

        
        static async Task RunApp(IServiceProvider services)
        {
            var blogService = services.GetRequiredService<IBlogService>();

            while (true)
            {
                MostraMenu();

                var scelta = Console.ReadLine()?.Trim();

                Console.WriteLine();

                try
                {
                    switch (scelta)
                    {
                        case "1":
                            await CreaArticolo(blogService);
                            break;
                        case "2":
                            await ListaArticoli(blogService);
                            break;
                        case "3":
                            await VisualizzaArticolo(blogService);
                            break;
                        case "4":
                            await ModificaArticolo(blogService);
                            break;
                        case "5":
                            await EliminaArticolo(blogService);
                            break;
                        case "6":
                            await TestCompleto(blogService);
                            break;
                        case "0":
                            Console.WriteLine("\nArrivederci!");
                            return;
                        default:
                            Console.WriteLine("Attenzione - Scelta non valida!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nERRORE: {ex.Message}");
                    Console.WriteLine($"Tipo: {ex.GetType().Name}");
                }

                Console.WriteLine("\nPremi un tasto per continuare...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void MostraMenu()
        {
            Console.WriteLine("==================================================================");
            Console.WriteLine("                        MENU PRINCIPALE                           ");
            Console.WriteLine("==================================================================");
            Console.WriteLine("  1. Crea Nuovo Articolo ");
            Console.WriteLine("  2. Lista Tutti gli Articoli ");
            Console.WriteLine("  3. Visualizza Articolo Specifico ");
            Console.WriteLine("  4. Modifica Articolo ");
            Console.WriteLine("  5. Elimina Articolo ");
            Console.WriteLine("  6. Test Completo (CRUD) ");
            Console.WriteLine("  0. Esci ");
            Console.WriteLine("==================================================================");
            Console.Write("\nScelta: ");
        }

        static async Task CreaArticolo(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("1. CREA ARTICOLO ");
            Console.WriteLine("============================================================================");

            Console.Write("Titolo: ");
            string title = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Il titolo non può essere vuoto!");
                return;
            }

            Console.Write("Contenuto: ");
            string content = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Il contenuto non può essere vuoto!");
                return;
            }

            Console.WriteLine("\nSalvataggio in corso...");

            try
            {
                await blogService.CreateArticleAsync(new ArticleCreateDto(title, content));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la creazione: {ex.Message}");
                return;
            }

            Console.WriteLine("Articolo pubblicato con successo!");
        }

        static async Task ListaArticoli(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("2. LISTA ARTICOLI ");
            Console.WriteLine("============================================================================");

            Console.WriteLine("Caricamento...\n");

            var articoli = await blogService.GetAllArticlesAsync();
            var listaArticoli = articoli.ToList();

            if (!listaArticoli.Any())
            {
                Console.WriteLine("Nessun articolo trovato nel database.");
                return;
            }

            Console.WriteLine($"Trovati {listaArticoli.Count} articoli:\n");
            Console.WriteLine(new string('─', 80));

            foreach (var articolo in listaArticoli)
            {
                Console.WriteLine($"\nID: {articolo.Id}");
                Console.WriteLine($"Titolo: {articolo.Title}");
                Console.WriteLine($"Data: {articolo.CreatedAt:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"Content: {articolo.Content}");
                Console.WriteLine(new string('─', 80));
            }

            Console.WriteLine($"\nTotale articoli: {listaArticoli.Count}");
        }

        static async Task VisualizzaArticolo(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("3. VISUALIZZA ARTICOLO SPECIFICO ");
            Console.WriteLine("============================================================================");

            Console.Write("Inserisci l'ID dell'articolo: ");
            var id = Console.ReadLine()?.Trim();

            ArticleReadDto? articolo = null;

            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("ID non valido!");
                return;
            }

            Console.WriteLine("\nRicerca in corso...\n");

            try
            {
               articolo = await blogService.GetArticleByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la ricerca: {ex.Message}");
                return;
            }

            if (articolo == null)
            {
                Console.WriteLine($"Articolo con ID '{id}' non trovato!");
                return;
            }


            Console.WriteLine(articolo.Title);
            Console.WriteLine($"ID: {articolo.Id}");
            Console.WriteLine($"Data Creazione: {articolo.CreatedAt:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"\nCONTENUTO:\n{new string('─', 80)}");
            Console.WriteLine(articolo.Content);
            Console.WriteLine(new string('─', 80));
        }

        static async Task ModificaArticolo(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("4. MODIFICA ARTICOLO ");
            Console.WriteLine("============================================================================");

            Console.Write("Inserisci l'ID dell'articolo da modificare: ");

            var id = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("ID non valido!");
                return;
            }
            var articolo = await blogService.GetArticleByIdAsync(id);
            if (articolo == null)
            {
                Console.WriteLine($"Articolo con ID '{id}' non trovato!");
                return;
            }
            Console.WriteLine($"\nTitolo attuale: {articolo.Title}");
            Console.Write("Nuovo titolo (lascia vuoto per mantenere): ");
            string newTitle = Console.ReadLine() ?? "";
            Console.WriteLine($"\nContenuto attuale:\n{new string('─', 80)}\n{articolo.Content}\n{new string('─', 80)}");
            Console.Write("Nuovo contenuto (lascia vuoto per mantenere): ");
            string newContent = Console.ReadLine() ?? "";
            await blogService.UpdateArticleAsync(id, new ArticleCreateDto(
                string.IsNullOrWhiteSpace(newTitle) ? articolo.Title : newTitle,
                string.IsNullOrWhiteSpace(newContent) ? articolo.Content : newContent
            ));
        }

        static async Task EliminaArticolo(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("5.ELIMINA ARTICOLO");
            Console.WriteLine("============================================================================");
            
            Console.Write("Inserisci l'ID dell'articolo da eliminare: ");
            string id = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("ID non valido!");
                return;
            }
            try
            {
                await blogService.DeleteArticleAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'eliminazione: {ex.Message}");
            }
        }

        static async Task TestCompleto(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("6. CRUD");
            Console.WriteLine("============================================================================");
            Console.WriteLine("Creazione articolo di test...");
            var newArticle = new ArticleCreateDto("Articolo di Test", "Questo è un articolo creato.");
            await blogService.CreateArticleAsync(newArticle);
            Console.WriteLine("Recupero dell'articolo appena creato...");
            var articoli = await blogService.GetAllArticlesAsync();
            foreach (var articolo in articoli)
            {
                Console.WriteLine($"\nID: {articolo.Id}");
                Console.WriteLine($"Titolo: {articolo.Title}");
                Console.WriteLine($"Data: {articolo.CreatedAt:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"Content: {articolo.Content}");
                Console.WriteLine(new string('─', 80));
            }
            Console.WriteLine("============================================================================");
            var articoloDaModificare = await blogService.GetArticleByIdAsync(articoli.First().Id);
            var content = articoloDaModificare.Content + "Contenuto aggiornato durante il test completo.";
            await blogService.UpdateArticleAsync(articoloDaModificare.Id, new ArticleCreateDto(articoloDaModificare.Title, content));
            articoli = await blogService.GetAllArticlesAsync();
            foreach (var articolo in articoli)
            {
                Console.WriteLine($"\nID: {articolo.Id}");
                Console.WriteLine($"Titolo: {articolo.Title}");
                Console.WriteLine($"Data: {articolo.CreatedAt:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"Content: {articolo.Content}");
                Console.WriteLine(new string('─', 80));
            }
            Console.WriteLine("============================================================================");
            Console.WriteLine("Eliminazione dell'articolo di test...");
            await blogService.DeleteArticleAsync(articoloDaModificare.Id);
            articoli = await blogService.GetAllArticlesAsync();
            foreach (var articolo in articoli)
            {
                Console.WriteLine($"\nID: {articolo.Id}");
                Console.WriteLine($"Titolo: {articolo.Title}");
                Console.WriteLine($"Data: {articolo.CreatedAt:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"Content: {articolo.Content}");
                Console.WriteLine(new string('─', 80));
            }
            Console.WriteLine("Fine del test completo. Tutte le operazioni CRUD sono state eseguite con successo.");
        }
    }

}
