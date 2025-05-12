using NUnit.Framework;
using Soneta.Handel.RelacjeDokumentow.Api;
using Soneta.Handel;
using Soneta.Magazyny.StanMagazynu;
using Soneta.Towary;
using Soneta.Types;
using SonetaPartner.Tests.Assemblers;
using SonetaPartner.Tests.Extensions.Handel.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soneta.Test;
using Soneta.CRM;
using Soneta.Kasa;
using static Soneta.Core.DokEwidencji;

namespace SonetaPartner.Tests.HandelTest
{
	internal class DokumentyHandloweTest : TestHandel
	{
		[Test]
		public void LimitKredytowy_Test()
		{
			Get<DefDokHandlowego>("FV")
			.Enqueue(x => x.KontrolaLimituKredytowego = true)
			.Utwórz(x => x.UpdateModule(y => y.Config.Ogólne.KontrolaLimitówKredytowych = true));
			InUIConfigTransaction(() =>
			{
				var ustawienie = ConfigEditSession.Get<KasaModule>().Config.LimitKredytowy;
				ustawienie.WydanieMagBezPlat = PodstawaLimituKredytowego.Zawsze;
			});
			SaveDisposeConfig();
			Get<Kontrahent>("ABC")
				.Enqueue(x => x.LimitKredytu = 1000.00m)
				.Utwórz();
			Nowy<DokumentHandlowy>()
				.Pozycja(KodyTowarów.Bikini).Ilosc(100).Cena(10.00d)
				.Dokument()
				.Zatwierdz()
				.Utwórz(cx => cx.Set("ZK", kontrahent: null));
			Nowy<DokumentHandlowy>()
				.LiczonaOd(SposobLiczeniaVAT.OdBrutto)
				.Kontrahent("ABC")
				.Pozycja(KodyTowarów.Transport).Cena(500.0d).Rabat(0)
				.Dokument()
				.Zatwierdz()
				.Utwórz(cx => cx.Set("FV", kontrahent: "ABC"));

			var wz2 = Nowy<DokumentHandlowy>()
				.LiczonaOd(SposobLiczeniaVAT.OdBrutto)
				.Kontrahent("ABC")
				.Pozycja(KodyTowarów.Transport).Cena(300.0d).Rabat(0)
				.Dokument()
				.Zatwierdz();
			wz2.Utwórz(cx => cx.Set("WZ 2", kontrahent: "ABC"));
			var worker = new ParametryLimituKredytowegoWorker { Podmiot = CRMModule.GetInstance(Session).Kontrahenci.WgKodu["Abc"] };
			Assert.AreEqual(200.00, worker.Dostępny.Value);
			var fv2 = DokumentHandlowyAssembler
					  .NowyWRelacji(wz2.Build(), "FV 2")
					  .Kontrahent("Abc")
					  .Zatwierdz()
					  .Utwórz();
			Assert.AreEqual(false, fv2.Bufor);
			var worker_after = new ParametryLimituKredytowegoWorker { Podmiot = CRMModule.GetInstance(Session).Kontrahenci.WgKodu["Abc"] };
			Assert.AreEqual(200.00, worker_after.Dostępny.Value);
		}

		[Test]
		public void ZbiorczaPredekretacjaIKsiegowanieZbiorcze_Test()
		{
			Get<DefDokHandlowego>("FZAL")
				.DefRelacji("Faktura")
				.ZPodrzednegoDolaczDokument()
				.Utwórz();

			var fzal1_build = Nowy<DokumentHandlowy>()
				.Kontrahent("Zefir")
				.Pozycja(KodyTowarów.Montaz)
				.Ilosc(1d)
				.Cena(500d)
				.Dokument()
				.BruttoCy(300)
				.Zatwierdz();

			var fzal1 = fzal1_build.Build(x => x.Set(defDokHandlowego: "FZAL"));

			var fzal2_build = Nowy<DokumentHandlowy>()
						.Kontrahent("Zefir")
						.Pozycja(KodyTowarów.Montaz)
						.Ilosc(1)
						.Cena(500)
						.Dokument()
						.BruttoCy(150)
						.Zatwierdz();

			var fzal2 = fzal2_build.Build(x => x.Set(defDokHandlowego: "FZAL"));

			var fv_builder = Nowy<DokumentHandlowy>()
						 .Kontrahent("Zefir");

			fv_builder.DolaczNadrzedny("Faktura zaliczkowa", new HandlerSet
			{
				WybierzDokumentyCallback =
					t =>
					{
						t.DokumentyWybrane = new[] { fzal1_build.Build(), fzal2_build.Build() };
					}
			});

			var fv = fv_builder.Utwórz(x => x.Set(defDokHandlowego: "FV"));

			var pozycje = fv.Pozycje.Cast<PozycjaDokHandlowego>().ToArray();

			var zaliczki = fv.DokumentyZaliczkowe.Cast<DokumentHandlowy>().Select(d => d.BruttoCy.Value).Sum();
			Assert.AreEqual(780.00d, fv.BruttoCy.Value);
			Assert.AreEqual(450.00d, zaliczki);
		}

		[Test]
		public void WystawienieFakturySprzedazyZDolaczeniemFakturyZaliczkowej_Test()
		{
			Get<DefDokHandlowego>("FZAL")
				.DefRelacji("Faktura")
				.ZPodrzednegoDolaczDokument()
				.Utwórz();

			var fzal1_build = Nowy<DokumentHandlowy>()
				.Kontrahent("Zefir")
				.Pozycja(KodyTowarów.Montaz)
				.Ilosc(1d)
				.Cena(500d)
				.Dokument()
				.BruttoCy(300)
				.Zatwierdz();

			var fzal1 = fzal1_build.Build(x => x.Set(defDokHandlowego: "FZAL"));

			var fzal2_build = Nowy<DokumentHandlowy>()
						.Kontrahent("Zefir")
						.Pozycja(KodyTowarów.Montaz)
						.Ilosc(1)
						.Cena(500)
						.Dokument()
						.BruttoCy(150)
						.Zatwierdz();

			var fzal2 = fzal2_build.Build(x => x.Set(defDokHandlowego: "FZAL"));

			var fv_builder = Nowy<DokumentHandlowy>()
						 .Kontrahent("Zefir");

			fv_builder.DolaczNadrzedny("Faktura zaliczkowa", new HandlerSet
			{
				WybierzDokumentyCallback =
					t =>
					{
						t.DokumentyWybrane = new[] { fzal1_build.Build(), fzal2_build.Build() };
					}
			});

			var fv = fv_builder.Utwórz(x => x.Set(defDokHandlowego: "FV"));

			var pozycje = fv.Pozycje.Cast<PozycjaDokHandlowego>().ToArray();

			var zaliczki = fv.DokumentyZaliczkowe.Cast<DokumentHandlowy>().Select(d => d.BruttoCy.Value).Sum();
			Assert.AreEqual(780.00d, fv.BruttoCy.Value);
			Assert.AreEqual(450.00d, zaliczki);
		}
	}
}
