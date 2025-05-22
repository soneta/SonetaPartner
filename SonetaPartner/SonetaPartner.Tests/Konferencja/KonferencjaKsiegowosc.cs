using FluentAssertions;
using NUnit.Framework;
using Soneta.Kasa.Extensions;
using Soneta.SrodkiTrwale;
using SonetaPartner.Tests.Assemblers;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.Konferencja
{
	internal class KonferencjaKsiegowosc : TestKsiegowosc
	{
		[Test]
		public void SrodekTrwalu_Assembler_Test()
		{
			NewSrodekTrwaly("1", "ST", "Nowy środek trwały", rodzaj: "1")
				.SetSezonowosc(RodzajSezonowosci.Roczna)
				.Update()
				.GoSave();

			GetFinder()
				.SrodkiTrwale()
				.GetTheOnlyElement()
				.Last.Sezonowosc.Rodzaj.Should().Be(RodzajSezonowosci.Roczna);
		}
	}
}
