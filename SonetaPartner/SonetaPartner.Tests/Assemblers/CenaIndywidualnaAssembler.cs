using Soneta.Test;
using Soneta.Towary;
using Soneta.Types;

namespace SonetaPartner.Tests.Assemblers
{
	static class CenaIndywidualnaAssembler
	{
		internal static IRowBuilder<CenaIndywidualna> Netto(
			this IRowBuilder<CenaIndywidualna> builder,
			double value)
		{
			return builder.Enqueue(c => c.Netto = new DoubleCy(value));
		}
		
		internal static IRowBuilder<CenaIndywidualna> Rabat(
			this IRowBuilder<CenaIndywidualna> builder,
			decimal value)
		{
			return builder.Enqueue(c => c.Rabat = new Percent(value));
		}
	}
}
