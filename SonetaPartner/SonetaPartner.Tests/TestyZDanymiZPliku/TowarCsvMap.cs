using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.TestyZDanymiZPliku
{
	public sealed class TowarCsvMap : ClassMap<TowarCsvRow>
	{
		public TowarCsvMap()
		{
			Map(m => m.Kod).Name("Kod");
			Map(m => m.Nazwa).Name("Nazwa");
			Map(m => m.EAN).Name("EAN");

			Map(m => m.Ceny_Podstawowa_Netto).Name("Ceny.Podstawowa.Netto");
			Map(m => m.Ceny_Podstawowa_Netto_Waluta).Name("Ceny.Podstawowa.Netto.Waluta");

			Map(m => m.Ceny_Hurtowa_Netto).Name("Ceny.Hurtowa.Netto");
			Map(m => m.Ceny_Hurtowa_Netto_Waluta).Name("Ceny.Hurtowa.Netto.Waluta");

			Map(m => m.Ceny_Detaliczna_Brutto).Name("Ceny.Detaliczna.Brutto");
			Map(m => m.Ceny_Detaliczna_Brutto_Waluta).Name("Ceny.Detaliczna.Brutto.Waluta");
		}
	}
}
