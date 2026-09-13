using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioneOrdini.Domain.Entities;
using Infrastructure.Dto;
using Application.Dto;

namespace Infrastructure.Mapper
{
    public static class ProdottoOrdinePersistanceMapper
    {
        public static ProdottoOrdinePersistance ToDtoPersistance(this global::Application.Dto.ProdottoOrdineDto prodottoOrdine)
        {
            return new ProdottoOrdinePersistance(
                prodottoOrdine.Id.ToString(),
                prodottoOrdine.Nome,
                prodottoOrdine.Quantità,
                prodottoOrdine.Prezzo
            );
        }
        public static global::Application.Dto.ProdottoOrdineDto ToEntity(this ProdottoOrdinePersistance prodottoOrdinePersistance)
        {
            return new ProdottoOrdineDto(
                Guid.Parse(prodottoOrdinePersistance.Id),
                prodottoOrdinePersistance.Nome,
                prodottoOrdinePersistance.PrezzoUnitario,
                prodottoOrdinePersistance.Quantità
            );
        }

    }
}