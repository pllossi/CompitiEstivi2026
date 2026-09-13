using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioneOrdini.Domain.Entities;

namespace Application.UseCases
{
    public class GestioneOrdini : IOrdineService
    {
        private readonly IOrdineRepository _ordineRepository;
        public GestioneOrdini(IOrdineRepository ordineRepository)
        {
            _ordineRepository = ordineRepository;
        }
        public async Task<IEnumerable<Ordine>> GetAllAsync()
        {
            return await _ordineRepository.GetAllAsync();
        }

        public async Task<Ordine?> GetByIdAsync(Guid id)
        {
            return await _ordineRepository.GetByIdAsync(id);
        }

        public async Task SaveAsync(Ordine ordine)
        {
            await _ordineRepository.SaveAsync(ordine);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _ordineRepository.DeleteAsync(id);
        }
    }
}
