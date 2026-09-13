using System;
using System.Collections.Generic;

namespace GestioneOrdini.Domain.Entities
{
    public class Ordine
    {
        public Guid Id { get; private set; }
        public string Cliente { get; private set; }
        public DateTime Data { get; private set; }
        public List<ProdottoOrdine> Prodotti { get; private set; }
        public string Stato { get; private set; }

        public Ordine( string cliente, DateTime data, List<ProdottoOrdine> prodotti, string stato = "In attesa", Guid id = default)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            Data = data;
            Prodotti = prodotti ?? new List<ProdottoOrdine>();
            Stato = stato ?? throw new ArgumentNullException(nameof(stato));
        }

        public decimal TotaleOrdine()
        {
            decimal totale = 0;
            foreach (var prodotto in Prodotti)
            {
                totale += prodotto.Quantità * prodotto.PrezzoUnitario;
            }
            return totale;
        }

        public void CambiaStato(string nuovoStato)
        {
            if (string.IsNullOrWhiteSpace(nuovoStato))
                throw new ArgumentNullException(nameof(nuovoStato));

            Stato = nuovoStato;
        }
    }
}