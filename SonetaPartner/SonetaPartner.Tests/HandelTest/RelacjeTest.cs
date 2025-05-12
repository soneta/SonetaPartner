using NUnit.Framework;
using Soneta.Handel.RelacjeDokumentow.Api;
using Soneta.Handel;
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
	internal class RelacjeTest : TestHandel
	{
		[Test]
		public void DolaczanieNadrzednegoDoCzesciowoRozliczonegoPoZmianieNazwyRelacji()
		{
			DefRelacjiAssembler.NowaZaliczka()
				.DefinicjaNadrzednego("FZAL")
				.DefinicjaPodrzednego("FV 2")
				.Utwórz();

			DefRelacjiAssembler.Nowa()
				.DefinicjaNadrzednego("FZAL")
				.DefinicjaPodrzednego("FV 2")
				.WieleNadrzednych(false)
				.WielePodrzednych(false)
				.ZPodrzednegoDolaczDokument()
				.Utwórz();

			Get<DefDokHandlowego>("WZ 2")
				.DefRelacji("Handlowy")
				.Blokada(true)
				.Utwórz();

			DefRelacjiAssembler.Nowa()
				.DefinicjaNadrzednego("WZ 2")
				.DefinicjaPodrzednego("FV 2")
				.Enqueue(d => d.ZNadrzednego.Nazwa = "Test")
				.Utwórz();

			Nowy<DokumentHandlowy>()
				.Pozycja(KodyTowarów.Bikini).Ilosc(1.0).Cena(10.0).Dokument()
				.Zatwierdz()
				.Utwórz(c => c.Set(defDokHandlowego: "PW"));

			Nowy<DokumentHandlowy>()
				.Pozycja(KodyTowarów.Montaz).Ilosc(1.0).Cena(100.0).Dokument()
				.Zatwierdz()
				.Utwórz(c => c.Set(defDokHandlowego: "FZAL"));

			var wz = Nowy<DokumentHandlowy>()
				.Pozycja(KodyTowarów.Bikini).Ilosc(1.0).Cena(100.0).Dokument()
				.Zatwierdz()
				.Utwórz(c => c.Set(defDokHandlowego: "WZ 2"));

			var fvBuilder = DokumentHandlowyAssembler.NowyWRelacji(wz, "FV 2");
			var fv = fvBuilder.Utwórz();

			Assert.AreEqual(1, fv.Pozycje.Count);

			var handlers = new HandlerSet
			{
				WybierzDokumentyCallback = p =>
				{
					Assert.AreEqual(1, p.Dokumenty.Count);
					p.DokumentyWybrane = p.Dokumenty.Cast<DokumentHandlowy>().ToArray();
				}
			};

			fv = fvBuilder.DolaczNadrzedny("Faktura zaliczkowa", handlers)
				.Zatwierdz()
				.Utwórz();

			Assert.AreEqual(2, fv.Pozycje.Count);
			Assert.AreEqual(1, fv.ZaliczkiRelacje.Count);
		}
	}
}
