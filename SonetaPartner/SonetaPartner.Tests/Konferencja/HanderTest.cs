using NUnit.Framework;
using Soneta.Handel;
using Soneta.Test;
using Soneta.Types;
using SonetaPartner.Tests.Assemblers;
using SonetaPartner.Tests.Extensions.Handel.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.Konferencja
{
	internal class HanderTest : TestHandel
	{
		[Test]
		public void NowaFaktura_Assebler_Test()
		{
			var fv = Session.GetHandel().DokHandlowe
				.WgDefinicja[Session.GetHandel().DefDokHandlowych.WgSymbolu["FV"]].ToArray();

			Assert.IsTrue(fv.Count() == 0);

			Nowy<DokumentHandlowy>()
				.Data(Date.Today)
				.Kontrahent("ZEFIR")
				.Pozycja(KodyTowarów.Transport)
				.Ilosc(1)
				.Dokument()
				.Utwórz(c => c.Set(defDokHandlowego: "FV"));

			fv = Session.GetHandel().DokHandlowe
				.WgDefinicja[Session.GetHandel().DefDokHandlowych.WgSymbolu["FV"]].ToArray();

			Assert.IsTrue(fv.Count() == 1);
		}
	}
}
