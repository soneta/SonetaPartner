using Soneta.Magazyny;
using Soneta.Test;

namespace SonetaPartner.Tests.Assemblers
{
	static class MagazynAssembler
	{
		internal static IRowBuilder<Magazyn> Symbol(this IRowBuilder<Magazyn> builder, string symbol)
		{
			return builder.Enqueue(m => m.Symbol = symbol);
		}

		internal static IRowBuilder<Magazyn> Nazwa(this IRowBuilder<Magazyn> builder, string nazwa)
		{
			return builder.Enqueue(m => m.Nazwa = nazwa);
		}

		internal static IRowBuilder<Magazyn> Opis(this IRowBuilder<Magazyn> builder, string opis)
		{
			return builder.Enqueue(m => m.Opis = opis);
		}
	}
}
