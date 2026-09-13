using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioneOrdini.Domain.Entities;
using Application.Dto;

namespace Application.Mappers
{
    public static class ProdottoOrdineMapper
    {
        public static ProdottoOrdineDto ToDto(this ProdottoOrdine prodotto)
        {
            return new ProdottoOrdineDto(
                prodotto.Id,
                prodotto.Nome,
                prodotto.PrezzoUnitario,
                prodotto.Quantità
            );
        }
        public static ProdottoOrdine ToEntity(this ProdottoOrdineDto prodottoDto)
        {
            return new ProdottoOrdine(
                prodottoDto.Nome,
                prodottoDto.Quantità,
                prodottoDto.Prezzo
            );
        }
    }
}
