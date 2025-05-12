using NUnit.Framework;
using Soneta.Business;
using Soneta.Handel.RelacjeDokumentow.Api;
using Soneta.Handel;
using Soneta.Magazyny;
using SonetaPartner.Tests.Assemblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonetaPartner.Tests.Extensions.Handel.Engine;
using Soneta.Test;

namespace SonetaPartner.Tests.HandelTest
{
	internal class ZamowieniaIOfertyTest : TestHandel
	{
		[Test]
		public void PoprawneZasobyDlaZOS_Test()
		{
			Get<DefDokHandlowego>("ZO")
				.WskazaniePartii()
				.Utwórz();

			Get<DefDokHandlowego>("WZ")
				.MomentMagazynu(MomentMagazynu.WBuforze)
				.Utwórz();

			Get<DefDokHandlowego>("ZO")
				.DefRelacji("Zamówienie do dostawcy")
				.Agregowanie(SposobLaczeniaPozycji.TylkoTowar)
				.Utwórz();

			var pz = Nowy<DokumentHandlowy>()
				.Kontrahent("Zefir")
				.Pozycja(KodyTowarów.Bikini).Ilosc(100.0).Cena(10.0).Dokument()
				.Zatwierdz()
				.Utwórz(x => x.Set(defDokHandlowego: "PZ 2"));

			var zo1Builder = Nowy<DokumentHandlowy>()
				.Kontrahent("Drynda")
				.Pozycja(KodyTowarów.Bikini).Ilosc(30.0).Dokument()
				.Zatwierdz();
			var zo1 = zo1Builder.Utwórz(x => x.Set(defDokHandlowego: "ZO"));

			var zo2Builder = Nowy<DokumentHandlowy>()
				.Kontrahent("Drynda")
				.Pozycja(KodyTowarów.Bikini).Ilosc(30.0).Dokument()
				.Zatwierdz();
			var zo2 = zo2Builder.Utwórz(x => x.Set(defDokHandlowego: "ZO"));

			var zd = DokumentHandlowyAssembler.UtworzZbiorczy(new[] { zo2Builder, zo1Builder }, "ZD")
				.Pozycja(1).Ilosc(45d).Dokument()
				.Zatwierdz()
				.Utwórz();

			Assert.AreEqual(1, zd.Pozycje.Count());
			Assert.AreEqual(45d, zd.PozycjaWgIdent(1).Ilosc.Value);

			zo1 = zo1Builder.Build();
			zo2 = zo2Builder.Build();

			Assert.AreEqual(1, zo1.Zasoby.Count);
			Assert.AreEqual(1, zo2.Zasoby.Count);

			var zasob = zo1.Zasoby.GetFirst() as Zasob;
			Assert.AreEqual(30d, zasob.Ilosc.Value);
			Assert.AreEqual(zd.NumerPelnyZapisany, zasob.Nadrzedny.Partia.Dokument.NumerPelnyZapisany);

			zasob = zo2.Zasoby.GetFirst() as Zasob;
			Assert.AreEqual(30d, zasob.Ilosc.Value);
			Assert.IsNull(zasob.Nadrzedny);

			var zk = DokumentHandlowyAssembler.NowyWRelacji(zd, "ZK", new HandlerSet()
			{
				WybierzPozycjeCallback = d =>
				{
					d.ZaznaczWszystko();
				}
			})
			.Kontrahent("Zefir")
			.Pozycja(1).Ilosc(45d).Cena(10d).Dokument()
			.Zatwierdz()
			.Utwórz();

			var zasobyZO = Session.GetMagazyny().Zasoby.Cast<Zasob>().Where(x => x.Partia.Dokument.Definicja.Symbol == "ZO").ToArray();

			Assert.AreEqual(2, zasobyZO.Count());

			zasob = zasobyZO.Where(x => x.Partia.Dokument.NumerPelnyZapisany == zo1.NumerPelnyZapisany).FirstOrDefault();
			Assert.IsNotNull(zasob);
			Assert.AreEqual(30d, zasob.Ilosc.Value);
			Assert.AreEqual(zk.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].NumerPelnyZapisany, zasob.Nadrzedny.Partia.Dokument.NumerPelnyZapisany);

			zasob = zasobyZO.Where(x => x.Partia.Dokument.NumerPelnyZapisany == zo2.NumerPelnyZapisany).FirstOrDefault();
			Assert.IsNotNull(zasob);
			Assert.AreEqual(30d, zasob.Ilosc.Value);
			Assert.IsNull(zasob.Nadrzedny);

			DokumentHandlowyAssembler.NoweWRelacji(new[] { zo1Builder, zo2Builder }, "FV", new HandlerSet()
			{
				WybierzPozycjeCallback = d =>
				{
					d.ZaznaczWszystko();
				}
			})
			.Utwórz();

			var handel = Session.GetHandel();
			var fv1Builder = GetBuilderOf<DokumentHandlowy>(handel.DokHandlowe.WgDefinicja[handel.DefDokHandlowych.WgSymbolu["FV"]].ElementAt(0).Guid).Zatwierdz();
			var fv1 = fv1Builder.Utwórz();
			var fv2Builder = GetBuilderOf<DokumentHandlowy>(handel.DokHandlowe.WgDefinicja[handel.DefDokHandlowych.WgSymbolu["FV"]].ElementAt(1).Guid).Zatwierdz();
			var fv2 = fv2Builder.Utwórz();

			Assert.AreEqual(30d, fv1.PozycjaWgIdent(1).Ilosc.Value);
			Assert.AreEqual(30d, fv2.PozycjaWgIdent(1).Ilosc.Value);

			Assert.AreEqual(1, fv1.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].PozycjaWgIdent(1).Obroty.Count);
			var obrot = fv1.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].PozycjaWgIdent(1).Obroty.GetFirst() as Obrot;
			Assert.AreEqual(30d, obrot.Ilosc.Value);
			Assert.AreEqual(zk.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].NumerPelnyZapisany, obrot.Przychod.Dokument.NumerPelnyZapisany);

			Assert.AreEqual(1, fv2.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].PozycjaWgIdent(1).Obroty.Count);
			obrot = fv2.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].PozycjaWgIdent(1).Obroty.GetFirst() as Obrot;
			Assert.AreEqual(30d, obrot.Ilosc.Value);
			Assert.AreEqual(pz.NumerPelnyZapisany, obrot.Przychod.Dokument.NumerPelnyZapisany);

			fv2Builder
				.Bufor()
				.Usuń()
				.Utwórz();

			fv1Builder
				.Bufor()
				.Usuń()
				.Utwórz();

			zasobyZO = Session.GetMagazyny().Zasoby.Cast<Zasob>().Where(x => x.Partia.Dokument.Definicja.Symbol == "ZO").ToArray();

			Assert.AreEqual(2, zasobyZO.Count());

			zasob = zasobyZO.Where(x => x.Partia.Dokument.NumerPelnyZapisany == zo1.NumerPelnyZapisany).FirstOrDefault();
			Assert.IsNotNull(zasob);
			Assert.AreEqual(30d, zasob.Ilosc.Value);
			Assert.AreEqual(zk.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].NumerPelnyZapisany, zasob.Nadrzedny.Partia.Dokument.NumerPelnyZapisany);

			zasob = zasobyZO.Where(x => x.Partia.Dokument.NumerPelnyZapisany == zo2.NumerPelnyZapisany).FirstOrDefault();
			Assert.IsNotNull(zasob);
			Assert.AreEqual(30d, zasob.Ilosc.Value);
			Assert.IsNull(zasob.Nadrzedny);
		}
	}
}
