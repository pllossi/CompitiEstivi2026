using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using GestioneOrdini.Domain.Entities;

namespace TestDomain
{
    [TestClass]
    public class OrdineTest
    {
        [TestMethod]
        public void CalcoloTotaleOrdine_WithMultipleProducts_ReturnsCorrectTotal()
        {
            var prodotti = new List<ProdottoOrdine>
            {
                new ProdottoOrdine("Prodotto A", 2, 10.50m),
                new ProdottoOrdine("Prodotto B", 1, 25.00m),
                new ProdottoOrdine("Prodotto C", 3, 5.75m)
            };
            var ordine = new global::GestioneOrdini.Domain.Entities.Ordine("Cliente Test", DateTime.Today, prodotti);

            decimal totale = ordine.TotaleOrdine();

            // Expected: (2 * 10.50) + (1 * 25.00) + (3 * 5.75) = 21.00 + 25.00 + 17.25 = 63.25
            Assert.AreEqual(63.25m, totale, 0.001m, "Il calcolo del totale ordine è errato");
        }

        [TestMethod]
        public void CalcoloTotaleOrdine_EmptyProductsList_ReturnsZero()
        {
            var prodotti = new List<ProdottoOrdine>();
            var ordine = new Ordine("Cliente Test", DateTime.Today, prodotti);

            decimal totale = ordine.TotaleOrdine();

            Assert.AreEqual(0m, totale, "Il totale di un ordine senza prodotti dovrebbe essere zero");
        }

        [TestMethod]
        public void CreazioneProdottoOrdine_QuantityGreaterThanZero_Succeeds()
        {
            var prodotto = new ProdottoOrdine("Prodotto Valido", 5, 10.0m);

            Assert.AreEqual("Prodotto Valido", prodotto.Nome);
            Assert.AreEqual(5, prodotto.Quantità);
            Assert.AreEqual(10.0m, prodotto.PrezzoUnitario);
        }

        [TestMethod]
        public void CreazioneProdottoOrdine_QuantityZero_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new ProdottoOrdine("Prodotto Invalido", 0, 10.0m));
        }

        [TestMethod]
        public void CreazioneProdottoOrdine_QuantityNegative_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new ProdottoOrdine("Prodotto Invalido", -1, 10.0m));
        }

        [TestMethod]
        public void CreazioneProdottoOrdine_NegativePrice_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new ProdottoOrdine("Prodotto Invalido", 5, -1.0m));
        }

        [TestMethod]
        public void CreazioneOrdine_NullCliente_ThrowsArgumentNullException()
        {
            var prodotti = new List<ProdottoOrdine>
            {
                new ProdottoOrdine("Prodotto", 1, 10.0m)
            };

            Assert.ThrowsException<ArgumentNullException>(() =>
                new Ordine(null!, DateTime.Today, prodotti));
        }

        [TestMethod]
        public void CreazioneOrdine_NullStato_ThrowsArgumentNullException()
        {
            var prodotti = new List<ProdottoOrdine>
            {
                new ProdottoOrdine("Prodotto", 1, 10.0m)
            };

            Assert.ThrowsException<ArgumentNullException>(() =>
                new Ordine("Cliente", DateTime.Today, prodotti, null!));
        }
    }
}