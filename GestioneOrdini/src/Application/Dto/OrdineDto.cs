using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public record OrdineDto(
        Guid Id,
        string Cliente,
        DateTime Data,
        List<Guid> Prodotti,
        string Stato
     );
}
