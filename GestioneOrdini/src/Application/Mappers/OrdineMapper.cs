using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioneOrdini.Domain.Entities;
using Application.Dto;

namespace Application.Mappers
{
    public static class OrdineMapper
    {
        public static OrdineDto ToDto(this Ordine ordine)
        {
            return new OrdineDto(
                ordine.Id,
                ordine.Cliente,
                ordine.Data,
                ordine.Prodotti.Select(p => p.Id).ToList(),
                ordine.Stato
            );
        }
        public static Ordine ToEntity(this OrdineDto ordineDto, List<ProdottoOrdine> prodotti)
        {
            return new Ordine(
                ordineDto.Cliente,
                ordineDto.Data,
                prodotti,
                ordineDto.Stato
            );
        }
    }
}
