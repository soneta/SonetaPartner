using Soneta.Test;
using Soneta.Towary;
using Soneta.Types;
using SonetaPartner.Tests.Extensions.Handel.Engine;

namespace SonetaPartner.Tests.Assemblers
{
	static class PrzecenaOkresowaCenyAssembler
	{
		internal static IRowBuilder<PrzecenaOkresowaTowaru> Towar(
			this IRowBuilder<PrzecenaOkresowaTowaru> builder,
			string towar)
		{
			return builder.Enqueue(pot => pot.Towar = pot.Module.Towary.WgKodu[towar]);
		}

		internal static IRowBuilder<PrzecenaOkresowaTowaru> PrzecenaOkresowaTowaru(
			this IRowBuilder<PrzecenaOkresowaCeny> builder,
			string towar)
		{
			return builder.GetChild<PrzecenaOkresowaTowaru>(
				cx => cx.Set(defDokHandlowego: null, magazyn: null, kontrahent: null, towar: towar),
				builderOptions: BuilderOptions.SetResultIntoContext_No).Towar(towar);
		}

		internal static IRowBuilder<PrzecenaOkresowaCeny> TypPrzeceny(
			this IRowBuilder<PrzecenaOkresowaCeny> builder,
			TypPrzecenyOkresowej typPrzeceny)
		{
			return builder.Enqueue(po => po.Typ = typPrzeceny);
		}

		internal static IRowBuilder<PrzecenaOkresowaCeny> Zatwierdz(
			this IRowBuilder<PrzecenaOkresowaCeny> builder)
		{
			return builder.Enqueue(po => po.Zatwierdzona = true);
		}

		internal static IRowBuilder<PrzecenaOkresowaTowaru> Netto(
			this IRowBuilder<PrzecenaOkresowaTowaru> builder,
			double value)
		{
			return builder.Enqueue(pot => pot.Netto = new DoubleCy(value));
		}

		internal static IRowBuilder<PrzecenaOkresowaCeny> PrzecenaOkresowaCeny(
			this IRowBuilder<PrzecenaOkresowaTowaru> builder)
		{
			return builder.GetParent<PrzecenaOkresowaCeny>();
		}
	}
}
