namespace GestioneOrdini.Domain.Entities
{
    public class ProdottoOrdine
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public int Quantità { get; private set; }
        public decimal PrezzoUnitario { get; private set; }

        public ProdottoOrdine(string nome, int quantità, decimal prezzoUnitario, Guid id = default)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            if (quantità <= 0)
                throw new ArgumentException("La quantità deve essere maggiore di zero", nameof(quantità));
            if (prezzoUnitario < 0)
                throw new ArgumentException("Il prezzo unitario non può essere negativo", nameof(prezzoUnitario));

            Quantità = quantità;
            PrezzoUnitario = prezzoUnitario;
        }
    }
}