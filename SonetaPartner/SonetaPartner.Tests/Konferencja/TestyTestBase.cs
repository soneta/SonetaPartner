using NUnit.Framework;
using Soneta.Business.Compiler;
using Soneta.CRM;
using Soneta.Handel;
using Soneta.Magazyny;
using Soneta.SrodkiTrwale;
using Soneta.Test;
using Soneta.Towary;
using Soneta.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.Konferencja
{
	internal class TestyTestBase : TestBase
	{
		[Test]
		public void SrodekTrwalu_Test()
		{

            Assert.That(Session.GetSrodkiTrwale()
                .SrodkiTrwale.Rows.Count() == 0, Is.True);

			InTransaction(() =>
			{
				var st = new SrodekTrwaly();
				Session.AddRow(st);
				st.Nazwa = "Nowy srodek trwaly";
				st.NumerInwentarzowy = "1";
				st.Last.KRST = st.Module.KRST.WgTyp[TypSrodkaTrwalego.ŚrodekTrwały,
					 "1"].FirstOrDefault();

			});

            Assert.That(Session.GetSrodkiTrwale()
                .SrodkiTrwale.Rows.Count() == 1, Is.True);

            Assert.That(Session.GetSrodkiTrwale()
                .SrodkiTrwale.WgNumeruInwentarzowego["1"] != null, Is.True);


            Assert.That(Session.GetSrodkiTrwale()
                .SrodkiTrwale.WgNumeruInwentarzowego["2"] == null, Is.True);
		}

		[Test]
		public void NowaFaktura_Test()
		{
			var fv = Session.GetHandel().DokHandlowe
				.WgDefinicja[Session.GetHandel().DefDokHandlowych.WgSymbolu["FV"]].ToArray();

            Assert.That(fv.Count() == 0, Is.True);

			InTransaction(() =>
			{
				var dh = new DokumentHandlowy();
				Session.AddRow(dh);
				dh.Data = Date.Today;
				dh.Definicja = Session.GetHandel().DefDokHandlowych.WgSymbolu["FV"];
				dh.Kontrahent = Session.GetCRM().Kontrahenci.WgKodu["ZEFIR"];
				dh.Magazyn = Session.GetMagazyny().Magazyny.Firma;

				PozycjaDokHandlowego poz = new PozycjaDokHandlowego(dh);
				Session.AddRow(poz);
				poz.Towar = Session.GetTowary().Towary.WgKodu["Transport"];
				poz.Ilosc = new Quantity(1);
			});
			//SaveDispose();

			fv = Session.GetHandel().DokHandlowe
				.WgDefinicja[Session.GetHandel().DefDokHandlowych.WgSymbolu["FV"]].ToArray();

            Assert.That(fv.Count() == 1, Is.True);
		}
	}
}
