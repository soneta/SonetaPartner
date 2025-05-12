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

namespace SonetaPartner.Tests.HandelTest
{
	internal class DokumentyMagazynowe : TestHandel
	{
		[Test]
		public void WorkerMarzyKoncowejPrzyMMP_Test()
		{
			Get<DefDokHandlowego>("WZ 2")
				.DefRelacji("Handlowy")
				.Agregowanie(SposobLaczeniaPozycji.TylkoTowar)
				.Utwórz();

			Nowy<DokumentHandlowy>()
				.Pozycja(KodyTowarów.Bikini).Ilosc(10d).Cena(1d)
				.Dokument().Zatwierdz().Utwórz(x => x.Set());

			Date today = Date.Today;
			DodajDrugiMagazyn("A");

			Nowy<DokumentHandlowy>()
			  .Data(today.AddDays(-5))
			  .DataOtrzymania(today.AddDays(-5))
			  .Kontrahent("FISZBIN")
			  .Pozycja(KodyTowarów.Kij160).Ilosc(3d).Cena(10d)
			  .Dokument()
			  .Zatwierdz()
			  .Utwórz(x => x.Set(defDokHandlowego: "ZK"));

			var zk2 = Nowy<DokumentHandlowy>()
			 .Data(today.AddDays(-4))
			 .DataOtrzymania(today.AddDays(-4))
			 .Kontrahent("FISZBIN")
			 .Pozycja(KodyTowarów.Kij160).Ilosc(7d).Cena(10d)
			 .Dokument()
			 .Zatwierdz()
			 .Utwórz(x => x.Set(defDokHandlowego: "ZK"));

			Nowy<DokumentHandlowy>()
			   .MagazynDo("A")
			   .Data(today.AddDays(-3))
			   .Pozycja(KodyTowarów.Kij160).Ilosc(10).Dokument()
			   .Zatwierdz()
			   .Utwórz(c => c.Set(defDokHandlowego: "MM"));

			var wz = Nowy<DokumentHandlowy>()
			  .Data(today.AddDays(-2))
			  .Kontrahent("ZEFIR")
			  .Pozycja(KodyTowarów.Kij160).Ilosc(10d).Cena(100d)
			  .Dokument()
			  .Zatwierdz();

			wz.Utwórz(x => x.Set(defDokHandlowego: "WZ 2", magazyn: "A"));

			var poz1 = wz.Build().PozycjaWgIdent(1);
			var worker = CreateContextRow(poz1, Context)
			  .GetWorker<ObrotyTransakcjiWorker>();

			Assert.AreEqual(worker.Marza, 900);

			var kzk2 = DokumentHandlowyAssembler.Korekta(zk2)
			 .Pozycja(1)
			 .WartoscCy(170d)
			 .Dokument()
			 .Zatwierdz()
			 .Utwórz();
			var poz2 = wz.Build().PozycjaWgIdent(1);

			var worker2 = CreateContextRow(wz.Build(), Context)
			  .GetWorker<ObrotyTransakcjiWorker>();
			Assert.AreEqual(worker2.Marza, 800);
		}
	}
}
