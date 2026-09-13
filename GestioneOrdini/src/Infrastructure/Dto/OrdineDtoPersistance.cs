using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dto
{
    public record OrdineDtoPersistance
    (
        string Id,
        string Cliente,
        string Data,
        IEnumerable<ProdottoOrdinePersistance> Prodotti,
        string Stato
    );
}
