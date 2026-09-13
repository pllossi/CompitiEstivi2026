using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioneOrdini.Domain.Entities;
using Infrastructure.Dto;

namespace Infrastructure.Mapper
{
    public static class OrdineDtoPersistanceMapper
    {
        public static OrdineDtoPersistance ToDtoPersistance(this Ordine ordine)
        {
            return new OrdineDtoPersistance(
                ordine.Id.ToString(),
                ordine.Cliente,
                ordine.Data.ToString("yyyy-MM-dd"),
                ordine.Prodotti.Select(p => new ProdottoOrdinePersistance(p.Id.ToString(), p.Nome, p.Quantità, p.PrezzoUnitario)).ToList(),
                ordine.Stato
            );
        }
        public static Ordine ToEntity(this OrdineDtoPersistance ordineDtoPersistance, List<ProdottoOrdine> prodotti)
        {
            return new Ordine(
                ordineDtoPersistance.Cliente,
                DateTime.Parse(ordineDtoPersistance.Data),
                prodotti,
                ordineDtoPersistance.Stato,
                Guid.Parse(ordineDtoPersistance.Id)
            );
        }
    }
}
