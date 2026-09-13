using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Application.Interfaces;
using GestioneOrdini.Domain.Entities;
using Infrastructure.Dto;

namespace Infrastructure.Repositories
{
    public class OrdineJsonRepository : IOrdineRepository
    {
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GestioneOrdini", "Ordini.json");

        public OrdineJsonRepository()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public async Task<IEnumerable<Ordine>> GetAllAsync()
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var list = System.Text.Json.JsonSerializer.Deserialize<List<OrdineDtoPersistance>>(json) ?? new List<OrdineDtoPersistance>();

            return list.Select(x =>
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
            var list = System.Text.Json.JsonSerializer.Deserialize<List<OrdineDtoPersistance>>(json) ?? new List<OrdineDtoPersistance>();

            var existing = list.FirstOrDefault(x => x.Id == ordine.Id.ToString());
            var dto = new OrdineDtoPersistance(
                ordine.Id.ToString(),
                ordine.Cliente,
                ordine.Data.ToString("yyyy-MM-dd"),
                ordine.Prodotti.Select(p => new ProdottoOrdinePersistance(p.Id.ToString(), p.Nome, p.Quantità, p.PrezzoUnitario)).ToList(),
                ordine.Stato
            );

            if (existing != null)
            {
                list.Remove(existing);
            }
            list.Add(dto);

            var newJson = System.Text.Json.JsonSerializer.Serialize(list, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, newJson);
        }

        public async Task DeleteAsync(Guid id)
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var list = System.Text.Json.JsonSerializer.Deserialize<List<OrdineDtoPersistance>>(json) ?? new List<OrdineDtoPersistance>();
            var toRemove = list.FirstOrDefault(x => x.Id == id.ToString());
            if (toRemove == null)
                throw new Exception($"Ordine with id {id} not found.");
            list.Remove(toRemove);
            var newJson = System.Text.Json.JsonSerializer.Serialize(list, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, newJson);
        }
    }
}
