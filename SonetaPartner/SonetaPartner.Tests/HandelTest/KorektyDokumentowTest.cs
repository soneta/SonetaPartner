using NUnit.Framework;
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
using SonetaPartner.Tests.Extensions.Magazyn.Engine;
using static Soneta.Handel.ParametryWydrukuDokumentu;

namespace SonetaPartner.Tests.HandelTest
{
	internal class KorektyDokumentowTest : TestHandel
	{
		[Test]
		public void CofnieDoBuforaKZK_Test()
		{
			DodajDrugiMagazyn("A");
			DodajDrugiMagazyn("B");

			var pz = Nowy<DokumentHandlowy>()
				.Pozycja(KodyTowarów.Bikini).Ilosc(10.0).Cena(10.0).Dokument()
				.Zatwierdz();
			pz.Utwórz(c => c.Set(defDokHandlowego: "PZ 2"));

			Nowy<DokumentHandlowy>()
				.MagazynDo("A")
				.Pozycja(KodyTowarów.Bikini).Ilosc(7.0).Dokument()
				.Zatwierdz()
				.Utwórz(c => c.Set(defDokHandlowego: "MM"));

			Nowy<DokumentHandlowy>()
				.MagazynDo("B")
				.Pozycja(KodyTowarów.Bikini).Ilosc(1.0).Dokument()
				.Zatwierdz()
				.Utwórz(c => c.Set(defDokHandlowego: "MM", magazyn: "A"));

			var wz = Nowy<DokumentHandlowy>()
				.Pozycja(KodyTowarów.Bikini).Ilosc(1.0).Dokument()
				.Zatwierdz();
			wz.Utwórz(c => c.Set(defDokHandlowego: "WZ 2", magazyn: "B"));

			var kpz = DokumentHandlowyAssembler.Korekta(pz.Build())
				.Pozycja(1).Cena(9.0).Dokument()
				.Zatwierdz();
			kpz.Utwórz();

			var obroty = wz.Build().Podrzędne[TypRelacjiHandlowej.KorektaPWZ].Obroty.ToArray<Obrot>();
			Assert.AreEqual(2, obroty.Length);
			Assert.AreEqual("KMMP", obroty[1].Przychod.Dokument.Definicja.Symbol);
			Assert.AreEqual(2, obroty[1].Przychod.Dokument.Numer.Numer);
			Assert.AreEqual("KWPZ", obroty[1].Rozchod.Dokument.Definicja.Symbol);
			Assert.AreEqual(KorektaObrotu.Brak, obroty[1].Korekta);

			kpz.Bufor().Utwórz();

			var obrot = wz.Build().ObrotyWszystkie[0] as Obrot;
			Assert.AreEqual("MMP", obrot.Przychod.Dokument.Definicja.Symbol);
			Assert.AreEqual(2, obrot.Przychod.Dokument.Numer.Numer);
			Assert.AreEqual("WZ 2", obrot.Rozchod.Dokument.Definicja.Symbol);
			Assert.AreEqual(0, Session.GetHandel().DokHandlowe.WgDefinicja[Get<DefDokHandlowego>("KWPZ").Build()].Count);
		}
	}
}
