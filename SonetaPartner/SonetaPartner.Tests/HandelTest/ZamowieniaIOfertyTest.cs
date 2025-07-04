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

            Assert.That(zd.Pozycje.Count(), Is.EqualTo(1));
            Assert.That(zd.PozycjaWgIdent(1).Ilosc.Value, Is.EqualTo(45d));

			zo1 = zo1Builder.Build();
			zo2 = zo2Builder.Build();

            Assert.That(zo1.Zasoby.Count, Is.EqualTo(1));
            Assert.That(zo2.Zasoby.Count, Is.EqualTo(1));

			var zasob = zo1.Zasoby.GetFirst() as Zasob;
            Assert.That(zasob.Ilosc.Value, Is.EqualTo(30d));
            Assert.That(zasob.Nadrzedny.Partia.Dokument.NumerPelnyZapisany, Is.EqualTo(zd.NumerPelnyZapisany));

			zasob = zo2.Zasoby.GetFirst() as Zasob;
            Assert.That(zasob.Ilosc.Value, Is.EqualTo(30d));
            Assert.That(zasob.Nadrzedny, Is.Null);

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

            Assert.That(zasobyZO.Count(), Is.EqualTo(2));

			zasob = zasobyZO.Where(x => x.Partia.Dokument.NumerPelnyZapisany == zo1.NumerPelnyZapisany).FirstOrDefault();
            Assert.That(zasob, Is.Not.Null);
            Assert.That(zasob.Ilosc.Value, Is.EqualTo(30d));
            Assert.That(zasob.Nadrzedny.Partia.Dokument.NumerPelnyZapisany, Is.EqualTo(zk.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].NumerPelnyZapisany));

			zasob = zasobyZO.Where(x => x.Partia.Dokument.NumerPelnyZapisany == zo2.NumerPelnyZapisany).FirstOrDefault();
            Assert.That(zasob, Is.Not.Null);
            Assert.That(zasob.Ilosc.Value, Is.EqualTo(30d));
            Assert.That(zasob.Nadrzedny, Is.Null);

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

            Assert.That(fv1.PozycjaWgIdent(1).Ilosc.Value, Is.EqualTo(30d));
            Assert.That(fv2.PozycjaWgIdent(1).Ilosc.Value, Is.EqualTo(30d));

            Assert.That(fv1.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].PozycjaWgIdent(1).Obroty.Count, Is.EqualTo(1));
			var obrot = fv1.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].PozycjaWgIdent(1).Obroty.GetFirst() as Obrot;
            Assert.That(obrot.Ilosc.Value, Is.EqualTo(30d));
            Assert.That(obrot.Przychod.Dokument.NumerPelnyZapisany, Is.EqualTo(zk.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].NumerPelnyZapisany));

            Assert.That(fv2.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].PozycjaWgIdent(1).Obroty.Count, Is.EqualTo(1));
			obrot = fv2.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].PozycjaWgIdent(1).Obroty.GetFirst() as Obrot;
            Assert.That(obrot.Ilosc.Value, Is.EqualTo(30d));
            Assert.That(obrot.Przychod.Dokument.NumerPelnyZapisany, Is.EqualTo(pz.NumerPelnyZapisany));

			fv2Builder
				.Bufor()
				.Usuń()
				.Utwórz();

			fv1Builder
				.Bufor()
				.Usuń()
				.Utwórz();

			zasobyZO = Session.GetMagazyny().Zasoby.Cast<Zasob>().Where(x => x.Partia.Dokument.Definicja.Symbol == "ZO").ToArray();

            Assert.That(zasobyZO.Count(), Is.EqualTo(2));

			zasob = zasobyZO.Where(x => x.Partia.Dokument.NumerPelnyZapisany == zo1.NumerPelnyZapisany).FirstOrDefault();
            Assert.That(zasob, Is.Not.Null);
            Assert.That(zasob.Ilosc.Value, Is.EqualTo(30d));
            Assert.That(zasob.Nadrzedny.Partia.Dokument.NumerPelnyZapisany, Is.EqualTo(zk.Podrzędne[TypRelacjiHandlowej.HandlowoMagazynowa].NumerPelnyZapisany));

			zasob = zasobyZO.Where(x => x.Partia.Dokument.NumerPelnyZapisany == zo2.NumerPelnyZapisany).FirstOrDefault();
            Assert.That(zasob, Is.Not.Null);
            Assert.That(zasob.Ilosc.Value, Is.EqualTo(30d));
            Assert.That(zasob.Nadrzedny, Is.Null);
		}
	}
}
