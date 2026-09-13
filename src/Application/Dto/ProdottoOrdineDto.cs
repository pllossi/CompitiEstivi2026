using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public record ProdottoOrdineDto(
        Guid Id,
        string Nome,
        decimal Prezzo,
        int Quantità
    );
}
