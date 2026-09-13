using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioneOrdini.Domain.Entities;
using Infrastructure.Dto;
using Application.Dto;

namespace Infrastructure.Repositories
{
    public class ProdottoOrdinePersistanceRepo
    {
        private string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GestioneOrdini", "ProdottiOrdine.json");
        public ProdottoOrdinePersistanceRepo()
        {
            //ensure the file exists
            if (!File.Exists(_filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                File.WriteAllText(_filePath, "[]");
            }
        }
        public async Task<List<ProdottoOrdinePersistance>> GetAllAsync()
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var listOut = System.Text.Json.JsonSerializer.Deserialize<List<ProdottoOrdinePersistance>>(json);
            return listOut;
        }
        public async Task SaveAllAsync(List<ProdottoOrdinePersistance> prodottiOrdine)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(prodottiOrdine ?? new List<ProdottoOrdinePersistance>(), new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
