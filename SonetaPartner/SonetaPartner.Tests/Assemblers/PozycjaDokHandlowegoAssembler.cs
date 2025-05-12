using Soneta.Core;
using Soneta.Handel;
using Soneta.Test;
using Soneta.Towary;
using Soneta.Types;

namespace SonetaPartner.Tests.Assemblers
{
	public static class PozycjaDokHandlowegoAssembler
	{
		internal static void Set(
		this DefinicjaCyklu cycleDef,
		DefinicjaCykluTyp typ,
		DefinicjaCykluRodzajTerminu rodzajTerminu,
		int dzienMiesiaca,
		DefinicjaCykluPozycjaDnia pozycjaDnia,
		DefinicjaCykluOkresCyklu okresCyklu,
		DefinicjaCykluSposobNaDniWolne dniWolne)
		{
			cycleDef.Typ = typ;
			cycleDef.RodzajTerminu = rodzajTerminu;
			cycleDef.PozycjaDnia = pozycjaDnia;
			cycleDef.DzienMiesiaca = dzienMiesiaca;
			cycleDef.OkresCyklu = okresCyklu;
			cycleDef.SposobNaDniWolne = dniWolne;
		}
		
		internal static IRowBuilder<PozycjaDokHandlowego> Pozycja(this IRowBuilder<PozycjaDokHandlowego> builder, string kod)
		{
			return builder.GetParent<DokumentHandlowy>().Pozycja(kod);
		}
		
		public static IRowBuilder<P> Cena<P>(this IRowBuilder<P> builder, double cena) where P : PozycjaDokHandlowego
		{
			return builder.Enqueue(p => p.Cena = new DoubleCy(cena, p.Cena.Symbol));
		}

		public static IRowBuilder<PozycjaDokHandlowego> Ilosc(this IRowBuilder<PozycjaDokHandlowego> builder, double ilość)
		{
			return builder.Enqueue(p => p.Ilosc = new Quantity(ilość, p.Ilosc.Symbol));
		}

		public static IRowBuilder<DokumentHandlowy> Dokument(this IRowBuilder<PozycjaDokHandlowego> builder)
		{
			return builder.GetParent<DokumentHandlowy>();
		}

		internal static IRowBuilder<PozycjaDokHandlowego> Towar(this IRowBuilder<PozycjaDokHandlowego> builder, string kod)
		{
			return builder.Enqueue(p => p.Towar = p.Module.Towary.Towary.WgKodu[kod]);
		}

		internal static IRowBuilder<PozycjaDokHandlowego> WartoscCy(this IRowBuilder<PozycjaDokHandlowego> builder, double value)
		{
			return builder.Enqueue(p => p.WartoscCy = new Currency(value, p.WartoscCy.Symbol));
		}

		internal static IRowBuilder<PozycjaDokHandlowego> RodzajUmowy(this IRowBuilder<PozycjaDokHandlowego> builder, RodzajUmowy rodzajUmowy)
		{
			return builder.Enqueue(p => p.RodzajUmowy = rodzajUmowy);
		}

		internal static IRowBuilder<P> Rabat<P>(this IRowBuilder<P> builder, decimal rabat) where P : PozycjaDokHandlowego
		{
			return builder.Enqueue(p => p.Rabat = new Percent(rabat));
		}

		internal static IRowBuilder<P> Cykl<P>(
		this IRowBuilder<P> builder,
		DefinicjaCykluRodzajTerminu rodzajTerminu,
		DefinicjaCykluTyp typ = DefinicjaCykluTyp.Miesieczny,
		int dzienMiesiaca = 1,
		DefinicjaCykluPozycjaDnia pozycjaDnia =
		  DefinicjaCykluPozycjaDnia.Pierwszy,
		DefinicjaCykluOkresCyklu okresCyklu = DefinicjaCykluOkresCyklu.Biezacy,
		DefinicjaCykluSposobNaDniWolne dniWolne =
		  DefinicjaCykluSposobNaDniWolne.None) where P : PozycjaDokHandlowego
	  =>
		builder.Enqueue(
		  p =>
			p.UmowaInfo.Cykl.Set(
			  typ,
			  rodzajTerminu,
			  dzienMiesiaca,
			  pozycjaDnia,
			  okresCyklu,
			  dniWolne));

	}
}
