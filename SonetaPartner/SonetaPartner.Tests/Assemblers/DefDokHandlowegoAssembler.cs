using Soneta.Handel;
using Soneta.Magazyny;
using Soneta.Test;
using System;

namespace SonetaPartner.Tests.Assemblers
{
	static class DefDokHandlowegoAssembler
	{

		internal static IRowBuilder<DefDokHandlowego> WskazaniePartii(
			this IRowBuilder<DefDokHandlowego> builder)
		{
			return builder.Enqueue(d => d.UstawieniaWskazaniePartii.DoZamowien = true);
		}

		internal static IRowBuilder<DefDokHandlowego> Blokada(
			this IRowBuilder<DefDokHandlowego> builder,
			bool value)
		{
			return builder.Enqueue(d => d.Blokada = value);
		}

		internal static IRowBuilder<DefDokHandlowego> MomentMagazynu(
		   this IRowBuilder<DefDokHandlowego> builder,
		   MomentMagazynu momentMagazynu)
		{
			return builder.Enqueue(d => d.MomentMagazynu = momentMagazynu);
		}

		internal static IRowBuilder<DefRelacjiHandlowej> DefRelacji(
			this IRowBuilder<DefDokHandlowego> builder,
			string nazwaZNadrzednego)
		{
			return builder.GetChild<DefRelacjiHandlowej>(
				(d, tc) => Array.Find(
					d.Podrzedne.ToArray<DefRelacjiHandlowej>(),
					dr => dr.ZNadrzednego.Nazwa == nazwaZNadrzednego),
				alternativeBuildOptions: BuildActionOptions.CommitUI_No,
				builderOptions: BuilderOptions.SetResultIntoContext_No);
		}

		internal static IRowBuilder<DefRelacjiHandlowej> Agregowanie(
			this IRowBuilder<DefRelacjiHandlowej> builder,
			SposobLaczeniaPozycji value,
			string feature = null)
		{
			builder = builder.Enqueue(d => d.Zachowanie.LaczeniePozycji = value);
			if (!string.IsNullOrEmpty(feature))
			{
				builder = builder.Enqueue(d => d.Zachowanie.CechaLaczenia = feature);
			}

			return builder;
		}
	}
}
