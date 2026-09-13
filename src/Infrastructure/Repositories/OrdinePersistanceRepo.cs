using Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Mapper;
using Application.Dto;
using Application.Interfaces;
using GestioneOrdini.Domain.Entities;

namespace Infrastructure.Repositories
{
    public class OrdinePersistanceRepo : IOrdineRepository
    {
        private readonly List<OrdineDtoPersistance> _ordini = new List<OrdineDtoPersistance>();
        private string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GestioneOrdini", "Ordini.json");
        public OrdinePersistanceRepo()
        {
            //ensure the file exists
            if (!File.Exists(_filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                File.WriteAllText(_filePath, "[]");
            }
        }
        public async Task<IEnumerable<Ordine>> GetAllAsync()
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var listOut = System.Text.Json.JsonSerializer.Deserialize<List<OrdineDtoPersistance>>(json) ?? new List<OrdineDtoPersistance>();
            _ordini.Clear();
            _ordini.AddRange(listOut);

            return listOut.Select(x =>
            {
                var prodotti = x.Prodotti?.Select(p => new ProdottoOrdine(p.Nome, p.Quantità, p.PrezzoUnitario, Guid.Parse(p.Id))).ToList() ?? new List<ProdottoOrdine>();
                return new Ordine(x.Cliente, DateTime.Parse(x.Data), prodotti, x.Stato, Guid.Parse(x.Id));
            }).ToList();
        }

        public async Task<Ordine?> GetByIdAsync(Guid id)
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault(o => o.Id == id);
        }

        public async Task SaveAsync(Ordine ordine)
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var listOut = System.Text.Json.JsonSerializer.Deserialize<List<OrdineDtoPersistance>>(json) ?? new List<OrdineDtoPersistance>();

            var newOrdine = ordine.ToDtoPersistance();
            var existing = listOut.FirstOrDefault(x => x.Id == newOrdine.Id);
            if (existing != null)
                listOut.Remove(existing);

            listOut.Add(newOrdine);

            var newJson = System.Text.Json.JsonSerializer.Serialize(listOut, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, newJson);
        }

        public async Task DeleteAsync(Guid id)
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var listOut = System.Text.Json.JsonSerializer.Deserialize<List<OrdineDtoPersistance>>(json);
            var ordineToRemove = listOut.FirstOrDefault(x => x.Id == id.ToString());
            if (ordineToRemove != null)
            {
                listOut.Remove(ordineToRemove);
                var newJson = System.Text.Json.JsonSerializer.Serialize(listOut, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, newJson);
            } else
            {
                throw new Exception($"Ordine with id {id} not found.");
            }
        }

    }
}
