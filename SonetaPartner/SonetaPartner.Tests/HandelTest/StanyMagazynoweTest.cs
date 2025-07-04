using NUnit.Framework;
using Soneta.Handel.RelacjeDokumentow.Api;
using Soneta.Handel;
using Soneta.Magazyny;
using SonetaPartner.Tests.Assemblers;
using SonetaPartner.Tests.Extensions.Handel.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soneta.Test;

namespace SonetaPartner.Tests.HandelTest
{
	internal class StanyMagazynoweTest : TestHandel
	{
		[Test]
		public void ObrotyRelacjiKopiowaniaGdyRozneKierunkiMagazynu_Test()
		{
			DodajDrugiMagazyn();

			DefRelacjiAssembler.Nowa()
				.DefinicjaNadrzednego("FV 2")
				.DefinicjaPodrzednego("PZ 2")
				.Utwórz();

			DefRelacjiAssembler.Nowa()
				.DefinicjaNadrzednego("PZ 2")
				.DefinicjaPodrzednego("MM")
				.WyborPozycji(WyborPozycjiDlaRelacji.WybórPozycji)
				.Utwórz();

			Nowy<DokumentHandlowy>()
				.Pozycja(KodyTowarów.Bikini).Ilosc(1.0).Cena(1.0).Dokument()
				.Zatwierdz()
				.Utwórz(c => c.Set(defDokHandlowego: "PW"));

			var wz = Nowy<DokumentHandlowy>()
				.Pozycja(KodyTowarów.Bikini).Ilosc(1.0).Dokument()
				.Zatwierdz()
				.Utwórz(c => c.Set(defDokHandlowego: "WZ 2"));

			var fv = DokumentHandlowyAssembler.NowyWRelacji(wz, "FV 2")
				.Zatwierdz()
				.Utwórz();

			var pz = DokumentHandlowyAssembler.NowyWRelacji(fv, "PZ 2")
				.Zatwierdz()
				.Utwórz();

			var handlers = new HandlerSet
			{
				WybierzPozycjeCallback = d =>
				{
					d.PrzeliczPozycje();
					d.MagazynDo = d.Magazyn.Module.Magazyny.WgSymbol["m2"];
				}
			};

			var mm = DokumentHandlowyAssembler.NowyWRelacji(pz, "MM", handlers)
				.Zatwierdz()
				.Utwórz();

			var obroty = Session.GetMagazyny().Obroty.WgMagazyn.ToArray();
            Assert.That(obroty.Length, Is.EqualTo(2));
            Assert.That(obroty[0].Rozchod.Dokument.Definicja.Symbol, Is.EqualTo("WZ 2"));
            Assert.That(obroty[0].Przychod.Dokument.Definicja.Symbol, Is.EqualTo("PW"));
            Assert.That(obroty[1].Rozchod.Dokument.Definicja.Symbol, Is.EqualTo("MMW"));
            Assert.That(obroty[1].Przychod.Dokument.Definicja.Symbol, Is.EqualTo("PZ 2"));
		}
	}
}
