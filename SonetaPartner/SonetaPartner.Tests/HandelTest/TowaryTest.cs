using NUnit.Framework;
using Soneta.Business;
using Soneta.Handel;
using Soneta.Towary;
using SonetaPartner.Tests.Extensions.Handel.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.HandelTest
{
	internal class TowaryTest : TestHandel
	{
		protected override bool EnableDbTransation => false;

		[Test]
		public void ImportCsvTowary()
		{
			string projectDir = Path.GetFullPath(
				Path.Combine(AppContext.BaseDirectory, @"..\..\..\")
			);
			var resPath = Path.Combine(projectDir, "Res", "Towary.csv");
			string data = File.ReadAllText(resPath);

			using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(data)))
			{
				View view = Session.GetHandel().Towary.Towary.CreateView();
				view.Context = Context;
				var reader = new SessionDataReader();
				reader.DataSource = view;
				reader.Format = SessionDataReaderFormat.Csv;
				reader.Read(stream);
			}

			SaveDispose();
		}

		[Test]
		public void SprawdzenieTowarowWBazieTest()
		{
			var towar1 = Session.GetHandel().Towary.Towary.WgNazwy["C000001"].ToArray();
			Assert.That(towar1.Count() > 0);
			Assert.That(towar1[0].Kod, Is.EqualTo("C000001"));
			Assert.That(towar1[0].Nazwa, Is.EqualTo("C000001"));
			View view1 = towar1[0].Ceny.CreateView();
			var ceny1 = view1.Cast<Cena>().ToArray();
			Assert.That(ceny1.Count() == 3);
			var ceny11 = ceny1.Where(x => x.Definicja.Nazwa == "Podstawowa").First();
			var ceny12 = ceny1.Where(x => x.Definicja.Nazwa == "Hurtowa").First();
			var ceny13 = ceny1.Where(x => x.Definicja.Nazwa == "Detaliczna").First();
			Assert.That(ceny11, Is.Not.Null);
			Assert.That(ceny11.Netto.Value == 310);
			Assert.That(ceny11.Netto.Symbol == "PLN");
			Assert.That(ceny12, Is.Not.Null);
			Assert.That(ceny12.Netto.Value == 310);
			Assert.That(ceny12.Netto.Symbol == "PLN");
			Assert.That(ceny13, Is.Not.Null);
			Assert.That(ceny13.Brutto.Value == 381.3);
			Assert.That(ceny13.Brutto.Symbol == "PLN");

			var towar2 = Session.GetHandel().Towary.Towary.WgNazwy["C000002"].ToArray();
			Assert.That(towar2.Count() > 0);
			Assert.That(towar2[0].Kod, Is.EqualTo("C000002"));
			Assert.That(towar2[0].Nazwa, Is.EqualTo("C000002"));
			View view2 = towar2[0].Ceny.CreateView();
			var ceny2 = view2.Cast<Cena>().ToArray();
			Assert.That(ceny2.Count() == 3);
			var ceny21 = ceny1.Where(x => x.Definicja.Nazwa == "Podstawowa").First();
			var ceny22 = ceny1.Where(x => x.Definicja.Nazwa == "Hurtowa").First();
			var ceny23 = ceny1.Where(x => x.Definicja.Nazwa == "Detaliczna").First();
			Assert.That(ceny21, Is.Not.Null);
			Assert.That(ceny21.Netto.Value == 310);
			Assert.That(ceny21.Netto.Symbol == "PLN");
			Assert.That(ceny22, Is.Not.Null);
			Assert.That(ceny22.Netto.Value == 310);
			Assert.That(ceny22.Netto.Symbol == "PLN");
			Assert.That(ceny23, Is.Not.Null);
			Assert.That(ceny23.Brutto.Value == 381.3);
			Assert.That(ceny23.Brutto.Symbol == "PLN");
		}

		[Test]
		public void UsuniecieTowarowZBazyTest()
		{
			InTransaction(() => {
				var towary = Session.GetTowary().Towary.WgNazwy
					.Where(t => t.Nazwa.StartsWith("C000"));
				foreach (var tower in towary)
					tower.Delete();
			});
			SaveDispose();
		}
	}
}
