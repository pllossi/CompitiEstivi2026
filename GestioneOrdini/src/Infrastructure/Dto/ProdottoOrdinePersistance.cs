using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dto
{
    public record ProdottoOrdinePersistance
    (
        string Id,
        string Nome,
        int Quantità,
        decimal PrezzoUnitario
    );
}
