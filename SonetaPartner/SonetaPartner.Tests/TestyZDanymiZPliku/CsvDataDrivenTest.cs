using CsvHelper;
using CsvHelper.Configuration;
using NUnit.Framework;
using Soneta.Business;
using Soneta.Test;
using Soneta.Towary;
using SonetaPartner.Tests.Assemblers;
using SonetaPartner.Tests.Extensions.Handel.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SonetaPartner.Tests.TestyZDanymiZPliku
{
	internal class CsvDataDrivenTest : TestHandel
	{
		[Test]
		[Description("Dodawanie nowych towarów z pliku .csv w teście i sprawdzenie czy poprawnie zostały dodane")]
		public void DodawanieNowychTowarowTest()
		{
			var towary = CsvTowaryTestData.GetTowary();

			foreach (var item in towary)
			{
				Nowy<Towar>()
					.Nazwa(item.Nazwa)
					.Kod(item.Kod)
					.Cena("Podstawowa").Netto(item.Ceny_Podstawowa_Netto, item.Ceny_Podstawowa_Netto_Waluta)
					.GetParent<Towar>()
					.Cena("Hurtowa").Netto(item.Ceny_Hurtowa_Netto, item.Ceny_Hurtowa_Netto_Waluta)
					.GetParent<Towar>()
					.Cena("Detaliczna")
					.Enqueue(x => x.Brutto = new Soneta.Types.DoubleCy(item.Ceny_Detaliczna_Brutto, item.Ceny_Detaliczna_Brutto_Waluta))
					.Utwórz();
			}

			foreach (var item in towary)
			{
				var towar = (Towar)Session.GetTowary().Towary.WgNazwy[item.Nazwa].First();
				Assert.That(towar, Is.Not.Null);
				Assert.That(towar.Kod == item.Kod);
				View view = towar.Ceny.CreateView();
				var ceny = view.Cast<Cena>().ToArray();
				Assert.That(ceny.Count() == 3);
				var ceny1 = ceny.Where(x => x.Definicja.Nazwa == "Podstawowa").First();
				var ceny2 = ceny.Where(x => x.Definicja.Nazwa == "Hurtowa").First();
				var ceny3 = ceny.Where(x => x.Definicja.Nazwa == "Detaliczna").First();
				Assert.That(ceny1, Is.Not.Null);
				Assert.That(ceny1.Netto.Value == item.Ceny_Podstawowa_Netto);
				Assert.That(ceny1.Netto.Symbol == item.Ceny_Podstawowa_Netto_Waluta);
				Assert.That(ceny2, Is.Not.Null);
				Assert.That(ceny2.Netto.Value == item.Ceny_Hurtowa_Netto);
				Assert.That(ceny2.Netto.Symbol == item.Ceny_Hurtowa_Netto_Waluta);
				Assert.That(ceny3, Is.Not.Null);
				Assert.That(ceny3.Brutto.Value == item.Ceny_Detaliczna_Brutto);
				Assert.That(ceny3.Brutto.Symbol == item.Ceny_Detaliczna_Brutto_Waluta);
			}
		}

		[TestCaseSource(typeof(CsvTowaryTestData), nameof(CsvTowaryTestData.TowaryZCsv))]
		[Description("Test casy są generowane na podstawie pobranego pliku .csv - jeden przypadek testowy to jeden towar")]
		public void DodanieNowegoTowaruZCsvTest(TowarCsvRow item)
		{
			Nowy<Towar>()
				.Nazwa(item.Nazwa)
				.Kod(item.Kod)
				.Cena("Podstawowa").Netto(item.Ceny_Podstawowa_Netto, item.Ceny_Podstawowa_Netto_Waluta)
				.GetParent<Towar>()
				.Cena("Hurtowa").Netto(item.Ceny_Hurtowa_Netto, item.Ceny_Hurtowa_Netto_Waluta)
				.GetParent<Towar>()
				.Cena("Detaliczna")
				.Enqueue(x => x.Brutto = new Soneta.Types.DoubleCy(
					item.Ceny_Detaliczna_Brutto,
					item.Ceny_Detaliczna_Brutto_Waluta))
				.Utwórz();

			var towar = (Towar)Session.GetTowary()
				.Towary.WgNazwy[item.Nazwa]
				.First();

			Assert.That(towar, Is.Not.Null);
		}

		[TestCaseSource(typeof(CsvTowaryTestData), nameof(CsvTowaryTestData.TowaryZCsv2))]
		[Description("Test casy są generowane na podstawie pobranego pliku .csv dwa razy - jeden przypadek testowy to jeden towar")]
		public void DodanieNowegoTowaruZCsvDwaRazyTest(TowarCsvRow item)
		{
			Nowy<Towar>()
				.Nazwa(item.Nazwa)
				.Kod(item.Kod)
				.Cena("Podstawowa").Netto(item.Ceny_Podstawowa_Netto, item.Ceny_Podstawowa_Netto_Waluta)
				.GetParent<Towar>()
				.Cena("Hurtowa").Netto(item.Ceny_Hurtowa_Netto, item.Ceny_Hurtowa_Netto_Waluta)
				.GetParent<Towar>()
				.Cena("Detaliczna")
				.Enqueue(x => x.Brutto = new Soneta.Types.DoubleCy(
					item.Ceny_Detaliczna_Brutto,
					item.Ceny_Detaliczna_Brutto_Waluta))
				.Utwórz();

			var towar = (Towar)Session.GetTowary()
				.Towary.WgNazwy[item.Nazwa]
				.First();

			Assert.That(towar, Is.Not.Null);
		}
	}

	public class TowarCsvRow
	{
		public string Kod { get; set; }
		public string Nazwa { get; set; }
		public string EAN { get; set; }

		public double Ceny_Podstawowa_Netto { get; set; }
		public string Ceny_Podstawowa_Netto_Waluta { get; set; }

		public double Ceny_Hurtowa_Netto { get; set; }
		public string Ceny_Hurtowa_Netto_Waluta { get; set; }

		public double Ceny_Detaliczna_Brutto { get; set; }
		public string Ceny_Detaliczna_Brutto_Waluta { get; set; }
	}
}
