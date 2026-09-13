using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioneOrdini.Domain.Entities;

namespace Application.Interfaces
{
    public interface IOrdineRepository
    {
        Task<IEnumerable<Ordine>> GetAllAsync();
        Task<Ordine?> GetByIdAsync(Guid id);
        Task SaveAsync(Ordine ordine);
        Task DeleteAsync(Guid id);
    }
}
