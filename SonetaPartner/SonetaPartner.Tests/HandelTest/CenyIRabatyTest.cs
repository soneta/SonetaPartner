using FluentAssertions;
using NUnit.Framework;
using Soneta.Business.Db;
using Soneta.CRM;
using Soneta.Handel;
using Soneta.Test;
using Soneta.Towary;
using Soneta.Types;
using SonetaPartner.Tests.Assemblers;
using SonetaPartner.Tests.Extensions.Handel.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.HandelTest
{
	internal class CenyIRabatyTest : TestHandel
	{
		[Test]
		public void SprawdzeniePoprawnościNaliczeniaRabatow_Test()
		{
			KonfigurujRabaty(true, false);
			FakturaZakupuUtworz();

			var fakturaBuilder = Nowy<DokumentHandlowy>()
			   .Kontrahent("Zefir")
			   .Pozycja("A").Ilosc(1)
			   .Pozycja("B").Ilosc(2)
			   .Pozycja("C").Ilosc(3)
			   .Pozycja("D").Ilosc(4)
			   .Pozycja("E").Ilosc(5)
			   .Pozycja("F").Ilosc(6)
			   .Pozycja("G").Ilosc(7)
			   .Dokument()
			   .Zatwierdz();
			var FV = fakturaBuilder.Utwórz(cx => cx.Set(defDokHandlowego: "FV"));

			FV.PozycjaWgIdent(1).Rabat.ToString().Should().Be("10,00%");
			FV.PozycjaWgIdent(2).Rabat.ToString().Should().Be("18,00%");
			FV.PozycjaWgIdent(3).Rabat.ToString().Should().Be("3,00%");
			FV.PozycjaWgIdent(4).Rabat.ToString().Should().Be("9,00%");
			FV.PozycjaWgIdent(5).Rabat.ToString().Should().Be("1,00%");
			FV.PozycjaWgIdent(6).Rabat.ToString().Should().Be("0,00%");
			FV.PozycjaWgIdent(7).Rabat.ToString().Should().Be("1,00%");

			FV.Suma.Netto.Should().Be(6134.04m);
			FV.Suma.Brutto.Should().Be(7544.87m);
		}

		public void KonfigurujRabaty(bool rabatIndywidualnyZefir = false, bool rabatIndywidualnyGawron = true, bool dodatkoweUstawienia = true)
		{
			var towarABuilder = Nowy<Towar>()
				.Kod("A").Nazwa("A")
				.Cena("Podstawowa").Netto(310d)
				.GetParent<Towar>()
				.Cena("Detaliczna")
				.Enqueue(x => x.Narzut = new Percent(0.375m))
				.GetParent<Towar>()
				.Cena("Hurtowa")
				.Enqueue(x => x.Narzut = new Percent(0.1m))
				.GetParent<Towar>();
			var A = towarABuilder.Utwórz();

			var towarBBuilder = Nowy<Towar>()
				.Kod("B").Nazwa("B")
				.Cena("Podstawowa").Netto(300d)
				.GetParent<Towar>()
				.Cena("Detaliczna")
				.Enqueue(x => x.Narzut = new Percent(0.375m))
				.GetParent<Towar>()
				.Cena("Hurtowa")
				.Enqueue(x => x.Narzut = new Percent(0.1m))
				.GetParent<Towar>();
			var B = towarBBuilder.Utwórz();

			var towarCBuilder = Nowy<Towar>()
				.Kod("C").Nazwa("C")
				.Cena("Podstawowa").Netto(250d)
				.GetParent<Towar>()
				.Cena("Detaliczna")
				.Enqueue(x => x.Narzut = new Percent(0.375m))
				.GetParent<Towar>()
				.Cena("Hurtowa")
				.Enqueue(x => x.Narzut = new Percent(0.1m))
				.GetParent<Towar>();
			var C = towarCBuilder.Utwórz();

			var towarDBuilder = Nowy<Towar>()
				.Kod("D").Nazwa("D")
				.Cena("Podstawowa").Netto(300d)
				.GetParent<Towar>()
				.Cena("Detaliczna")
				.Enqueue(x => x.Narzut = new Percent(0.375m))
				.GetParent<Towar>()
				.Cena("Hurtowa")
				.Enqueue(x => x.Narzut = new Percent(0.1m))
				.GetParent<Towar>();
			var D = towarDBuilder.Utwórz();

			var towarEBuilder = Nowy<Towar>()
				.Kod("E").Nazwa("E")
				.Cena("Podstawowa").Netto(160d)
				.GetParent<Towar>()
				.Cena("Detaliczna")
				.Enqueue(x => x.Narzut = new Percent(0.375m))
				.GetParent<Towar>()
				.Cena("Hurtowa")
				.Enqueue(x => x.Narzut = new Percent(0.1m))
				.GetParent<Towar>();
			var E = towarEBuilder.Utwórz();

			var towarFBuilder = Nowy<Towar>()
				.Kod("F").Nazwa("F")
				.Cena("Podstawowa").Netto(150d)
				.GetParent<Towar>()
				.Cena("Detaliczna")
				.Enqueue(x => x.Narzut = new Percent(0.375m))
				.GetParent<Towar>()
				.Cena("Hurtowa")
				.Enqueue(x => x.Narzut = new Percent(0.1m))
				.GetParent<Towar>();
			var F = towarFBuilder.Utwórz();

			var towarGBuilder = Nowy<Towar>()
				.Kod("G").Nazwa("G")
				.Cena("Podstawowa").Netto(230d)
				.GetParent<Towar>()
				.Cena("Detaliczna")
				.Enqueue(x => x.Narzut = new Percent(0.375m))
				.GetParent<Towar>()
				.Cena("Hurtowa")
				.Enqueue(x => x.Narzut = new Percent(0.1m))
				.GetParent<Towar>();
			var G = towarGBuilder.Utwórz();

			var defGrupy1 = FeatureDefinitionAssembler.Nowa("GrupaRabatowa", "Kontrahenci").Dictionary("GrupaRabatowa").Utwórz().Guid;
			var defGrupy2 = Session.GetBusiness().FeatureDefs.ByName["Towary", "Producent"].Guid;

			var grupaRabatowanaBuilder_AA = DictionaryItemAssembler
				.Nowa("F.GrupaRabatowa")
				.Value("AA");
			var grupaRabatowana_AA = grupaRabatowanaBuilder_AA.Utwórz();

			var grupaRabatowanaBuilder_BB = DictionaryItemAssembler
				.Nowa("F.GrupaRabatowa")
				.Value("BB");
			var grupaRabatowana_BB = grupaRabatowanaBuilder_BB.Utwórz();

			Get<Kontrahent>("ABC")
			 .SetFeature("GrupaRabatowa", grupaRabatowanaBuilder_AA.Build().Value)
			 .Utwórz();

			Get<Kontrahent>("ZEFIR")
			   .SetFeature("GrupaRabatowa", grupaRabatowanaBuilder_AA.Build().Value)
			   .Utwórz();

			Get<Kontrahent>("GAWRON")
			   .SetFeature("GrupaRabatowa", grupaRabatowanaBuilder_BB.Build().Value)
			   .Utwórz();

			Get<DefinicjaCeny>("Hurtowa")
			   .Rabat1Rodzaj(RodzajRabatu.GrupowyWszystkichTowarów)
			   .Rabat1Wliczaj(WliczanieRabatu.SumujRabatyPozycjiDokumentu)
			   .Rabat1Grupa(defGrupy1)
			   .Rabat2Rodzaj(RodzajRabatu.GrupowyGrupyTowarowej)
			   .Rabat2Wliczaj(WliczanieRabatu.SumujRabatyPozycjiDokumentu)
			   .Rabat2Grupa(defGrupy1)
			   .Rabat2GrupaTowarowa(defGrupy2)
			   .Rabat3Rodzaj(RodzajRabatu.GrupowyKażdegoTowaru)
			   .Rabat3Wliczaj(WliczanieRabatu.SumujRabatyPozycjiDokumentu)
			   .Rabat3Grupa(defGrupy1)
			   .Rabat4Rodzaj(RodzajRabatu.IndywidualnyGrupyTowarowej)
			   .Rabat4Wliczaj(WliczanieRabatu.SumujRabatyPozycjiDokumentu)
			   .Rabat4GrupaTowarowa(defGrupy2)
			   .Rabat5Rodzaj(RodzajRabatu.IndywidualnyKażdegoTowaru)
			   .Rabat5Wliczaj(WliczanieRabatu.SumujRabatyPozycjiDokumentu)
			   .Utwórz();

			if (dodatkoweUstawienia)
			{
				var producentBuilder_SP = DictionaryItemAssembler
				  .Nowa("F.Producent")
				  .Value("SuperPower");
				var producent_SP = producentBuilder_SP.Utwórz();

				var producentBuilder_DE = DictionaryItemAssembler
				  .Nowa("F.Producent")
				  .Value("DexoElektro");
				var producent_DE = producentBuilder_DE.Utwórz();

				towarBBuilder
				 .SetFeature("Producent", producentBuilder_SP.Build().Value)
				 .Utwórz();
				towarDBuilder
				 .SetFeature("Producent", producentBuilder_SP.Build().Value)
				 .Utwórz();
				towarABuilder
				 .SetFeature("Producent", producentBuilder_DE.Build().Value)
				 .Utwórz();
				towarCBuilder
				 .SetFeature("Producent", producentBuilder_DE.Build().Value)
				 .Utwórz();

				InUITransaction(() =>
				{
					//Grupa rabatowa
					//rabaty towarów
					var worker1 = new CennikGrupyWorker();
					worker1.Grupa = grupaRabatowanaBuilder_AA.Build();
					worker1.Towar = towarABuilder.Build();
					worker1.Rabat = new Percent(0.05m);

					worker1.Towar = towarBBuilder.Build();
					worker1.Rabat = new Percent(0.06m);

					//rabaty grup
					var worker2 = new RabatyGrupyWorker();
					worker2.Grupa = grupaRabatowanaBuilder_AA.Build();
					worker2.Rabat = new Percent(0.01m);

					worker2.Grupa = grupaRabatowanaBuilder_BB.Build();
					worker2.Rabat = new Percent(0.02m);

					//rabaty grupy grup
					var worker3 = new RabatyGrupyTowaruGrupyWorker();
					worker3.Grupa = grupaRabatowanaBuilder_AA.Build();
					worker3.GrupaTowaru = producentBuilder_SP.Build();
					worker3.Rabat = new Percent(0.03m);

					worker3.GrupaTowaru = producentBuilder_DE.Build();
					worker3.Rabat = new Percent(0.02m);

					//Producent
					//rabat kontrahentow
					var worker4 = new RabatyGrupyTowaruKontrahentaWorker();
					worker4.GrupaTowaru = producentBuilder_SP.Build();
					worker4.Kontrahent = Get<Kontrahent>("ABC").Build();
					worker4.Rabat = new Percent(0.04m);

					worker4.Kontrahent = Get<Kontrahent>("ZEFIR").Build();
					worker4.Rabat = new Percent(0.05m);

					//rabat grupy grup
					var worker5 = new RabatyGrupyTowaruGrupyWorker();
					worker5.GrupaTowaru = producentBuilder_SP.Build();
					worker5.Grupa = grupaRabatowanaBuilder_AA.Build();
					worker5.Rabat = new Percent(0.03m);
				});
				SaveDispose();

				Nowy<CenaIndywidualna>()
				   .Rabat(.01m)
				   .Utwórz(cx => cx.Set(null, null, kontrahent: "ABC", towar: "A"));

				Nowy<CenaIndywidualna>()
				   .Rabat(.02m)
				   .Utwórz(cx => cx.Set(null, null, kontrahent: "ABC", towar: "B"));

				if (rabatIndywidualnyZefir)
				{
					Nowy<CenaIndywidualna>()
					   .Rabat(.02m)
					   .Utwórz(cx => cx.Set(null, null, kontrahent: "ZEFIR", towar: "A"));

					Nowy<CenaIndywidualna>()
					   .Rabat(.03m)
					   .Utwórz(cx => cx.Set(null, null, kontrahent: "ZEFIR", towar: "B"));
				}

				if (rabatIndywidualnyGawron)
					Nowy<CenaIndywidualna>()
						.Netto(220.00d)
						.Utwórz(cx => cx.Set(null, null, kontrahent: "GAWRON", towar: "G"));

				Get<DefDokHandlowego>("WZ").UstawRabatOperatora(0.15m).Utwórz();
			}

			var przecenaOgolnaBuilder = Nowy<PrzecenaOkresowaCeny>()
			   .PrzecenaOkresowaTowaru("F").Netto(110.0).PrzecenaOkresowaCeny()
			   .TypPrzeceny(TypPrzecenyOkresowej.Ogólna)
			   .Zatwierdz()
			   .Utwórz(cx => cx.Set(new PrzecenyOkresoweParams(cx)));

			Assert.True(przecenaOgolnaBuilder.Zatwierdzona);
			Assert.AreEqual(1, przecenaOgolnaBuilder.PrzecenyTowarow.Count);
		}

		private void FakturaZakupuUtworz()
		{
			var zakupBuilder = Nowy<DokumentHandlowy>()
			   .Kontrahent("ABC")
			   .Pozycja("A").Ilosc(100d).Cena(100d)
			   .Pozycja("B").Ilosc(100d).Cena(100d)
			   .Pozycja("C").Ilosc(100d).Cena(100d)
			   .Pozycja("D").Ilosc(100d).Cena(100d)
			   .Pozycja("E").Ilosc(100d).Cena(100d)
			   .Pozycja("F").Ilosc(100d).Cena(100d)
			   .Pozycja("G").Ilosc(100d).Cena(100d)
			   .Dokument()
			   .Zatwierdz();
			var ZK = zakupBuilder.Utwórz(cx => cx.Set(defDokHandlowego: "ZK"));
		}
	}
}
