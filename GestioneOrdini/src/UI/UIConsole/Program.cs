using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases;
using Infrastructure.Repositories;
using GestioneOrdini.Domain.Entities;

namespace GestioneOrdini.UIConsole
{
    class Program
    {
        static readonly IOrdineService _service = new global::Application.UseCases.GestioneOrdini(new global::Infrastructure.Repositories.OrdinePersistanceRepo());

        static async Task Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n=== Gestione Ordini ===");
                Console.WriteLine("1. Aggiungere un ordine");
                Console.WriteLine("2. Visualizzare tutti gli ordini");
                Console.WriteLine("3. Cercare un ordine per Id");
                Console.WriteLine("4. Eliminare un ordine");
                Console.WriteLine("5. Cambiare stato ordine");
                Console.WriteLine("0. Esci");
                Console.Write("Seleziona un'opzione: ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await AggiungiOrdine();
                        break;
                    case "2":
                        await VisualizzaOrdini();
                        break;
                    case "3":
                        await CercaOrdinePerId();
                        break;
                    case "4":
                        await EliminaOrdine();
                        break;
                    case "5":
                        await CambiaStatoOrdine();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Opzione non valida.");
                        break;
                }
            }
        }

        static async Task AggiungiOrdine()
        {
            Console.Write("Inserisci nome cliente: ");
            string? cliente = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(cliente))
            {
                Console.WriteLine("Cliente non valido.");
                return;
            }

            Console.Write("Inserisci data ordine (dd/MM/yyyy) o lascia vuoto per oggi: ");
            string? dataInput = Console.ReadLine();
            DateTime data;
            if (string.IsNullOrWhiteSpace(dataInput))
                data = DateTime.Today;
            else
            {
                if (!DateTime.TryParseExact(dataInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                {
                    Console.WriteLine("Formato data non valido, uso oggi.");
                    data = DateTime.Today;
                }
            }

            List<ProdottoOrdine> prodotti = new();
            while (true)
            {
                Console.Write("Inserisci nome prodotto (o invio per terminare): ");
                string? nome = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(nome))
                    break;

                Console.Write("Inserisci quantità: ");
                if (!int.TryParse(Console.ReadLine(), out int quantità) || quantità <= 0)
                {
                    Console.WriteLine("Quantità non valida.");
                    continue;
                }

                Console.Write("Inserisci prezzo unitario: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal prezzo) || prezzo < 0)
                {
                    Console.WriteLine("Prezzo non valido.");
                    continue;
                }

                try
                {
                    prodotti.Add(new ProdottoOrdine(nome, quantità, prezzo));
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Errore: {ex.Message}");
                }
            }

            if (prodotti.Count == 0)
            {
                Console.WriteLine("Nessun prodotto aggiunto, ordine annullato.");
                return;
            }

            var ordine = new Ordine(cliente, data, prodotti);
            await _service.SaveAsync(ordine);
            Console.WriteLine($"Ordine aggiunto con Id: {ordine.Id}");
        }

        static async Task VisualizzaOrdini()
        {
            var ordini = (await _service.GetAllAsync()).ToList();
            if (!ordini.Any())
            {
                Console.WriteLine("Nessun ordine presente.");
                return;
            }

            foreach (var ordine in ordini)
            {
                Console.WriteLine($"Id: {ordine.Id}");
                Console.WriteLine($"Cliente: {ordine.Cliente}");
                Console.WriteLine($"Data: {ordine.Data:dd/MM/yyyy}");
                Console.WriteLine($"Stato: {ordine.Stato}");
                Console.WriteLine("Prodotti:");
                foreach (var p in ordine.Prodotti)
                {
                    Console.WriteLine($"  - {p.Nome} x{p.Quantità} @ {p.PrezzoUnitario:C}");
                }
                Console.WriteLine($"Totale: {ordine.TotaleOrdine():C}");
                Console.WriteLine(new string('-', 40));
            }
        }

        static async Task CercaOrdinePerId()
        {
            Console.Write("Inserisci Id ordine da cercare: ");
            string? input = Console.ReadLine();
            if (!Guid.TryParse(input, out Guid id))
            {
                Console.WriteLine("Id non valido.");
                return;
            }

            var ordine = await _service.GetByIdAsync(id);
            if (ordine == null)
            {
                Console.WriteLine("Ordine non trovato.");
                return;
            }

            Console.WriteLine($"Id: {ordine.Id}");
            Console.WriteLine($"Cliente: {ordine.Cliente}");
            Console.WriteLine($"Data: {ordine.Data:dd/MM/yyyy}");
            Console.WriteLine($"Stato: {ordine.Stato}");
            Console.WriteLine("Prodotti:");
            foreach (var p in ordine.Prodotti)
            {
                Console.WriteLine($"  - {p.Nome} x{p.Quantità} @ {p.PrezzoUnitario:C}");
            }
            Console.WriteLine($"Totale: {ordine.TotaleOrdine():C}");
        }

        static async Task EliminaOrdine()
        {
            Console.Write("Inserisci Id ordine da eliminare: ");
            string? input = Console.ReadLine();
            if (!Guid.TryParse(input, out Guid id))
            {
                Console.WriteLine("Id non valido.");
                return;
            }

            var ordine = await _service.GetByIdAsync(id);
            if (ordine == null)
            {
                Console.WriteLine("Ordine non trovato.");
                return;
            }

            Console.Write($"Sei sicuro di voler eliminare l'ordine {ordine.Id}? (s/n): ");
            string? conferma = Console.ReadLine();
            if (conferma?.ToLower() == "s")
            {
                await _service.DeleteAsync(id);
                Console.WriteLine("Ordine eliminato.");
            }
            else
            {
                Console.WriteLine("Operazione annullata.");
            }
        }

        static async Task CambiaStatoOrdine()
        {
            Console.Write("Inserisci Id ordine da aggiornare: ");
            string? input = Console.ReadLine();
            if (!Guid.TryParse(input, out Guid id))
            {
                Console.WriteLine("Id non valido.");
                return;
            }

            var ordine = await _service.GetByIdAsync(id);
            if (ordine == null)
            {
                Console.WriteLine("Ordine non trovato.");
                return;
            }

            Console.WriteLine($"Stato corrente: {ordine.Stato}");
            Console.Write("Inserisci nuovo stato: ");
            string? nuovoStato = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nuovoStato))
            {
                Console.WriteLine("Stato non valido.");
                return;
            }

            try
            {
                ordine.CambiaStato(nuovoStato);
                await _service.SaveAsync(ordine);
                Console.WriteLine("Stato aggiornato.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'aggiornamento dello stato: {ex.Message}");
            }
        }
    }
}
