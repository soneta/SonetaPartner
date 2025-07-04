using NUnit.Framework;
using Soneta.Core;
using Soneta.Handel.RelacjeDokumentow.Api;
using Soneta.Handel;
using Soneta.Types;
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
	internal class UmowyCykliczneTest : TestHandel
	{
		[Test]
		public void PoprawneNaliczaniePozycjiGdyPodzialOkresuProporcjaMiesieczna_Test()
		{
			Get<DefDokHandlowego>("UC")
				.Blokada(false)
				.Utwórz();

			Get<DefDokHandlowego>("AC")
				.Blokada(false)
				.Utwórz();

			Get<DefDokHandlowego>("UC")
				.DefRelacji("Faktura")
				.WieleNadrzednych()
				.Utwórz();

			Get<DefDokHandlowego>("AC")
				.DefRelacji("Faktura")
				.WieleNadrzednych()
				.Utwórz();

			var ucBuilder = Nowy<DokumentHandlowy>()
				.Data(new Date(2023, 12, 1))
				.Kontrahent("Abc")
				.Pozycja(KodyTowarów.Montaz).RodzajUmowy(RodzajUmowy.CyklicznaUsługa).Ilosc(1d).Cena(100d).Rabat(0m)
				.Cykl(
					DefinicjaCykluRodzajTerminu.WgOpisu,
					DefinicjaCykluTyp.Miesieczny,
					pozycjaDnia: DefinicjaCykluPozycjaDnia.Pierwszy)
				.Dokument()
				.CenaNaPodrzedny()
				.DataOperacji(new Date(2023, 12, 1))
				.Enqueue(x => x.UmowaInfo.DataOkresuRozliczeniowego = new Date(2023, 12, 1))
				.CyklUmowy(
					DefinicjaCykluRodzajTerminu.WgOpisu,
					DefinicjaCykluTyp.Miesieczny,
					pozycjaDnia: DefinicjaCykluPozycjaDnia.Pierwszy
					)
				.Enqueue(x => x.UmowaInfo.Cykl.Interwal = 3)
				.CyklDostawy(
					DefinicjaCykluRodzajTerminu.WgOpisu,
					DefinicjaCykluTyp.Miesieczny,
					pozycjaDnia: DefinicjaCykluPozycjaDnia.Pierwszy
					)
				.Zatwierdz();

			var uc = ucBuilder.Utwórz(cx => cx.Set(defDokHandlowego: "UC"));

			var fv = DokumentHandlowyAssembler.NowyWRelacji(uc, "FV", new HandlerSet
			{
				UstawParametryFakturowania = (args) =>
				{
					args.ZakresFakturowania = UmowaZakresFakturowania.WgCyklu;
					args.Zakres = UmowaZakresRozliczanychOkresow.PierwszyNiefakturowany;
				},
				WybierzDokumentyCallback = dp =>
				{
					dp.DokumentWybrany = dp.Dokumenty.Cast<DokumentHandlowy>().First();
				}
			})
			.Zatwierdz()
			.Utwórz();

            Assert.That(fv.Pozycje.Count, Is.EqualTo(1));
            Assert.That(fv.PozycjaWgIdent(1).Ilosc.Value, Is.EqualTo(3d));
            Assert.That(fv.PozycjaWgIdent(1).Cena.Value, Is.EqualTo(100d));
            Assert.That(fv.PozycjaWgIdent(1).Rabat, Is.EqualTo(new Percent(.0m)));
            Assert.That(fv.Suma.Netto, Is.EqualTo(300m));
            Assert.That(fv.Okres, Is.EqualTo(new FromTo(new Date(2023, 12, 1), new Date(2024, 2, 29))));
            Assert.That(fv.PozycjaWgIdent(1).OkresRozliczony, Is.EqualTo(new FromTo(new Date(2023, 12, 1), new Date(2024, 2, 29))));

			var aucBuilder = DokumentHandlowyAssembler.Korekta(uc)
				.Data(new Date(2024, 4, 1))
				.DataOperacji(new Date(2024, 4, 1))
				.Pozycja(1).Cena(200d).Dokument()
				.Zatwierdz();

			var auc = aucBuilder.Utwórz();

			fv = DokumentHandlowyAssembler.NowyWRelacji(auc, "FV", new HandlerSet
			{
				UstawParametryFakturowania = (args) =>
				{
					args.ZakresFakturowania = UmowaZakresFakturowania.WgCyklu;
					args.Zakres = UmowaZakresRozliczanychOkresow.PierwszyNiefakturowany;
				},
				WybierzDokumentyCallback = dp =>
				{
					dp.DokumentWybrany = dp.Dokumenty.Cast<DokumentHandlowy>().First();
				}
			})
			.Zatwierdz()
			.Utwórz();

            Assert.That(fv.Pozycje.Count, Is.EqualTo(1));
            Assert.That(fv.PozycjaWgIdent(1).Ilosc.Value, Is.EqualTo(1d));
            Assert.That(fv.PozycjaWgIdent(1).Cena.Value, Is.EqualTo(100d));
            Assert.That(fv.PozycjaWgIdent(1).Rabat, Is.EqualTo(new Percent(.0m)));
            Assert.That(fv.Suma.Netto, Is.EqualTo(100m));
            Assert.That(fv.Okres, Is.EqualTo(new FromTo(new Date(2024, 3, 1), new Date(2024, 3, 31))));
            Assert.That(fv.PozycjaWgIdent(1).OkresRozliczony, Is.EqualTo(new FromTo(new Date(2024, 3, 1), new Date(2024, 3, 31))));

			auc = aucBuilder.Utwórz();

			fv = DokumentHandlowyAssembler.NowyWRelacji(auc, "FV", new HandlerSet
			{
				UstawParametryFakturowania = (args) =>
				{
					args.ZakresFakturowania = UmowaZakresFakturowania.WgCyklu;
					args.Zakres = UmowaZakresRozliczanychOkresow.PierwszyNiefakturowany;
				},
				WybierzDokumentyCallback = dp =>
				{
					dp.DokumentWybrany = dp.Dokumenty.Cast<DokumentHandlowy>().First();
				}
			})
			.Zatwierdz()
			.Utwórz();

            Assert.That(fv.Pozycje.Count, Is.EqualTo(1));
            Assert.That(fv.PozycjaWgIdent(1).Ilosc.Value, Is.EqualTo(2d));
            Assert.That(fv.PozycjaWgIdent(1).Cena.Value, Is.EqualTo(200d));
            Assert.That(fv.PozycjaWgIdent(1).Rabat, Is.EqualTo(new Percent(.0m)));
            Assert.That(fv.Suma.Netto, Is.EqualTo(400m));
            Assert.That(fv.Okres, Is.EqualTo(new FromTo(new Date(2024, 4, 1), new Date(2024, 5, 31))));
            Assert.That(fv.PozycjaWgIdent(1).OkresRozliczony, Is.EqualTo(new FromTo(new Date(2024, 4, 1), new Date(2024, 5, 31))));
		}
	}
}
