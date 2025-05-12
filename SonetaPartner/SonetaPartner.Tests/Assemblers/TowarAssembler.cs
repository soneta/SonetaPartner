using Soneta.Core;
using System;
using Soneta.Magazyny;
using Soneta.Magazyny.StanMagazynu;
using Soneta.Test;
using Soneta.Test.Helpers;
using Soneta.Towary;
using Soneta.Types;
using Soneta.Business;

namespace SonetaPartner.Tests.Assemblers
{
	static class TowarAssembler
	{
		internal static IRowBuilder<Cena> Cena(this IRowBuilder<Towar> builder, string name)
			=> Cena(builder, name, Date.Today);

		internal static IRowBuilder<Cena> Cena(this IRowBuilder<Towar> builder, string name, Date data)
		{
			return builder.GetChild(
				t => t.Module.Ceny.WgDaty(t, t.Module.DefinicjeCen[name], data),
				alternativeBuildOptions: BuildActionOptions.CommitUI_No,
				builderOptions: BuilderOptions.SetResultIntoContext_No);
		}

		internal static IRowBuilder<Towar> Kod(this IRowBuilder<Towar> builder, string value)
		{
			return builder.Enqueue(t => t.Kod = value);
		}

		internal static IRowBuilder<Towar> Nazwa(this IRowBuilder<Towar> builder, string value)
		{
			return builder.Enqueue(t => t.Nazwa = value);
		}
	}
}
